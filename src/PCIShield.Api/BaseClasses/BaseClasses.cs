using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Diagnostics;
using System.Linq;
using System.Linq.Expressions;
using System.Reactive.Concurrency;
using System.Reactive.Linq;
using System.Reactive.Subjects;
using System.Reflection;
using System.Threading;
using System.Threading.Tasks;
using Ardalis.GuardClauses;
using Ardalis.Specification;
using FastEndpoints;
using FluentValidation;
using FluentValidation.Results;
using PCIShield.BlazorMauiShared.Models;
using PCIShield.Infrastructure.Services;
using PCIShield.Infrastructure.Services.Redis;
using PCIShieldLib.SharedKernel.Interfaces;
using LanguageExt;
using MediatR;
using Microsoft.EntityFrameworkCore;
using ModelingEvolution.Plumberd;
using ModelingEvolution.Plumberd.EventStore;
using ModelingEvolution.Plumberd.Metadata;
using static LanguageExt.Prelude;
using Sort = PCIShieldLib.SharedKernel.Interfaces.Sort;
using Unit = LanguageExt.Unit;
namespace PCIShield.Api.BaseClasses;
public record Error(string Message);
public record ValidationError(string Field, string Message) : Error(Message);
public record NotFoundError(string Entity, string Id) : Error($"{Entity} with id {Id} not found");

public class ValidationResult
{
    public string Rule { get; set; }
    public ValidationSeverity Severity { get; set; }
    public string Message { get; set; }
}

public enum ValidationSeverity
{
    Error,
    Warning,
    Info
}

