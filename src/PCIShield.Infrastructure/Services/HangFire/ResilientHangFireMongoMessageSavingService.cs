using System;
using System.Reactive.Linq;
using System.Threading;
using System.Threading.Tasks;
using Hangfire;
using MediatR;
using Hangfire.Storage;
using PCIShieldLib.SharedKernel.Interfaces;
using System.Collections.Concurrent;
using System.Linq;
using Hangfire.Storage.Monitoring;
namespace PCIShield.Infrastructure.Services;
public interface IResilientHangFireMongoMessageSavingService<T>
    where T : class
{
    bool IsJobCompleted(Guid correlationId);
    void PublishCreateAsync<T>(T entity, Guid correlationId)
        where T : IMongoAggregateRoot;
    void PublishCreateAsyncInternal<T>(T entity)
        where T : IMongoAggregateRoot;
    void PublishDeleteAsync(string id);
    void PublishDeleteAsyncInternal(string id);
    void PublishUpdateAsync<T>(string id, T entity)
        where T : IMongoAggregateRoot;
    void PublishUpdateAsyncInternal<T>(string id, T entity)
        where T : IMongoAggregateRoot;
}
public class MongoEntityCreatedEvent<T> : INotification
{
    public MongoEntityCreatedEvent(T entity)
    {
        Entity = entity;
    }
    public T Entity { get; }
}
public class MongoEntityCreatedEventService<T>
    : INotificationHandler<MongoEntityCreatedEvent<T>>
    where T : class
{
    private readonly IMongoRepository<T> _mongoRepository;
    public MongoEntityCreatedEventService(IMongoRepository<T> mongoRepository)
    {
        _mongoRepository = mongoRepository;
    }
    public async Task Handle(
        MongoEntityCreatedEvent<T> notification,
        CancellationToken cancellationToken
    )
    {
        try
        {
            await _mongoRepository.CreateAsync(notification.Entity);
        }
        catch (Exception e)
        {
        }
    }
}
public class MongoEntityDeletedEvent<T> : INotification
{
    public MongoEntityDeletedEvent(string id)
    {
        Id = id;
    }
    public string Id { get; }
}
public class MongoEntityDeletedEventService<T>
    : INotificationHandler<MongoEntityDeletedEvent<T>>
    where T : class
{
    private readonly IMongoRepository<T> _mongoRepository;
    public MongoEntityDeletedEventService(IMongoRepository<T> mongoRepository)
    {
        _mongoRepository = mongoRepository;
    }
    public async Task Handle(
        MongoEntityDeletedEvent<T> notification,
        CancellationToken cancellationToken
    )
    {
        await _mongoRepository.DeleteAsync(notification.Id);
    }
}
public class MongoEntityUpdatedEvent<T> : INotification
{
    public MongoEntityUpdatedEvent(string id, T entity)
    {
        Id = id;
        Entity = entity;
    }
    public T Entity { get; }
    public string Id { get; }
}
public class MongoEntityUpdatedEventService<T>
    : INotificationHandler<MongoEntityUpdatedEvent<T>>
    where T : class
{
    private readonly IMongoRepository<T> _mongoRepository;
    public MongoEntityUpdatedEventService(IMongoRepository<T> mongoRepository)
    {
        _mongoRepository = mongoRepository;
    }
    public async Task Handle(
        MongoEntityUpdatedEvent<T> notification,
        CancellationToken cancellationToken
    )
    {
        await _mongoRepository.UpdateByCustomerIdAsync(notification.Id, notification.Entity);
    }
}
public class ResilientHangFireMongoMessageSavingService<T>
    : IResilientHangFireMongoMessageSavingService<T>
    where T : class
{
    private readonly IBackgroundJobClient _backgroundJobClient;
    private readonly IAppLoggerService<ResilientHangFireMongoMessageSavingService<T>> _logger;
    private readonly IMediator _mediator;
    private readonly IMongoRepository<T> _mongoRepository;
    private ConcurrentDictionary<Guid, string> _correlationIdToJobIdMap = new();
    public ResilientHangFireMongoMessageSavingService(
        IBackgroundJobClient backgroundJobClient,
        IMediator mediator,
        IAppLoggerService<ResilientHangFireMongoMessageSavingService<T>> logger,
        IMongoRepository<T> mongoRepository
    )
    {
        _backgroundJobClient = backgroundJobClient;
        _mediator = mediator;
        _logger = logger;
        _mongoRepository = mongoRepository;
    }
    public bool IsJobCompleted(Guid correlationId)
    {
        if (!_correlationIdToJobIdMap.TryGetValue(correlationId, out string jobId))
        {
            return false;
        }
        IMonitoringApi? monitor = JobStorage.Current.GetMonitoringApi();
        JobDetailsDto? jobDetails = monitor.JobDetails(jobId);
        return jobDetails?.History.Any(x => x.StateName == "Succeeded") ?? false;
    }
    public void PublishCreateAsync<T>(T entity, Guid correlationId)
        where T : IMongoAggregateRoot
    {
        try
        {
            string jobId = _backgroundJobClient.Enqueue(
                () => PublishCreateAsyncInternal(entity)
            );
            _correlationIdToJobIdMap[correlationId] = jobId;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "An error occurred while enqueuing a Create operation.");
        }
    }
    public void PublishCreateAsyncInternal<T>(T entity)
        where T : IMongoAggregateRoot
    {
        try
        {
            _mediator.Publish(new MongoEntityCreatedEvent<T>(entity)).Wait();
        }
        catch (Exception ex)
        {
            _logger.LogError(
                ex,
                "An error occurred while publishing a a Create operation internally."
            );
        }
    }
    public void PublishDeleteAsync(string id)
    {
        try
        {
            _backgroundJobClient.Enqueue(() => PublishDeleteAsyncInternal(id));
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "An error occurred while enqueuing a Delete operation.");
        }
    }
    public void PublishDeleteAsyncInternal(string id)
    {
        try
        {
            _mediator.Publish(new MongoEntityDeletedEvent<T>(id)).Wait();
        }
        catch (Exception ex)
        {
            _logger.LogError(
                ex,
                "An error occurred while publishing a Delete operation internally."
            );
        }
    }
    public void PublishUpdateAsync<T>(string id, T entity)
        where T : IMongoAggregateRoot
    {
        try
        {
            _backgroundJobClient.Enqueue(() => PublishUpdateAsyncInternal(id, entity));
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "An error occurred while enqueuing an Update operation.");
        }
    }
    public void PublishUpdateAsyncInternal<T>(string id, T entity)
        where T : IMongoAggregateRoot
    {
        try
        {
            _mediator.Publish(new MongoEntityUpdatedEvent<T>(id, entity)).Wait();
            ;
        }
        catch (Exception ex)
        {
            _logger.LogError(
                ex,
                "An error occurred while publishing an Update operation internally."
            );
        }
    }
}