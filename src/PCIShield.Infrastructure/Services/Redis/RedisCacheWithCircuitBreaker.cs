using NRedisStack;
using StackExchange.Redis;
using System.Text.Json.Serialization;
using System.Text.Json;
using System.Threading.Tasks;
using System;
using System.Collections.Generic;
using System.Reactive.Linq;
using NRedisStack.RedisStackCommands;
using Microsoft.Extensions.Caching.Distributed;
using System.Threading;
using Ardalis.Specification;
using PCIShieldLib.SharedKernel.Interfaces;
using Microsoft.Extensions.Logging;
using System.Linq;
namespace PCIShield.Infrastructure.Services.Redis
{
    public class RedisCacheWithCircuitBreaker<T> : IReadRedisRepository<T> where T : class, IAggregateRoot
    {
        private readonly RedisCacheRepository<T> _innerRepository;
        private readonly ICircuitBreaker _circuitBreaker;
        private readonly ILogger<RedisCacheWithCircuitBreaker<T>> _logger;
        private readonly IRedisHealthCheck _healthCheck;
        private readonly ICacheStrategy _cacheStrategy;
        private readonly IRedisCacheService _cache;
        private readonly ICacheKeyListService _cacheKeyListService;
        public RedisCacheWithCircuitBreaker(
            RedisCacheRepository<T> innerRepository,
            ICircuitBreaker circuitBreaker,
            ILogger<RedisCacheWithCircuitBreaker<T>> logger,
            IRedisHealthCheck healthCheck,
            ICacheStrategy cacheStrategy,
            IRedisCacheService cache,
            ICacheKeyListService cacheKeyListService)
        {
            _innerRepository = innerRepository;
            _circuitBreaker = circuitBreaker;
            _logger = logger;
            _healthCheck = healthCheck;
            _cacheStrategy = cacheStrategy;
            _cache = cache;
            _cacheKeyListService = cacheKeyListService;
        }
        public Task<PagedResult<T>> GetPagedResultAsync(
            PagedSpecification<T> specification,
            int pageNumber,
            int pageSize,
            CancellationToken cancellationToken = default)
        {
            throw new NotImplementedException();
        }
        public Task<PagedResult<TResult>> GetPagedResultAsync<TResult>(
            PagedSpecification<T, TResult> specification,
            int pageNumber,
            int pageSize,
            CancellationToken cancellationToken = default)
        {
            throw new NotImplementedException();
        }
        public Task InvalidateEntityCacheAsync<TEntity>(Guid? entityId = null, string? customSpecificationName = null,
            int? pageSize = null, int? pageNumber = null, Type? projectionType = null) where TEntity : class, IAggregateRoot
        {
            throw new NotImplementedException();
        }
        public async Task<T?> FirstOrDefaultAsync(ISpecification<T> specification, CancellationToken cancellationToken = default)
        {
            return await _circuitBreaker.ExecuteAsync(async () =>
            {
                if (!specification.CacheEnabled || !await _healthCheck.IsRedisHealthy())
                    return await _innerRepository.FirstOrDefaultAsync(specification, cancellationToken);
                var key = _cacheStrategy.GenerateKey(specification, "default");
                var cachedResult = await _cache.GetAsync<T>(key);
                if (cachedResult != null)
                {
                    _logger.LogInformation("Cache hit for key: {Key}", key);
                    return cachedResult;
                }
                var result = await _innerRepository.FirstOrDefaultAsync(specification, cancellationToken);
                if (result != null)
                {
                    var expiry = _cacheStrategy.GetExpirationTime(result);
                    await _cache.SetAsync(key, result, expiry);
                    _cacheKeyListService.AddKey(key);
                }
                return result;
            });
        }
        public async Task<T?> FirstOrDefaultAsyncFromOtherMicroservice(
            ISpecification<T> specification,
            string pciShieldappAppSuperUserId,
            CancellationToken cancellationToken = default)
        {
            return await _circuitBreaker.ExecuteAsync(async () =>
            {
                if (!specification.CacheEnabled || !await _healthCheck.IsRedisHealthy())
                    return await _innerRepository.FirstOrDefaultAsync(specification, cancellationToken);
                var key = _cacheStrategy.GenerateKey(specification, pciShieldappAppSuperUserId);
                var cachedResult = await _cache.GetAsync<T>(key);
                if (cachedResult != null)
                {
                    _logger.LogInformation("Cache hit from microservice for key: {Key}", key);
                    return cachedResult;
                }
                var result = await _innerRepository.FirstOrDefaultAsync(specification, cancellationToken);
                if (result != null)
                {
                    var expiry = _cacheStrategy.GetExpirationTime(result);
                    await _cache.SetAsync(key, result, expiry);
                    _cacheKeyListService.AddKey(key);
                }
                return result;
            });
        }
        public async Task<List<T>> ListAsyncFromOtherMicroservice(
            ISpecification<T> specification,
            string pciShieldappAppSuperUserId,
            CancellationToken cancellationToken = default)
        {
            return await _circuitBreaker.ExecuteAsync(async () =>
            {
                if (!specification.CacheEnabled || !await _healthCheck.IsRedisHealthy())
                    return await _innerRepository.ListAsync(specification, cancellationToken);
                var key = _cacheStrategy.GenerateKey(specification, pciShieldappAppSuperUserId);
                var cachedResult = await _cache.GetAsync<List<T>>(key);
                if (cachedResult != null)
                {
                    _logger.LogInformation("Cache hit from microservice for list with key: {Key}", key);
                    return cachedResult;
                }
                var result = await _innerRepository.ListAsync(specification, cancellationToken);
                if (result?.Any() == true)
                {
                    var expiry = _cacheStrategy.GetExpirationTime(result.First());
                    await _cache.SetAsync(key, result, expiry);
                    _cacheKeyListService.AddKey(key);
                }
                return result ?? new List<T>();
            });
        }
        public async Task<List<T>> ListAsyncWithCustomExpiration(ISpecification<T> specification, TimeSpan? absoluteExpiration,
            CancellationToken cancellationToken = default)
            => await _circuitBreaker.ExecuteAsync(async () =>
                await _innerRepository.ListAsyncWithCustomExpiration(specification, absoluteExpiration, cancellationToken));
        public async Task<T?> GetByIdAsync<TId>(TId id, CancellationToken cancellationToken = default) where TId : notnull
            => await _circuitBreaker.ExecuteAsync(async () =>
                await _innerRepository.GetByIdAsync(id, cancellationToken));
        public async Task<T?> GetBySpecAsync(ISpecification<T> specification, CancellationToken cancellationToken = default)
            => await _circuitBreaker.ExecuteAsync(async () =>
                await _innerRepository.GetBySpecAsync(specification, cancellationToken));
        public async Task<TResult?> GetBySpecAsync<TResult>(ISpecification<T, TResult> specification, CancellationToken cancellationToken = default)
            => await _circuitBreaker.ExecuteAsync(async () =>
                await _innerRepository.GetBySpecAsync(specification, cancellationToken));
        public async Task<TResult?> FirstOrDefaultAsync<TResult>(ISpecification<T, TResult> specification, CancellationToken cancellationToken = default)
            => await _circuitBreaker.ExecuteAsync(async () =>
                await _innerRepository.FirstOrDefaultAsync(specification, cancellationToken));
        public async Task<T?> SingleOrDefaultAsync(ISingleResultSpecification<T> specification, CancellationToken cancellationToken = default)
            => await _circuitBreaker.ExecuteAsync(async () =>
                await _innerRepository.SingleOrDefaultAsync(specification, cancellationToken));
        public async Task<TResult?> SingleOrDefaultAsync<TResult>(ISingleResultSpecification<T, TResult> specification, CancellationToken cancellationToken = default)
            => await _circuitBreaker.ExecuteAsync(async () =>
                await _innerRepository.SingleOrDefaultAsync(specification, cancellationToken));
        public async Task<List<T>> ListAsync(CancellationToken cancellationToken = default)
            => await _circuitBreaker.ExecuteAsync(async () =>
                await _innerRepository.ListAsync(cancellationToken));
        public async Task<List<T>> ListAsync(ISpecification<T> specification, CancellationToken cancellationToken = default)
        {
            return await _circuitBreaker.ExecuteAsync(async () =>
            {
                if (!specification.CacheEnabled || !await _healthCheck.IsRedisHealthy())
                    return await _innerRepository.ListAsync(specification, cancellationToken);
                var key = _cacheStrategy.GenerateKey(specification, "default");
                var cachedResult = await _cache.GetAsync<List<T>>(key);
                if (cachedResult != null)
                {
                    _logger.LogInformation("Cache hit for list with key: {Key}", key);
                    return cachedResult;
                }
                var result = await _innerRepository.ListAsync(specification, cancellationToken);
                if (result?.Any() == true)
                {
                    var expiry = _cacheStrategy.GetExpirationTime(result.First());
                    await _cache.SetAsync(key, result, expiry);
                    _cacheKeyListService.AddKey(key);
                }
                return result ?? new List<T>();
            });
        }
        public async Task<List<TResult>> ListAsync<TResult>(ISpecification<T, TResult> specification, CancellationToken cancellationToken = default)
            => await _circuitBreaker.ExecuteAsync(async () =>
                await _innerRepository.ListAsync(specification, cancellationToken));
        public async Task<int> CountAsync(ISpecification<T> specification, CancellationToken cancellationToken = default)
            => await _circuitBreaker.ExecuteAsync(async () =>
                await _innerRepository.CountAsync(specification, cancellationToken));
        public async Task<int> CountAsync(CancellationToken cancellationToken = default)
            => await _circuitBreaker.ExecuteAsync(async () =>
                await _innerRepository.CountAsync(cancellationToken));
        public async Task<bool> AnyAsync(ISpecification<T> specification, CancellationToken cancellationToken = default)
            => await _circuitBreaker.ExecuteAsync(async () =>
                await _innerRepository.AnyAsync(specification, cancellationToken));
        public async Task<bool> AnyAsync(CancellationToken cancellationToken = default)
            => await _circuitBreaker.ExecuteAsync(async () =>
                await _innerRepository.AnyAsync(cancellationToken));
        public IAsyncEnumerable<T> AsAsyncEnumerable(ISpecification<T> specification)
            => _innerRepository.AsAsyncEnumerable(specification);
}
    public interface ICircuitBreaker
    {
        Task<T> ExecuteAsync<T>(Func<Task<T>> action);
    }
    public class CircuitBreaker : ICircuitBreaker
    {
        private readonly SemaphoreSlim _semaphore = new(1, 1);
        private int _failureCount;
        private DateTime _lastFailure;
        private readonly int _threshold;
        private readonly TimeSpan _resetTimeout;
        public CircuitBreaker(int threshold = 5, int resetTimeoutSeconds = 30)
        {
            _threshold = threshold;
            _resetTimeout = TimeSpan.FromSeconds(resetTimeoutSeconds);
        }
        public async Task<T> ExecuteAsync<T>(Func<Task<T>> action)
        {
            await _semaphore.WaitAsync();
            try
            {
                if (_failureCount >= _threshold &&
                    DateTime.UtcNow - _lastFailure < _resetTimeout)
                    throw new CircuitBreakerOpenException();
                var result = await action();
                Reset();
                return result;
            }
            catch (Exception)
            {
                IncrementFailures();
                throw;
            }
            finally
            {
                _semaphore.Release();
            }
        }
        private void Reset() => _failureCount = 0;
        private void IncrementFailures()
        {
            _failureCount++;
            _lastFailure = DateTime.UtcNow;
        }
    }
    public interface IFrequentlyUpdated
    {
    }
    public interface IReadOnly
    {
    }
    public class CircuitBreakerOpenException : Exception
    {
        public CircuitBreakerOpenException()
            : base("Circuit breaker is open due to too many recent failures") { }
    }
    public interface ICacheStrategy
    {
        TimeSpan GetExpirationTime<T>(T item) where T : class, IAggregateRoot;
        string GenerateKey<T>(ISpecification<T> specification, string userId) where T : class, IAggregateRoot;
    }
    public class DynamicCacheStrategy : ICacheStrategy
    {
        public TimeSpan GetExpirationTime<T>(T item) where T : class, IAggregateRoot
        {
            return item switch
            {
                IFrequentlyUpdated => TimeSpan.FromMinutes(5),
                IReadOnly => TimeSpan.FromHours(24),
                _ => TimeSpan.FromMinutes(30)
            };
        }
        public string GenerateKey<T>(ISpecification<T> specification, string userId) where T : class, IAggregateRoot
            => $"{typeof(T).Name}-{specification.CacheKey}-{userId}-{DateTime.UtcNow.Date}";
    }
    public static class RedisRepositoryExtensions
    {
        public static IObservable<T?> ObserveFirstOrDefault<T>(
            IReadRedisRepository<T> repository,
            ISpecification<T> specification) where T : class, IAggregateRoot
        {
            return Observable.FromAsync(ct =>
                    repository.FirstOrDefaultAsync(specification, ct))
                .Retry(3)
                .Timeout(TimeSpan.FromSeconds(5))
                .Catch<T?, TimeoutException>(ex =>
                    Observable.Return<T?>(default));
        }
        public static IObservable<List<T>> ObserveList<T>(
            IReadRedisRepository<T> repository,
            ISpecification<T> specification) where T : class, IAggregateRoot
        {
            return Observable.FromAsync(ct =>
                    repository.ListAsync(specification, ct))
                .Select(list => list ?? new List<T>())
                .Catch<List<T>, Exception>(ex =>
                    Observable.Return(new List<T>()));
        }
    }
    public interface IRedisHealthCheck
    {
        Task<bool> IsRedisHealthy();
    }
    public interface IObservableRedisRepository<T> where T : class, IAggregateRoot
    {
        IObservable<T?> WatchFirstOrDefault(ISpecification<T> specification);
        IObservable<List<T>> WatchList(ISpecification<T> specification);
    }
    public class ObservableRedisRepository<T> : IObservableRedisRepository<T> where T : class, IAggregateRoot
    {
        private readonly IReadRedisRepository<T> _repository;
        private readonly ILogger<ObservableRedisRepository<T>> _logger;
        public ObservableRedisRepository(
            IReadRedisRepository<T> repository,
            ILogger<ObservableRedisRepository<T>> logger)
        {
            _repository = repository;
            _logger = logger;
        }
        public IObservable<T?> WatchFirstOrDefault(ISpecification<T> specification)
        {
            return Observable.Create<T?>(async (observer, cancellationToken) =>
            {
                try
                {
                    var result = await _repository.FirstOrDefaultAsync(specification, cancellationToken);
                    observer.OnNext(result);
                    observer.OnCompleted();
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex, "Error watching FirstOrDefault for {EntityType}", typeof(T).Name);
                    observer.OnError(ex);
                }
            });
        }
        public IObservable<List<T>> WatchList(ISpecification<T> specification)
        {
            return Observable.Create<List<T>>(async (observer, cancellationToken) =>
            {
                try
                {
                    var result = await _repository.ListAsync(specification, cancellationToken);
                    observer.OnNext(result);
                    observer.OnCompleted();
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex, "Error watching List for {EntityType}", typeof(T).Name);
                    observer.OnError(ex);
                }
            });
        }
    }
    public class RedisHealthCheck : IRedisHealthCheck
    {
        private readonly IConnectionMultiplexer _connectionMultiplexer;
        private readonly ILogger<RedisHealthCheck> _logger;
        public RedisHealthCheck(
            IConnectionMultiplexer connectionMultiplexer,
            ILogger<RedisHealthCheck> logger)
        {
            _connectionMultiplexer = connectionMultiplexer;
            _logger = logger;
        }
        public async Task<bool> IsRedisHealthy()
        {
            try
            {
                var db = _connectionMultiplexer.GetDatabase();
                var result = await db.PingAsync();
                return result < TimeSpan.FromSeconds(1);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Redis health check failed");
                return false;
            }
        }
    }
}
