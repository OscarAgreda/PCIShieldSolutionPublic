using System.Collections.Concurrent;
using System.Reactive.Subjects;
using PCIShield.BlazorAdmin.Client.SignalR;
using PCIShield.Domain.ModelsDto;
using PCIShieldLib.SharedKernel;
using Microsoft.AspNetCore.SignalR;

namespace PCIShield.BlazorAdmin.Hubs
{
    public class UserConnection
    {
        public string ConnectionId { get; set; }
        public DateTime ConnectedAt { get; set; }
        public string LastOperation { get; set; }
        public DateTime? LastOperationTime { get; set; }
    }
    public class GenericEventHub : Hub
    {
        private static readonly ConcurrentDictionary<string, UserConnection> _connections = new();
        private static readonly Subject<GenericSignalREvent> _eventStream = new();
        private readonly ILogger<GenericEventHub> _logger;
        private readonly ThrottleManager _throttle;
        public GenericEventHub(ILogger<GenericEventHub> logger)
        {
            _logger = logger;
            _throttle = new ThrottleManager(TimeSpan.FromMilliseconds(500));
        }
        public async Task BroadcastEvent(GenericSignalREvent evt)
        {
            await _throttle.WaitAsync(GetEventKey(evt));
            try
            {
                evt = evt with
                {
                    Timestamp = DateTime.UtcNow,
                    Version = evt.Version + 1
                };
                _eventStream.OnNext(evt);
                await Clients.Others.SendAsync("ReceiveGenericEvent", evt);
                _logger.LogDebug("Event broadcast: {Type} {Id} {Op} by {User}",
                    evt.AggregateName, evt.EntityId, evt.Operation, evt.UserId);
            }
            finally
            {
                _throttle.Release(GetEventKey(evt));
            }
        }
        private static string GetEventKey(GenericSignalREvent evt) =>
            $"{evt.AggregateName}_{evt.EntityId}_{evt.Operation}";
        private class ThrottleManager
        {
            private readonly ConcurrentDictionary<string, SemaphoreSlim> _throttles = new();
            private readonly TimeSpan _minInterval;
            public ThrottleManager(TimeSpan minInterval) => _minInterval = minInterval;
            public async Task WaitAsync(string key)
            {
                var throttle = _throttles.GetOrAdd(key, _ => new SemaphoreSlim(1, 1));
                await throttle.WaitAsync();
                await Task.Delay(_minInterval);
            }
            public void Release(string key)
            {
                if (_throttles.TryGetValue(key, out var throttle))
                {
                    throttle.Release();
                }
            }
        }
    }
}