public record DomainEvent(Guid Id, DateTime Timestamp);
public record FilterRequest(
    int PageNumber = 1,
    int PageSize = 10,
    ImmutableDictionary<string, string> Filters = null,
    Option<string> SearchTerm = default,
    ImmutableList<Sort> Sorting = null)
{
    public string ToCacheKey()
    {
        return $"{GetType().Name}-{PageNumber}-{PageSize}"
               + SearchTerm.Match(term => $"-{term}", () => string.Empty)
               + (Filters?.IsEmpty == false
                   ? $"-{string.Join("-", Filters.Select(f => $"{f.Key}_{f.Value}"))}"
                   : string.Empty)
               + (Sorting?.IsEmpty == false
                   ? $"-{string.Join("-", Sorting.Select(s => $"{s.Field}_{s.Direction}"))}"
                   : string.Empty);
    }
}
public interface IApiService<TEntity, TId>
    where TEntity : class, IAggregateRoot
{
    TryAsync<TDto> CreateAsync<TDto>(TDto dto)
        where TDto : class;
    TryAsync<Unit> DeleteAsync(TId id);
    IObservable<TResponse> FilterAsync<TResponse>(FilterRequest request);
    TryAsync<TEntity> GetByIdAsync(TId id);
    TryAsync<TResponse> GetByIdAsync<TResponse>(TId id);
    TryAsync<TDto> UpdateAsync<TDto>(TId id, TDto dto)
        where TDto : class;
}
public class BaseSearchSpecification<TEntity, TDto> : Specification<TEntity, TDto>
    where TEntity : class
{
    public BaseSearchSpecification(string searchTerm)
    {
        string searchLower = searchTerm?.ToLower() ?? string.Empty;
        Query.Where(BuildSearchPredicate(searchLower)).OrderByDescending(x => EF.Property<DateTime>(x, "CreatedDate"))
            .AsNoTracking();
    }
    protected virtual Expression<Func<TEntity, bool>> BuildSearchPredicate(string searchLower)
    {
        return x => false;
    }
}
public abstract class CachedApiService<TEntity, TId> : IApiService<TEntity, TId>
    where TEntity : class, IAggregateRoot
{
    private readonly IReadRedisRepository<TEntity> _cache;
    private readonly ISubject<CacheInvalidation> _cacheInvalidations = new Subject<CacheInvalidation>();
    private readonly string _cachePrefix;
    private readonly IAppLoggerService<CachedApiService<TEntity, TId>> _logger;
    private readonly IRedisCacheService _redisCacheService;
    private readonly IKeyCloakTenantService _tenantService;
    protected CachedApiService(
        IReadRedisRepository<TEntity> cache,
        IRedisCacheService redisCacheService,
        IKeyCloakTenantService tenantService,
        IAppLoggerService<CachedApiService<TEntity, TId>> logger,
        string cachePrefix)
    {
        _cache = cache;
        _redisCacheService = redisCacheService;
        _tenantService = tenantService;
        _logger = logger;
        _cachePrefix = cachePrefix;
        _cacheInvalidations
            .Buffer(TimeSpan.FromMilliseconds(100))
            .Where(x => x.Any())
            .Select(
                batch => Observable.FromAsync(() => Task.WhenAll(batch.Select(InvalidateCache))).Retry(1)
                    .Timeout(TimeSpan.FromSeconds(5)))
            .Concat()
            .Subscribe();
    }
    public TryAsync<TDto> CreateAsync<TDto>(TDto dto)
        where TDto : class
    {
        return from entity in TryAsync(() => CreateEntityAsync(dto))
            from _ in InvalidateCacheAsync(GetEntityId(entity))
            select MapToDto<TDto>(entity);
    }
    public TryAsync<Unit> DeleteAsync(TId id)
    {
        return from _ in TryAsync(() => DeleteEntityAsync(id)) from __ in InvalidateCacheAsync(id) select Unit.Default;
    }
    public IObservable<TResponse> FilterAsync<TResponse>(FilterRequest request)
    {
        return Observable
            .FromAsync(
                async () =>
                {
                    ISpecification<TEntity, TResponse> spec = CreateFilterSpecification<TResponse>(request);
                    return await _cache.ListAsync(spec);
                })
            .SelectMany(x => x)
            .SubscribeOn(TaskPoolScheduler.Default);
    }
    public TryAsync<TEntity> GetByIdAsync(TId id)
    {
        return TryAsync(
            async () =>
            {
                ISpecification<TEntity> spec = CreateByIdSpecification(id);
                TEntity? entity = await _cache.FirstOrDefaultAsync(spec);
                return entity != null
                    ? entity
                    : throw new NotFoundException($"{typeof(TEntity).Name} with id {id} not found");
            });
    }
    public TryAsync<TResponse> GetByIdAsync<TResponse>(TId id)
    {
        return TryAsync(
            async () =>
            {
                ISpecification<TEntity, TResponse> spec = CreateByIdSpecification<TResponse>(id);
                TResponse? result = await _cache.FirstOrDefaultAsync(spec);
                return result != null
                    ? result
                    : throw new NotFoundException($"{typeof(TResponse).Name} with id {id} not found");
            });
    }
    public TryAsync<TDto> UpdateAsync<TDto>(TId id, TDto dto)
        where TDto : class
    {
        return from entity in TryAsync(() => UpdateEntityAsync(id, dto))
            from _ in InvalidateCacheAsync(id)
            select MapToDto<TDto>(entity);
    }
    protected abstract ISpecification<TEntity> CreateByIdSpecification(TId id);
    protected abstract ISpecification<TEntity, TResponse> CreateByIdSpecification<TResponse>(TId id);
    protected abstract Task<TEntity> CreateEntityAsync<TDto>(TDto dto)
        where TDto : class;
    protected abstract ISpecification<TEntity, TResponse> CreateFilterSpecification<TResponse>(FilterRequest request);
    protected abstract Task DeleteEntityAsync(TId id);
    protected abstract TId GetEntityId(TEntity entity);
    protected TryAsync<Unit> InvalidateCacheAsync(TId id)
    {
        return TryAsync(
            async () =>
            {
                await InvalidateCache(new CacheInvalidation(id));
                _cacheInvalidations.OnNext(new CacheInvalidation(id));
                return Unit.Default;
            });
    }
    protected abstract TDto MapToDto<TDto>(TEntity entity)
        where TDto : class;
    protected abstract Task<TEntity> UpdateEntityAsync<TDto>(TId id, TDto dto)
        where TDto : class;
    private async Task InvalidateCache(CacheInvalidation invalidation)
    {
        string? tenantId = _tenantService.GetPCIShieldSolutionuperUserId();
        string[] patterns = new[]
        {
            $"{_cachePrefix}-{invalidation.Id}--{tenantId}-*", $"{_cachePrefix}ListSpec--{tenantId}-*",
            $"{_cachePrefix}ListSpec-*--{tenantId}-*"
        };
        await Task.WhenAll(patterns.Select(_redisCacheService.RemoveByPatternAsync));
    }
    private record CacheInvalidation(TId Id);
}
public abstract class CrudEndpointBase<TRequest, TResponse, TService, TEntity, TId>
    where TRequest : BaseRequest
    where TResponse : BaseResponse, new()
    where TService : IApiService<TEntity, TId>
    where TEntity : class, IAggregateRoot
{
    private readonly TaskCompletionSource<Unit> _eventProcessing = new();
    private readonly ISubject<DomainEvent> _events = new Subject<DomainEvent>();
    private readonly IAppLoggerService<TEntity> _logger;
    private readonly IMediator _mediator;
    private readonly TService _service;
    private readonly IValidator<TRequest>? _validator;
    protected CrudEndpointBase(
        TService service,
        IMediator mediator,
        IAppLoggerService<TEntity> logger,
        IValidator<TRequest>? validator = null)
    {
        _service = service;
        _mediator = mediator;
        _logger = logger;
        _validator = validator;
        _events
            .Buffer(TimeSpan.FromMilliseconds(100))
            .Where(batch => batch.Any())
            .Subscribe(
                async batch =>
                {
                    try
                    {
                        await Task.WhenAll(batch.Select(evt => _mediator.Publish(evt)));
                    }
                    catch (Exception ex)
                    {
                        _logger.LogError(ex, "Error processing domain events");
                    }
                    finally
                    {
                        _eventProcessing.TrySetResult(Unit.Default);
                    }
                });
    }
    protected void RaiseDomainEvent(DomainEvent evt)
    {
        _events.OnNext(evt);
    }
    protected async Task WaitForEventProcessing()
    {
        await _eventProcessing.Task;
    }
    protected IObservable<TResponse> ExecuteOperation<TResult>(
        Func<TryAsync<TResult>> operation,
        Func<TResult, TResponse> mapper)
    {
        return Observable
            .FromAsync(
                () =>
                    operation()
                        .Match(
                            val => Task.FromResult(mapper(val)),
                            ex =>
                            {
                                _logger.LogError(ex, "Operation failed");
                                return Task.FromResult(new TResponse { IsSuccess = false, ErrorMessage = ex.Message });
                            }))
            .SubscribeOn(TaskPoolScheduler.Default);
    }
    protected abstract Task<Either<Error, TResponse>> ProcessRequestAsync(TRequest request, CancellationToken ct);
}
public class DefaultFilterSpecification<TEntity, TDto> : Specification<TEntity, TDto>
    where TEntity : class
{
    public DefaultFilterSpecification(
        int pageNumber,
        int pageSize,
        ImmutableDictionary<string, string> filters,
        ImmutableList<Sort> sorting)
    {
         Guard.Against.Negative(pageNumber, nameof(pageNumber));
         Guard.Against.Negative(pageSize, nameof(pageSize));
        Query.Where(x => !EF.Property<bool>(x, "IsDeleted"));
        ApplyFilters(filters);
        ApplySorting(sorting);
        Query.Skip((pageNumber - 1) * pageSize).Take(pageSize).AsNoTracking();
    }
    private enum SortDirection
    {
        Ascending,
        Descending
    }
    private void ApplyFilters(ImmutableDictionary<string, string> filters)
    {
        if (!filters.IsEmpty)
        {
            foreach (KeyValuePair<string, string> filter in filters)
            {
                PropertyInfo? property = typeof(TEntity).GetProperty(filter.Key);
                if (property != null)
                {
                    ParameterExpression parameter = Expression.Parameter(typeof(TEntity), "x");
                    MemberExpression propertyAccess = Expression.Property(parameter, property);
                    ConstantExpression constant = Expression.Constant(filter.Value);
                    MethodInfo? containsMethod = typeof(string).GetMethod("Contains", new[] { typeof(string) });
                    MethodCallExpression body = Expression.Call(propertyAccess, containsMethod, constant);
                    Expression<Func<TEntity, bool>> predicate = Expression.Lambda<Func<TEntity, bool>>(body, parameter);
                    Query.Where(predicate);
                }
            }
        }
    }
    private void ApplySorting(ImmutableList<Sort> sorting)
    {
        if (!sorting.IsEmpty)
        {
            Sort first = sorting.First();
            IOrderedSpecificationBuilder<TEntity> ordered =
                first.Direction == PCIShieldLib.SharedKernel.Interfaces.SortDirection.Descending
                    ? Query.OrderByDescending(x => EF.Property<object>(x, first.Field))
                    : Query.OrderBy(x => EF.Property<object>(x, first.Field));
            IOrderedSpecificationBuilder<TEntity> orderedBuilder = (IOrderedSpecificationBuilder<TEntity>)ordered;
            foreach (Sort sort in sorting.Skip(1))
            {
                if (sort.Direction == PCIShieldLib.SharedKernel.Interfaces.SortDirection.Descending)
                {
                    orderedBuilder = orderedBuilder.ThenByDescending(x => EF.Property<object>(x, sort.Field));
                }
                else
                {
                    orderedBuilder = orderedBuilder.ThenBy(x => EF.Property<object>(x, sort.Field));
                }
            }
        }
        else
        {
            Query.OrderByDescending(x => EF.Property<DateTime>(x, "CreatedDate"));
        }
    }
    private Expression<Func<TEntity, object>> GetSortExpression(string field)
    {
        ParameterExpression parameter = Expression.Parameter(typeof(TEntity), "x");
        MemberExpression property = Expression.Property(parameter, field);
        UnaryExpression conversion = Expression.Convert(property, typeof(object));
        return Expression.Lambda<Func<TEntity, object>>(conversion, parameter);
    }
}
public class EndpointResponse<T>
{
    public Option<T> Data { get; init; }
    public Option<Error> Error { get; init; }
    public bool IsSuccess { get; init; }
    public TimeSpan ProcessingTime { get; init; }
    public static EndpointResponse<T> Failure(Error error, TimeSpan time)
    {
        return new EndpointResponse<T> { IsSuccess = false, Data = None, Error = Some(error), ProcessingTime = time };
    }
    public static EndpointResponse<T> Success(T data, TimeSpan time)
    {
        return new EndpointResponse<T> { IsSuccess = true, Data = Some(data), Error = None, ProcessingTime = time };
    }
}
public class EventProcessor
{
    private readonly Subject<DomainEvent> _events = new();
    public EventProcessor(IMediator mediator)
    {
        _events
            .Buffer(TimeSpan.FromMilliseconds(100))
            .Where(events => events.Any())
            .Select(batch => Observable.FromAsync(() => Task.WhenAll(batch.Select(evt => mediator.Publish(evt)))))
            .Concat()
            .Subscribe();
    }
}
public abstract class ListResponseBase<TDto>
    where TDto : class
{
    public int Count { get; init; }
    public string? ErrorMessage { get; init; }
    public bool IsSuccess { get; init; }
    public ImmutableList<TDto> Items { get; init; } = ImmutableList<TDto>.Empty;
    public string OperationElapsedTime { get; init; } = string.Empty;
    public string? OperationTypeNameId { get; init; }
}
public class NotFoundException : Exception
{
    public NotFoundException(string message)
        : base(message)
    {
    }
}
public abstract class OptimizedSpecification<TEntity, TResult> : Specification<TEntity, TResult>
    where TEntity : class
{
    private static readonly ConcurrentDictionary<string, Expression<Func<TEntity, bool>>> FilterCache = new();
    private static readonly ConcurrentDictionary<string, PropertyInfo> PropertyCache = new();
    private static readonly ConcurrentDictionary<string, Expression<Func<TEntity, object>>> SortCache = new();
    protected OptimizedSpecification(FilterRequest request)
    {
         Guard.Against.Negative(request.PageNumber, nameof(request.PageNumber));
         Guard.Against.Negative(request.PageSize, nameof(request.PageSize));
        Query.Where(x => !EF.Property<bool>(x, "IsDeleted"));
        request.SearchTerm.Match(term => ApplySearch(term), () => ApplyDefaultOrder());
        if (request.Filters?.IsEmpty == false)
        {
            ApplyOptimizedFilters(request.Filters);
        }
        if (request.Sorting?.IsEmpty == false)
        {
            ApplyOptimizedSorting(request.Sorting);
        }
        else
        {
            ApplyDefaultOrder();
        }
        Query.Skip((request.PageNumber - 1) * request.PageSize).Take(request.PageSize).AsNoTracking();
    }
    protected abstract Expression<Func<TEntity, bool>> BuildSearchPredicate(string searchTerm);
    private static Expression<Func<TEntity, bool>> CreateFilterExpression(PropertyInfo property, string value)
    {
        ParameterExpression param = Expression.Parameter(typeof(TEntity), "x");
        MemberExpression prop = Expression.Property(param, property);
        Expression body;
        if (property.PropertyType == typeof(string))
        {
            body = Expression.Call(prop, "Contains", null, Expression.Constant(value, typeof(string)));
        }
        else if (property.PropertyType == typeof(DateTime))
        {
            body = Expression.Equal(prop, Expression.Constant(DateTime.Parse(value)));
        }
        else if (property.PropertyType.IsEnum)
        {
            body = Expression.Equal(prop, Expression.Constant(Enum.Parse(property.PropertyType, value)));
        }
        else
        {
            body = Expression.Equal(prop, Expression.Constant(Convert.ChangeType(value, property.PropertyType)));
        }
        return Expression.Lambda<Func<TEntity, bool>>(body, param);
    }
    private static Expression<Func<TEntity, object>> CreateSortExpression(string field)
    {
        ParameterExpression param = Expression.Parameter(typeof(TEntity), "x");
        MemberExpression prop = Expression.Property(param, GetCachedProperty(field));
        UnaryExpression conv = Expression.Convert(prop, typeof(object));
        return Expression.Lambda<Func<TEntity, object>>(conv, param);
    }
    private static PropertyInfo GetCachedProperty(string propertyName)
    {
        return PropertyCache.GetOrAdd(
            $"{typeof(TEntity).Name}_{propertyName}",
            _ => typeof(TEntity).GetProperty(propertyName));
    }
    private void ApplyDefaultOrder()
    {
        Query.OrderByDescending(x => EF.Property<DateTime>(x, "CreatedDate"));
    }
    private void ApplyOptimizedFilters(ImmutableDictionary<string, string> filters)
    {
        foreach ((string key, string value) in filters.Where(f => !string.IsNullOrEmpty(f.Value)))
        {
            PropertyInfo? property = GetCachedProperty(key);
            if (property != null)
            {
                Expression<Func<TEntity, bool>> predicate = BuildFilterPredicate(property, value);
                Query.Where(predicate);
            }
        }
    }
    private void ApplyOptimizedSorting(ImmutableList<Sort> sorting)
    {
        Sort first = sorting.First();
        IOrderedSpecificationBuilder<TEntity> ordered =
            first.Direction == SortDirection.Descending
                ? Query.OrderByDescending(GetSortExpression(first.Field))
                : Query.OrderBy(GetSortExpression(first.Field));
        foreach (Sort sort in sorting.Skip(1))
        {
            ordered = sort.Direction == SortDirection.Descending
                ? ordered.ThenByDescending(GetSortExpression(sort.Field))
                : ordered.ThenBy(GetSortExpression(sort.Field));
        }
    }
    private void ApplySearch(string searchTerm)
    {
        Query.Where(BuildSearchPredicate(searchTerm));
    }
    private Expression<Func<TEntity, bool>> BuildFilterPredicate(PropertyInfo property, string value)
    {
        return FilterCache.GetOrAdd(
            $"{typeof(TEntity).Name}_{property.Name}_{value}",
            _ => CreateFilterExpression(property, value));
    }
    private Expression<Func<TEntity, object>> GetSortExpression(string field)
    {
        return SortCache.GetOrAdd($"{typeof(TEntity).Name}_{field}", _ => CreateSortExpression(field));
    }
}
public abstract class ReactiveApiService<TEntity, TId> : IApiService<TEntity, TId>
    where TEntity : class, IAggregateRoot
{
    private readonly IReadRedisRepository<TEntity> _cache;
    private readonly Subject<CacheInvalidation> _cacheInvalidations = new();
    private readonly string _cachePrefix;
    private readonly IAppLoggerService<ReactiveApiService<TEntity, TId>> _logger;
    private readonly IRedisCacheService _redisCacheService;
    private readonly IKeyCloakTenantService _tenantService;
    private readonly IScheduler _scheduler = TaskPoolScheduler.Default;
    protected ReactiveApiService(
        IReadRedisRepository<TEntity> cache,
        IRedisCacheService redisCacheService,
        IKeyCloakTenantService tenantService,
        IAppLoggerService<ReactiveApiService<TEntity, TId>> logger,
        string cachePrefix)
    {
        _cache = cache;
        _redisCacheService = redisCacheService;
        _tenantService = tenantService;
        _logger = logger;
        _cachePrefix = cachePrefix;
        _cacheInvalidations
            .Buffer(TimeSpan.FromMilliseconds(100))
            .Where(batch => batch.Any())
            .Select(batch => Observable.FromAsync(() => Task.WhenAll(batch.Select(InvalidateCache))))
            .Retry(1)
            .Timeout(TimeSpan.FromSeconds(5))
            .Concat()
            .Subscribe(_ => { }, ex => _logger.LogError(ex, "Cache invalidation failed"));
    }
    public TryAsync<TDto> CreateAsync<TDto>(TDto dto)
        where TDto : class
    {
        return from entity in TryAsync(() => CreateEntityAsync(dto))
            from _ in InvalidateCacheAsync(GetEntityId(entity))
            select MapToDto<TDto>(entity);
    }
    public TryAsync<Unit> DeleteAsync(TId id)
    {
        return from _ in TryAsync(() => DeleteEntityAsync(id)) from __ in InvalidateCacheAsync(id) select Unit.Default;
    }
    public IObservable<TResponse> FilterAsync<TResponse>(FilterRequest request)
    {
        return Observable
            .FromAsync(
                async () =>
                {
                    ISpecification<TEntity, TResponse> spec = CreateFilterSpecification<TResponse>(request);
                    return await _cache.ListAsync(spec);
                })
            .SelectMany(x => x)
            .SubscribeOn(_scheduler);
    }
    public TryAsync<TEntity> GetByIdAsync(TId id)
    {
        return TryAsync(
            async () =>
            {
                ISpecification<TEntity> spec = CreateByIdSpecification(id);
                TEntity? result = await _cache.FirstOrDefaultAsync(spec);
                return result ?? throw new NotFoundException($"{typeof(TEntity).Name} with id {id} not found");
            });
    }
    public TryAsync<TResponse> GetByIdAsync<TResponse>(TId id)
    {
        return TryAsync(
            async () =>
            {
                ISpecification<TEntity, TResponse> spec = CreateByIdSpecification<TResponse>(id);
                TResponse? result = await _cache.FirstOrDefaultAsync(spec);
                return result ?? throw new NotFoundException($"{typeof(TResponse).Name} with id {id} not found");
            });
    }
    public TryAsync<TDto> UpdateAsync<TDto>(TId id, TDto dto)
        where TDto : class
    {
        return from entity in TryAsync(() => UpdateEntityAsync(id, dto))
            from _ in InvalidateCacheAsync(id)
            select MapToDto<TDto>(entity);
    }
    protected abstract ISpecification<TEntity> CreateByIdSpecification(TId id);
    protected abstract ISpecification<TEntity, TResponse> CreateByIdSpecification<TResponse>(TId id);
    protected abstract Task<TEntity> CreateEntityAsync<TDto>(TDto dto)
        where TDto : class;
    protected abstract ISpecification<TEntity, TResponse> CreateFilterSpecification<TResponse>(FilterRequest request);
    protected abstract Task DeleteEntityAsync(TId id);
    protected abstract TId GetEntityId(TEntity entity);
    protected abstract TDto MapToDto<TDto>(TEntity entity)
        where TDto : class;
    protected abstract Task<TEntity> UpdateEntityAsync<TDto>(TId id, TDto dto)
        where TDto : class;
    protected TryAsync<Unit> InvalidateCacheAsync(TId id)
    {
        return TryAsync(
            async () =>
            {
                await InvalidateCache(new CacheInvalidation(id));
                _cacheInvalidations.OnNext(new CacheInvalidation(id));
                return Unit.Default;
            });
    }
    private async Task InvalidateCache(CacheInvalidation invalidation)
    {
        string? tenantId = _tenantService.GetPCIShieldSolutionuperUserId();
        string[] patterns = new[]
        {
            $"{_cachePrefix}-{invalidation.Id}--{tenantId}-*", $"{_cachePrefix}ListSpec--{tenantId}-*",
            $"{_cachePrefix}ListSpec-*--{tenantId}-*"
        };
        await Task.WhenAll(patterns.Select(_redisCacheService.RemoveByPatternAsync));
    }
    private record CacheInvalidation(TId Id);
}
public abstract class ReactiveEndpoint<TRequest, TResponse>
{
    private readonly ReactiveEventProcessor<DomainEvent> _eventProcessor;
    protected ReactiveEndpoint(IMediator mediator)
    {
        _eventProcessor = new ReactiveEventProcessor<DomainEvent>(mediator);
    }
    protected void RaiseDomainEvent(DomainEvent evt)
    {
        _eventProcessor.Raise(evt);
    }
}
public abstract class ReactiveEndpointBase<TRequest, TResponse, TService, TEntity, TId>
    where TRequest : BaseRequest
    where TResponse : BaseResponse, new()
    where TService : IApiService<TEntity, TId>
    where TEntity : class, IAggregateRoot
{
    private readonly Subject<DomainEvent> _events = new();
    private readonly IAppLoggerService<TEntity> _logger;
    private readonly IMediator _mediator;
    private readonly IScheduler _scheduler = TaskPoolScheduler.Default;
    private readonly TService _service;
    private readonly IValidator<TRequest>? _validator;
    protected ReactiveEndpointBase(
        TService service,
        IMediator mediator,
        IAppLoggerService<TEntity> logger,
        IValidator<TRequest>? validator = null)
    {
        _service = service;
        _mediator = mediator;
        _logger = logger;
        _validator = validator;
        _events
            .Buffer(TimeSpan.FromMilliseconds(100))
            .Where(events => events.Any())
            .Select(
                events => Observable.FromAsync(() => Task.WhenAll(events.Select(evt => _mediator.Publish(evt))))
                    .Retry(1).Timeout(TimeSpan.FromSeconds(5)))
            .Concat()
            .Subscribe(_ => { }, ex => _logger.LogError(ex, "Event processing failed"));
    }
    protected IObservable<TResponse> ExecuteOperation<TResult>(
        Func<TryAsync<TResult>> operation,
        Func<TResult, TResponse> mapper)
    {
        return Observable
            .FromAsync(
                async () =>
                    await operation()
                        .Match(
                            value => mapper(value),
                            ex =>
                            {
                                _logger.LogError(ex, "Operation failed");
                                return new TResponse { IsSuccess = false, ErrorMessage = ex.Message };
                            }))
            .SubscribeOn(_scheduler);
    }
    protected void RaiseDomainEvent(DomainEvent evt)
    {
        _events.OnNext(evt);
    }
}
public abstract class ReactiveListEndpointBase<TRequest, TResponse, TEntity, TDto> : Endpoint<TRequest, TResponse>
    where TRequest : FilterRequest
    where TResponse : ListResponseBase<TDto>, new()
    where TEntity : class, IAggregateRoot
    where TDto : class
{
    private readonly IAppLoggerService<TEntity> _logger;
    private readonly IReadRedisRepository<TEntity> _repository;
    private readonly Subject<ListRequest> _requests = new();
    private readonly IScheduler _scheduler = TaskPoolScheduler.Default;
    protected ReactiveListEndpointBase(IReadRedisRepository<TEntity> repository, IAppLoggerService<TEntity> logger)
    {
        _repository = repository;
        _logger = logger;
        _requests.Buffer(TimeSpan.FromMilliseconds(50), 10).SelectMany(ProcessRequestBatch).Retry(3).Subscribe(
            _ => { },
            ex => _logger.LogError(ex, "Request batch processing failed"));
    }
    public override async Task HandleAsync(TRequest request, CancellationToken ct)
    {
        TaskCompletionSource<TResponse> tcs =
            new TaskCompletionSource<TResponse>(TaskCreationOptions.RunContinuationsAsynchronously);
        using CancellationTokenSource cts = CancellationTokenSource.CreateLinkedTokenSource(ct);
        cts.CancelAfter(TimeSpan.FromSeconds(30));
        try
        {
            _requests.OnNext(new ListRequest(request, tcs));
            TResponse response = await tcs.Task.WaitAsync(cts.Token);
            await SendAsync(response, response.IsSuccess ? 200 : 400, ct);
        }
        catch (OperationCanceledException)
        {
            await SendAsync(CreateTimeoutResponse(), 408, ct);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error processing request");
            await SendAsync(CreateErrorResponse(), 500, ct);
        }
    }
    protected virtual ISpecification<TEntity, TDto> CreateSpecification(TRequest request)
    {
        return new OptimizedSpec(request);
    }
    protected virtual ISpecification<TEntity, TDto> CreateDefaultSpecification(TRequest request)
    {
        return new DefaultFilterSpecification<TEntity, TDto>(
            request.PageNumber,
            request.PageSize,
            request.Filters ?? ImmutableDictionary<string, string>.Empty,
            request.Sorting ?? ImmutableList<Sort>.Empty);
    }
    protected virtual Task<int> GetTotalCountAsync(ISpecification<TEntity, TDto> spec)
    {
        return _repository.CountAsync(spec);
    }
    private IObservable<Unit> ProcessRequestBatch(IList<ListRequest> requests)
    {
        return Observable
            .FromAsync(
                async () =>
                {
                    foreach (ListRequest req in requests)
                    {
                        Stopwatch stopwatch = Stopwatch.StartNew();
                        try
                        {
                            ISpecification<TEntity, TDto> spec = CreateSpecification(req.Request);
                            List<TDto> items = await _repository.ListAsync(spec);
                            int count = await GetTotalCountAsync(spec);
                            req.Completion.TrySetResult(
                                new TResponse
                                {
                                    Items = items.ToImmutableList(),
                                    Count = count,
                                    IsSuccess = true,
                                    OperationElapsedTime = stopwatch.Elapsed.ToString(),
                                    OperationTypeNameId = GetType().Name
                                });
                        }
                        catch (Exception ex)
                        {
                            _logger.LogError(ex, $"Error processing request {req.Request.GetHashCode()}");
                            req.Completion.TrySetResult(CreateErrorResponse(ex, stopwatch));
                        }
                    }
                    return Unit.Default;
                })
            .SubscribeOn(_scheduler);
    }
    private TResponse CreateTimeoutResponse()
    {
        return new TResponse
        {
            IsSuccess = false,
            ErrorMessage = "Request timed out",
            OperationElapsedTime = "30s",
            OperationTypeNameId = GetType().Name
        };
    }
    private TResponse CreateErrorResponse(Exception ex = null, Stopwatch stopwatch = null)
    {
        return new TResponse
        {
            IsSuccess = false,
            ErrorMessage = ex?.Message ?? "Internal server error",
            OperationElapsedTime = stopwatch?.Elapsed.ToString() ?? "0s",
            OperationTypeNameId = GetType().Name
        };
    }
    private record ListRequest(TRequest Request, TaskCompletionSource<TResponse> Completion);
    private class OptimizedSpec : Specification<TEntity, TDto>
    {
        private static readonly ConcurrentDictionary<string, Expression<Func<TEntity, bool>>> FilterCache = new();
        private static readonly ConcurrentDictionary<string, Expression<Func<TEntity, object>>> SortCache = new();
        private static readonly ConcurrentDictionary<string, PropertyInfo> PropertyCache = new();
        public OptimizedSpec(FilterRequest request)
        {
             Guard.Against.Negative(request.PageNumber, nameof(request.PageNumber));
             Guard.Against.Negative(request.PageSize, nameof(request.PageSize));
            Query.Where(x => !EF.Property<bool>(x, "IsDeleted"));
            request.SearchTerm.Match(
                term => Query.Where(BuildSearchPredicate(term)),
                () => Query.OrderByDescending(x => EF.Property<DateTime>(x, "CreatedDate")));
            if (request.Filters?.IsEmpty == false)
            {
                ApplyFilters(request.Filters);
            }
            if (request.Sorting?.IsEmpty == false)
            {
                ApplySorting(request.Sorting);
            }
            else
            {
                Query.OrderByDescending(x => EF.Property<DateTime>(x, "CreatedDate"));
            }
            Query.Skip((request.PageNumber - 1) * request.PageSize).Take(request.PageSize).AsNoTracking();
        }
        private Expression<Func<TEntity, bool>> BuildSearchPredicate(string searchTerm)
        {
            string lower = searchTerm.ToLowerInvariant();
            return x => true;
        }
        private void ApplyFilters(ImmutableDictionary<string, string> filters)
        {
            foreach ((string key, string value) in filters)
            {
                if (!string.IsNullOrEmpty(value))
                {
                    PropertyInfo? property = GetCachedProperty(key);
                    if (property != null)
                    {
                        Expression<Func<TEntity, bool>> pred = BuildFilterPredicate(property, value);
                        Query.Where(pred);
                    }
                }
            }
        }
        private void ApplySorting(ImmutableList<Sort> sorting)
        {
            Sort first = sorting.First();
            IOrderedSpecificationBuilder<TEntity> ordered = first.Direction == SortDirection.Descending
                ? Query.OrderByDescending(GetSortExpression(first.Field))
                : Query.OrderBy(GetSortExpression(first.Field));
            foreach (Sort sort in sorting.Skip(1))
            {
                ordered = sort.Direction == SortDirection.Descending
                    ? ordered.ThenByDescending(GetSortExpression(sort.Field))
                    : ordered.ThenBy(GetSortExpression(sort.Field));
            }
        }
        private static PropertyInfo GetCachedProperty(string fieldName)
        {
            return PropertyCache.GetOrAdd(
                $"{typeof(TEntity).Name}_{fieldName}",
                _ => typeof(TEntity).GetProperty(fieldName) ??
                     throw new ArgumentException($"Property {fieldName} not found on {typeof(TEntity).Name}"));
        }
        private Expression<Func<TEntity, bool>> BuildFilterPredicate(PropertyInfo property, string val)
        {
            return FilterCache.GetOrAdd(
                $"{typeof(TEntity).Name}_{property.Name}_{val}",
                _ => CreateFilterExpression(property, val));
        }
        private Expression<Func<TEntity, object>> GetSortExpression(string fieldName)
        {
            return SortCache.GetOrAdd($"{typeof(TEntity).Name}_{fieldName}", _ => CreateSortExpression(fieldName));
        }
        private static Expression<Func<TEntity, bool>> CreateFilterExpression(PropertyInfo property, string val)
        {
            ParameterExpression param = Expression.Parameter(typeof(TEntity), "x");
            MemberExpression prop = Expression.Property(param, property);
            Expression body;
            if (property.PropertyType == typeof(string))
            {
                body = Expression.Call(prop, "Contains", null, Expression.Constant(val, typeof(string)));
            }
            else if (property.PropertyType == typeof(DateTime))
            {
                body = Expression.Equal(prop, Expression.Constant(DateTime.Parse(val)));
            }
            else if (property.PropertyType.IsEnum)
            {
                object enumVal = Enum.Parse(property.PropertyType, val);
                body = Expression.Equal(prop, Expression.Constant(enumVal));
            }
            else
            {
                object convertedVal = Convert.ChangeType(val, property.PropertyType);
                body = Expression.Equal(prop, Expression.Constant(convertedVal));
            }
            return Expression.Lambda<Func<TEntity, bool>>(body, param);
        }
        private static Expression<Func<TEntity, object>> CreateSortExpression(string fieldName)
        {
            ParameterExpression param = Expression.Parameter(typeof(TEntity), "x");
            MemberExpression prop = Expression.Property(param, GetCachedProperty(fieldName));
            UnaryExpression conv = Expression.Convert(prop, typeof(object));
            return Expression.Lambda<Func<TEntity, object>>(conv, param);
        }
    }
}
public abstract class RichEndpointBase<TRequest, TResponse>
    where TRequest : BaseRequest
    where TResponse : BaseResponse, new()
{
    private readonly Subject<DomainEvent> _domainEvents = new();
    private readonly IEventStore _eventStore;
    private readonly IAppLoggerService<TRequest> _logger;
    private readonly IMediator _mediator;
    private readonly IValidator<TRequest>? _validator;
    protected RichEndpointBase(
        IMediator mediator,
        IAppLoggerService<TRequest> logger,
        IEventStore eventStore,
        IValidator<TRequest>? validator = null)
    {
        _mediator = mediator;
        _logger = logger;
        _eventStore = eventStore;
        _validator = validator;
        _domainEvents.Buffer(TimeSpan.FromMilliseconds(50)).Where(events => events.Any())
            .Select(events => PersistAndPublishEvents(events)).Concat().Subscribe();
    }
    protected TRequest? CurrentRequest { get; private set; }
    protected TryAsync<TResponse> ExecuteOperation<T>(
        TRequest request,
        Func<TryAsync<T>> operation,
        Func<T, TResponse> mapper)
    {
        CurrentRequest = request;
        return from validation in ValidateRequest(request)
            from result in operation()
            from _ in PublishEvents()
            select mapper(result);
    }
    protected async Task RaiseDomainEvent(DomainEvent evt)
    {
        IStream stream = _eventStore.GetStream("DomainEvents", evt.Id);
        EventMetadata metadata = new EventMetadata
        {
            Timestamp = DateTime.UtcNow, EventType = evt.GetType().Name, StreamId = evt.Id
        };
        await stream.Append(evt as IRecord, metadata as IMetadata);
        _domainEvents.OnNext(evt);
    }
    private record EventMetadata : IMetadata
    {
        public DateTime Timestamp { get; init; }
        public string EventType { get; init; }
        public Guid StreamId { get; init; }
        public ILink Link(string destinationCategory)
        {
            throw new NotImplementedException();
        }
        public IMetadataSchema Schema => throw new NotImplementedException();
        public object this[MetadataProperty property]
        {
            get => throw new NotImplementedException();
            set => throw new NotImplementedException();
        }
    }
    private IObservable<Unit> PersistAndPublishEvents(IList<DomainEvent> events)
    {
        return Observable.FromAsync(
            async () =>
            {
                try
                {
                    foreach (DomainEvent evt in events)
                    {
                        await _mediator.Publish(evt);
                    }
                    return Unit.Default;
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex, "Failed to publish events");
                    throw;
                }
            });
    }
    private TryAsync<Unit> PublishEvents()
    {
        return TryAsync(
            async () =>
            {
                await Task.Delay(50);
                return Unit.Default;
            });
    }
    private TryAsync<Unit> ValidateRequest(TRequest request)
    {
        return TryAsync(
            () =>
            {
                if (_validator == null)
                {
                    return Task.FromResult(Unit.Default);
                }
                FluentValidation.ValidationContext<TRequest> context =
                    new FluentValidation.ValidationContext<TRequest>(request);
                var result = _validator.Validate(context);
                return result.IsValid ? Task.FromResult(Unit.Default) : throw new ValidationException(result.Errors);
            });
    }
}
public class ReactiveEventProcessor<TEvent>
    where TEvent : DomainEvent
{
    private readonly Subject<TEvent> _events = new();
    private readonly IScheduler _scheduler;
    public ReactiveEventProcessor(IMediator mediator, TimeSpan? bufferWindow = null, IScheduler scheduler = null)
    {
        _scheduler = scheduler ?? TaskPoolScheduler.Default;
        _events.Buffer(bufferWindow ?? TimeSpan.FromMilliseconds(100)).Where(events => events.Any())
            .Select(batch => ProcessBatch(mediator, batch)).Concat().Subscribe();
    }
    public void Raise(TEvent evt)
    {
        _events.OnNext(evt);
    }
    private IObservable<Unit> ProcessBatch(IMediator mediator, IList<TEvent> batch)
    {
        return Observable.FromAsync(() => Task.WhenAll(batch.Select(evt => mediator.Publish(evt))))
            .Select(_ => Unit.Default).SubscribeOn(_scheduler);
    }
}