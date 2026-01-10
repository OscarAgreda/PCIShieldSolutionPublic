using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Ardalis.Specification;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Caching.Hybrid;
using PCIShield.Infrastructure.Services;
using PCIShieldLib.SharedKernel.Interfaces;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using System.Buffers;
using System.IO;
using System.Text.Json;
namespace PCIShield.Infrastructure.Data
{
#pragma warning disable EXTEXP0018
    public class DefaultHybridCacheSerializer<T> : IHybridCacheSerializer<T>
    {
        private readonly JsonSerializerOptions _options;
        public DefaultHybridCacheSerializer()
        {
            _options = new JsonSerializerOptions
            {
                PropertyNameCaseInsensitive = true,
                PropertyNamingPolicy = JsonNamingPolicy.CamelCase
            };
        }
        public T Deserialize(ReadOnlySequence<byte> source)
        {
            using var memoryStream = new MemoryStream();
            foreach (var segment in source)
            {
                memoryStream.Write(segment.Span);
            }
            memoryStream.Position = 0;
            return JsonSerializer.Deserialize<T>(memoryStream, _options)!;
        }
        public void Serialize(T value, IBufferWriter<byte> target)
        {
            var bytes = JsonSerializer.SerializeToUtf8Bytes(value, _options);
            target.Write(bytes);
        }
    }
    public class DefaultSerializerFactory : IHybridCacheSerializerFactory
    {
        public bool TryCreateSerializer<T>([System.Diagnostics.CodeAnalysis.NotNullWhen(true)] out IHybridCacheSerializer<T>? serializer)
        {
            if (typeof(T) == typeof(string) || typeof(T) == typeof(byte[]))
            {
                serializer = null;
                return false;
            }
            serializer = new DefaultHybridCacheSerializer<T>();
            return true;
        }
    }
    public static class HybridCacheExtensions
    {
        public static IServiceCollection AddCustomHybridCache(this IServiceCollection services)
        {
            services.AddHybridCache(options =>
            {
                options.MaximumPayloadBytes = 1024 * 1024;
                options.MaximumKeyLength = 1024;
                options.DefaultEntryOptions = new HybridCacheEntryOptions
                {
                    Expiration = TimeSpan.FromMinutes(30),
                    LocalCacheExpiration = TimeSpan.FromMinutes(5)
                };
            }).AddSerializerFactory<DefaultSerializerFactory>();
            return services;
        }
    }
    public class InMemoryCacheRepository<T> : IReadInMemoryRepository<T> where T : class, IAggregateRoot
    {
        private readonly HybridCache _cache;
        private readonly ICacheKeyListService _cacheKeyListService;
        private readonly HybridCacheEntryOptions _defaultCacheOptions;
        private readonly IKeyCloakTenantService _iKeyCloakTenantService;
        private readonly ILogger<InMemoryCacheRepository<T>> _logger;
        private readonly IAppLoggerService<InMemoryCacheRepository<T>> _seriLogger;
        private readonly IRepository<T> _sourceRepository;
        public InMemoryCacheRepository(
            HybridCache cache,
            ILogger<InMemoryCacheRepository<T>> logger,
            IRepository<T> sourceRepository,
            ICacheKeyListService cacheKeyListService,
            IKeyCloakTenantService iKeyCloakTenantService,
            IAppLoggerService<InMemoryCacheRepository<T>> seriLogger)
        {
            _cache = cache;
            _logger = logger;
            _sourceRepository = sourceRepository;
            _cacheKeyListService = cacheKeyListService;
            _iKeyCloakTenantService = iKeyCloakTenantService;
            _seriLogger = seriLogger;
            _defaultCacheOptions = new HybridCacheEntryOptions
            {
                Expiration = TimeSpan.FromSeconds(50),
                LocalCacheExpiration = TimeSpan.FromSeconds(40),
                Flags = HybridCacheEntryFlags.None
            };
        }
        private async ValueTask<TResult> GetOrCreateCacheAsync<TResult>(
            string key,
            Func<CancellationToken, ValueTask<TResult>> factory,
            HybridCacheEntryOptions? options = null,
            CancellationToken cancellationToken = default)
        {
            options ??= _defaultCacheOptions;
            var tags = new[] { typeof(T).Name, _iKeyCloakTenantService.GetPCIShieldSolutionuperUserId() };
            var result = await _cache.GetOrCreateAsync(
                key,
                async (token) => await factory(token),
                options,
                tags,
                cancellationToken);
            _cacheKeyListService.AddKey(key);
            return result;
        }
        public async Task<T?> GetByIdAsync<TId>(TId id, CancellationToken cancellationToken = default)
        {
            string? tenantId = _iKeyCloakTenantService.GetPCIShieldSolutionuperUserId();
            string key = $"{typeof(T).Name}-{tenantId}-{id}";
            return await GetOrCreateCacheAsync(
                key,
                async (token) => await _sourceRepository.GetByIdAsync(id, token),
                cancellationToken: cancellationToken);
        }
        public async Task<List<T>> ListAsync(ISpecification<T> specification, CancellationToken cancellationToken = default)
        {
            if (!specification.CacheEnabled)
                return await _sourceRepository.ListAsync(specification, cancellationToken);
            string pciShieldappAppSuperUserId = _iKeyCloakTenantService.GetPCIShieldSolutionuperUserId();
            string key = $"{specification.CacheKey}-{pciShieldappAppSuperUserId}-ListAsync";
            return await GetOrCreateCacheAsync(
                key,
                async (token) => await _sourceRepository.ListAsync(specification, token),
                cancellationToken: cancellationToken);
        }
        public async Task<List<TResult>> ListAsync<TResult>(
            ISpecification<T, TResult> specification,
            CancellationToken cancellationToken = default)
        {
            if (!specification.CacheEnabled)
                return await _sourceRepository.ListAsync(specification, cancellationToken);
            string tenantId = _iKeyCloakTenantService.GetPCIShieldSolutionuperUserId();
            string key = $"{specification.CacheKey}-{tenantId}-ListAsync";
            return await GetOrCreateCacheAsync(
                key,
                async (token) => await _sourceRepository.ListAsync(specification, token),
                cancellationToken: cancellationToken);
        }
        public async Task<TResult> FirstOrDefaultAsync<TResult>(
            ISpecification<T, TResult> specification,
            CancellationToken cancellationToken = default)
        {
            if (!specification.CacheEnabled)
                return await _sourceRepository.FirstOrDefaultAsync(specification, cancellationToken);
            string tenantId = _iKeyCloakTenantService.GetPCIShieldSolutionuperUserId();
            string key = $"{specification.CacheKey}-{tenantId}-FirstOrDefaultAsync";
            return await GetOrCreateCacheAsync(
                key,
                async (token) => await _sourceRepository.FirstOrDefaultAsync(specification, token),
                cancellationToken: cancellationToken);
        }
        public Task<T> AddAsync(T entity) => _sourceRepository.AddAsync(entity);
        public Task DeleteAsync(T entity) => _sourceRepository.DeleteAsync(entity);
        public Task DeleteRangeAsync(IEnumerable<T> entities) => _sourceRepository.DeleteRangeAsync(entities);
        public Task UpdateAsync(T entity) => _sourceRepository.UpdateAsync(entity);
        public Task SaveChangesAsync() => _sourceRepository.SaveChangesAsync();
        public async Task<T?> GetBySpecAsync(ISpecification<T> specification, CancellationToken cancellationToken = default)
        {
            if (!specification.CacheEnabled)
                return await _sourceRepository.GetBySpecAsync(specification, cancellationToken);
            string tenantId = _iKeyCloakTenantService.GetPCIShieldSolutionuperUserId();
            string key = $"{specification.CacheKey}-{tenantId}-GetBySpecAsync";
            return await GetOrCreateCacheAsync(
                key,
                async (token) => await _sourceRepository.GetBySpecAsync(specification, token),
                cancellationToken: cancellationToken);
        }
        public async Task<TResult?> GetBySpecAsync<TResult>(
            ISpecification<T, TResult> specification,
            CancellationToken cancellationToken = default)
        {
            if (!specification.CacheEnabled)
                return await _sourceRepository.GetBySpecAsync(specification, cancellationToken);
            string tenantId = _iKeyCloakTenantService.GetPCIShieldSolutionuperUserId();
            string key = $"{specification.CacheKey}-{tenantId}-GetBySpecAsync";
            return await GetOrCreateCacheAsync(
                key,
                async (token) => await _sourceRepository.GetBySpecAsync(specification, token),
                cancellationToken: cancellationToken);
        }
        public async Task<T?> FirstOrDefaultAsync(ISpecification<T> specification, CancellationToken cancellationToken = default)
        {
            if (!specification.CacheEnabled)
                return await _sourceRepository.FirstOrDefaultAsync(specification, cancellationToken);
            string tenantId = _iKeyCloakTenantService.GetPCIShieldSolutionuperUserId();
            string key = $"{specification.CacheKey}-{tenantId}-FirstOrDefaultAsync";
            return await GetOrCreateCacheAsync(
                key,
                async (token) => await _sourceRepository.FirstOrDefaultAsync(specification, token),
                cancellationToken: cancellationToken);
        }
        public async ValueTask<T> FirstOrDefaultAsyncFromOtherMicroservice(
            ISpecification<T> specification,
            string pciShieldappAppSuperUserId,
            CancellationToken cancellationToken = default)
        {
            if (!specification.CacheEnabled)
                return await _sourceRepository.FirstOrDefaultAsync(specification, cancellationToken);
            string key = $"{specification.CacheKey}-{pciShieldappAppSuperUserId}-FirstOrDefaultAsync";
            return await GetOrCreateCacheAsync(
                key,
                async (token) => await _sourceRepository.FirstOrDefaultAsync(specification, token),
                cancellationToken: cancellationToken);
        }
        public async ValueTask<List<T>> ListAsyncFromOtherMicroservice(
            ISpecification<T> specification,
            string pciShieldappAppSuperUserId,
            CancellationToken cancellationToken = default)
        {
            if (!specification.CacheEnabled)
                return await _sourceRepository.ListAsync(specification, cancellationToken);
            string key = $"{specification.CacheKey}-{pciShieldappAppSuperUserId}-ListAsync";
            return await GetOrCreateCacheAsync(
                key,
                async (token) => await _sourceRepository.ListAsync(specification, token),
                cancellationToken: cancellationToken);
        }
        public async Task<List<T>> ListAsync(CancellationToken cancellationToken = default)
        {
            string tenantId = _iKeyCloakTenantService.GetPCIShieldSolutionuperUserId();
            string key = $"{typeof(T).Name}-{tenantId}-List";
            return await GetOrCreateCacheAsync(
                key,
                async (token) => await _sourceRepository.ListAsync(token),
                cancellationToken: cancellationToken);
        }
        public async Task<int> CountAsync(ISpecification<T> specification, CancellationToken cancellationToken = default)
        {
            if (!specification.CacheEnabled)
                return await _sourceRepository.CountAsync(specification, cancellationToken);
            string key = $"{specification.CacheKey}-Count";
            return await GetOrCreateCacheAsync(
                key,
                async (token) => await _sourceRepository.CountAsync(specification, token),
                cancellationToken: cancellationToken);
        }
        public async Task<int> CountAsync(CancellationToken cancellationToken = default)
        {
            string key = $"{typeof(T).Name}-Count";
            return await GetOrCreateCacheAsync(
                key,
                async (token) => await _sourceRepository.CountAsync(token),
                cancellationToken: cancellationToken);
        }
        public async Task<bool> AnyAsync(ISpecification<T> specification, CancellationToken cancellationToken = default)
        {
            if (!specification.CacheEnabled)
                return await _sourceRepository.AnyAsync(specification, cancellationToken);
            string key = $"{specification.CacheKey}-Any";
            return await GetOrCreateCacheAsync(
                key,
                async (token) => await _sourceRepository.AnyAsync(specification, token),
                cancellationToken: cancellationToken);
        }
        public async Task<bool> AnyAsync(CancellationToken cancellationToken = default)
        {
            string key = $"{typeof(T).Name}-Any";
            return await GetOrCreateCacheAsync(
                key,
                async (token) => await _sourceRepository.AnyAsync(token),
                cancellationToken: cancellationToken);
        }
        public Task<T?> SingleOrDefaultAsync(ISingleResultSpecification<T> specification, CancellationToken cancellationToken = default)
        {
            throw new NotImplementedException();
        }
        public Task<TResult?> SingleOrDefaultAsync<TResult>(ISingleResultSpecification<T, TResult> specification, CancellationToken cancellationToken = default)
        {
            throw new NotImplementedException();
        }
#if NET6_0_OR_GREATER
        public IAsyncEnumerable<T> AsAsyncEnumerable(ISpecification<T> specification)
        {
            throw new NotImplementedException();
        }
#endif
    }
#pragma warning restore EXTEXP0018
}
