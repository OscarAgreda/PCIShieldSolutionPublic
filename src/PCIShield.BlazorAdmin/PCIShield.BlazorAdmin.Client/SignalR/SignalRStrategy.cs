using System.Collections.Concurrent;
using System.Reactive.Linq;
using System.Reactive.Subjects;
using LanguageExt;
using LanguageExt.Common;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.SignalR.Client;
namespace PCIShield.BlazorAdmin.Client.SignalR;
public static class SignalROperations
{
    public const string BeingEdited = "BeingEdited";
    public const string EditingFinished = "EditingFinished";
    public const string Create = "Create";
    public const string Update = "Update";
    public const string Delete = "Delete";
    public const string Approval = "Approval";
}
public record GenericSignalREvent : BaseSignalREventMetadata
{
    public Dictionary<string, object> Parameters { get; set; } = new();
    public GenericSignalREvent(
        string aggregateName,
        Guid entityId,
        string operation,
        string userId,
        int version = 0,
        Guid? parentId = null)
    {
        AggregateName = aggregateName;
        EntityId = entityId;
        Operation = operation;
        UserId = userId;
        Version = version;
        ParentId = parentId;
    }
}
public abstract record BaseSignalREventMetadata
{
    public string AggregateName { get; init; }
    public string JsonPayload { get; init; }
    public Guid EntityId { get; init; }
    public Guid? ParentId { get; init; }
    public string Operation { get; init; }
    public DateTime Timestamp { get; init; } = DateTime.UtcNow;
    public string UserId { get; init; }
    public int Version { get; init; }
}
public class EnhancedSignalRStrategy : IAsyncDisposable, ISignalRNotificationStrategy
{
    private readonly SemaphoreSlim _connectionLock = new(1, 1);
    private readonly Subject<BaseSignalREventMetadata> _eventSubject = new();
    private readonly ILogger _logger;
    private readonly NavigationManager _navigationManager;
    private readonly ConcurrentDictionary<string, SemaphoreSlim> _operationLocks = new();
    private readonly SignalROptions _options;
    private HubConnection? _hubConnection;
    private bool _isDisposed;
    public EnhancedSignalRStrategy(
        NavigationManager navigationManager,
        ILogger<EnhancedSignalRStrategy> logger,
        SignalROptions options)
    {
        _navigationManager = navigationManager;
        _logger = logger;
        _options = options;
    }
    public async ValueTask DisposeAsync()
    {
        if (_isDisposed)
            return;
        try
        {
            if (_hubConnection != null)
                await _hubConnection.DisposeAsync();
            _eventSubject.Dispose();
            foreach (var opLock in _operationLocks.Values)
                opLock.Dispose();
            _connectionLock.Dispose();
        }
        finally
        {
            _isDisposed = true;
        }
    }
    public async Task EnsureConnectedAsync(CancellationToken cancellationToken = default)
    {
        if (_hubConnection?.State == HubConnectionState.Connected)
            return;
        await _connectionLock.WaitAsync(cancellationToken);
        try
        {
            if (_hubConnection == null)
            {
                _hubConnection = new HubConnectionBuilder()
                    .WithUrl(_navigationManager.ToAbsoluteUri(_options.HubUrl))
                    .WithAutomaticReconnect()
                    .Build();
                InitializeEventHandlers();
            }
            if (_hubConnection.State == HubConnectionState.Disconnected)
            {
                await _hubConnection.StartAsync(cancellationToken);
                _logger.LogInformation("SignalR connection established");
            }
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Failed to establish SignalR connection");
            throw;
        }
        finally
        {
            _connectionLock.Release();
        }
    }
    public IObservable<GenericSignalREvent> GetEventStream()
        => _eventSubject.OfType<GenericSignalREvent>();
    public async Task ReconnectAsync(string connectionId)
    {
        await EnsureConnectedAsync();
    }
    public async Task<Either<Error, Unit>> SendNotificationAsync(GenericSignalREvent evt)
    {
        try
        {
            await EnsureConnectedAsync();
            string lockKey = $"{evt.AggregateName}_{evt.Operation}_{evt.EntityId}";
            var opLock = _operationLocks.GetOrAdd(lockKey, _ => new SemaphoreSlim(1, 1));
            await opLock.WaitAsync();
            try
            {
                await _hubConnection!.SendAsync("BroadcastEvent", evt);
                return Unit.Default;
            }
            finally
            {
                opLock.Release();
            }
        }
        catch (Exception ex)
        {
            return Error.New(ex);
        }
    }
    private void InitializeEventHandlers()
    {
        if (_hubConnection == null)
            return;
        _hubConnection.Reconnecting += error =>
        {
            _logger.LogWarning(error, "SignalR connection lost. Reconnecting...");
            return Task.CompletedTask;
        };
        _hubConnection.Reconnected += async connectionId =>
        {
            _logger.LogInformation("SignalR connection reestablished: {ConnectionId}", connectionId);
            await ReconnectAsync(connectionId);
        };
        _hubConnection.Closed += error =>
        {
            _logger.LogError(error, "SignalR connection closed unexpectedly");
            return Task.CompletedTask;
        };
        _hubConnection.On<GenericSignalREvent>("ReceiveGenericEvent", evt =>
        {
            _eventSubject.OnNext(evt);
            return Task.CompletedTask;
        });
    }
}
public class SignalROptions
{
    public string HubUrl { get; set; } = "/genericEventHub";
    public int MaxRetryAttempts { get; set; } = 3;
}
public interface ISignalRNotificationStrategy
{
    Task EnsureConnectedAsync(CancellationToken cancellationToken = default);
    IObservable<GenericSignalREvent> GetEventStream();
    Task<Either<Error, Unit>> SendNotificationAsync(GenericSignalREvent evt);
    Task ReconnectAsync(string connectionId);
    ValueTask DisposeAsync();
}
public static class SignalRServiceExtensions
{
    public static IServiceCollection AddSmartSignalR(
        this IServiceCollection services,
        Action<SignalROptions> configure)
    {
        var options = new SignalROptions();
        configure(options);
        services.AddSingleton(options);
        services.AddScoped<ISignalRNotificationStrategy, SmartSignalRStrategy>();
        return services;
    }
}
public class SmartSignalRStrategy : ISignalRNotificationStrategy
{
    private readonly Subject<ISignalRNotificationStrategy> _strategySubject = new();
    private readonly Subject<GenericSignalREvent> _eventSubject = new();
    private readonly ConcurrentDictionary<string, DateTime> _lastEventTimes = new();
    private readonly NavigationManager _navigationManager;
    private readonly ILogger _logger;
    private readonly SignalROptions _options;
    private volatile ISignalRNotificationStrategy _currentStrategy;
    public SmartSignalRStrategy(
        NavigationManager navigationManager,
        ILogger<EnhancedSignalRStrategy> logger,
        SignalROptions options)
    {
        _navigationManager = navigationManager;
        _logger = logger;
        _options = options;
        _currentStrategy = new NoOpSignalRStrategy();
        Observable.StartAsync(async () =>
        {
            await Task.Yield();
            _currentStrategy = new EnhancedSignalRStrategy(navigationManager, logger, options);
            _strategySubject.OnNext(_currentStrategy);
        }).Subscribe(onNext: _ =>
        {
        });
    }
    public Task EnsureConnectedAsync(CancellationToken ct = default) =>
        _currentStrategy.EnsureConnectedAsync(ct);
    public IObservable<GenericSignalREvent> GetEventStream() =>
        _strategySubject
            .StartWith(_currentStrategy)
            .SelectMany(strategy => strategy.GetEventStream())
            .Synchronize()
            .Where(evt => ShouldProcessEvent(evt))
            .Do(evt => _lastEventTimes[GetEventKey(evt)] = DateTime.UtcNow)
            .Publish()
            .RefCount();
    public async Task<Either<Error, Unit>> SendNotificationAsync(GenericSignalREvent evt)
    {
        if (!ShouldBroadcastEvent(evt))
        {
            return Unit.Default;
        }
        var result = await _currentStrategy.SendNotificationAsync(evt);
        result.IfRight(_ => _eventSubject.OnNext(evt));
        return result;
    }
    public Task ReconnectAsync(string connectionId) =>
        _currentStrategy.ReconnectAsync(connectionId);
    public async ValueTask DisposeAsync()
    {
        await _currentStrategy.DisposeAsync();
        _strategySubject.Dispose();
    }
    private bool ShouldProcessEvent(GenericSignalREvent evt)
    {
        var key = GetEventKey(evt);
        if (_lastEventTimes.TryGetValue(key, out var lastTime))
        {
            if (DateTime.UtcNow - lastTime < TimeSpan.FromMilliseconds(500))
            {
                _logger.LogDebug("Skipping duplicate event: {Key}", key);
                return false;
            }
        }
        return true;
    }
    private bool ShouldBroadcastEvent(GenericSignalREvent evt)
    {
        var key = GetEventKey(evt);
        if (_lastEventTimes.TryGetValue(key, out var lastTime))
        {
            if (DateTime.UtcNow - lastTime < TimeSpan.FromMilliseconds(500))
            {
                _logger.LogDebug("Throttling broadcast: {Key}", key);
                return false;
            }
        }
        return true;
    }
    private static string GetEventKey(GenericSignalREvent evt) =>
        $"{evt.AggregateName}_{evt.EntityId}_{evt.Operation}";
}
public class NoOpSignalRStrategy : ISignalRNotificationStrategy
{
    public IObservable<GenericSignalREvent> GetEventStream() =>
        Observable.Empty<GenericSignalREvent>();
    public Task EnsureConnectedAsync(CancellationToken ct = default) =>
        Task.CompletedTask;
    public Task<Either<Error, Unit>> SendNotificationAsync(GenericSignalREvent evt) =>
        Task.FromResult<Either<Error, Unit>>(Unit.Default);
    public Task ReconnectAsync(string connectionId) =>
        Task.CompletedTask;
    public ValueTask DisposeAsync() =>
        ValueTask.CompletedTask;
}