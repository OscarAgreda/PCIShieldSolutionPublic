using System;
using System.Reactive.Linq;
using System.Reactive.Subjects;
using System.Threading.Tasks;
using Microsoft.Extensions.Caching.Memory;
using Polly;
using PCIShield.Infrastructure.Services;
using Serilog;
namespace PCIShield.Infrastructure.TurnServices
{
    public interface ITurnClient
    {
        Task<TurnSession> CreateSessionAsync(string username, string password);
        Task RefreshSessionAsync(TurnSession session);
        Task DeleteSessionAsync(TurnSession session);
    }
    public class TurnSession
    {
        public string SessionId { get; private set; }
        public DateTime ExpiryTime { get; private set; }
        public string Username { get; private set; }
        public string Password { get; private set; }
        public TurnSession(string sessionId, DateTime expiryTime, string username, string password)
        {
            SessionId = sessionId;
            ExpiryTime = expiryTime;
            Username = username;
            Password = password;
        }
        public bool IsExpired() => DateTime.UtcNow >= ExpiryTime;
    }
    public interface ITurnService
    {
        IObservable<TurnSession> TurnSessionObservable { get; }
        Task CheckAndHandleSessionExpiry(TurnSession session);
        Task<TurnSession> CreateTurnSessionAsync(string username, string password);
        Task DeleteTurnSessionAsync(TurnSession session);
        Task RefreshTurnSessionAsync(TurnSession session);
        Task RenewSessionIfNeeded(TurnSession session);
    }
    public class TurnService : ITurnService
    {
        private readonly ITurnClient _turnClient;
        private readonly IAppLoggerService<TurnService> _logger;
        private readonly BehaviorSubject<TurnSession> _turnSessionSubject = new BehaviorSubject<TurnSession>(null);
        private readonly MemoryCache _sessionCache = new MemoryCache(new MemoryCacheOptions());
        public IObservable<TurnSession> TurnSessionObservable => _turnSessionSubject.AsObservable();
        public async Task<TurnSession> CreateTurnSessionAsync(string username, string password)
        {
            var retryPolicy = Policy
                .Handle<Exception>()
                .WaitAndRetryAsync(retryCount: 2, sleepDurationProvider: _ => TimeSpan.FromSeconds(2));
            return await retryPolicy.ExecuteAsync(async () =>
            {
                try
                {
                    var session = await _turnClient.CreateSessionAsync(username, password);
                    _logger.LogInformation($"TURN session created for {username} with Session ID: {session.SessionId}");
                    CacheTurnSession(session);
                    _turnSessionSubject.OnNext(session);
                    return session;
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex, $"Error creating TURN session for {username}");
                    throw new TurnServiceException("Failed to create TURN session", ex);
                }
            });
        }
        public async Task CheckAndHandleSessionExpiry(TurnSession session)
        {
            if (session.IsExpired())
            {
                _logger.LogInformation($"TURN session {session.SessionId} for {session.Username} is expired. Deleting session.");
                await DeleteTurnSessionAsync(session);
            }
        }
        public async Task RefreshTurnSessionAsync(TurnSession session)
        {
            try
            {
                await _turnClient.RefreshSessionAsync(session);
                _logger.LogInformation($"TURN session {session.SessionId} refreshed");
                UpdateCachedTurnSession(session);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Error refreshing TURN session {session.SessionId}");
                throw;
            }
        }
        public async Task DeleteTurnSessionAsync(TurnSession session)
        {
            try
            {
                await _turnClient.DeleteSessionAsync(session);
                _logger.LogInformation($"TURN session {session.SessionId} deleted");
                RemoveCachedTurnSession(session);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Error deleting TURN session {session.SessionId}");
                throw;
            }
        }
        private void CacheTurnSession(TurnSession session)
        {
            _sessionCache.Set(session.SessionId, session, session.ExpiryTime);
        }
        private void UpdateCachedTurnSession(TurnSession session)
        {
            if (_sessionCache.TryGetValue(session.SessionId, out TurnSession cachedSession))
            {
                cachedSession = session;
                _sessionCache.Set(session.SessionId, cachedSession, session.ExpiryTime);
            }
        }
        private void RemoveCachedTurnSession(TurnSession session)
        {
            _sessionCache.Remove(session.SessionId);
        }
        public async Task RenewSessionIfNeeded(TurnSession session)
        {
            if (DateTime.UtcNow.AddMinutes(5) >= session.ExpiryTime)
            {
                _logger.LogInformation($"Renewing TURN session {session.SessionId} for {session.Username}");
                await RefreshTurnSessionAsync(session);
            }
        }
    }
    public class TurnServiceException : Exception
    {
        public TurnServiceException(string message, Exception innerException)
            : base(message, innerException)
        {
        }
    }
}
