using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Ardalis.Specification;
using Microsoft.Extensions.Logging;
using PCIShield.Infrastructure.Data;
using PCIShieldLib.SharedKernel.Interfaces;
using System.Linq;
namespace PCIShield.Infrastructure.Services.Redis;

//Sharp edges (high leverage fixes)
//Pattern invalidation via `server.Keys(...).ToArray()` : even if it uses SCAN under the hood, this can become a production hotspot (blocking, memory spikes, multi endpoint, multi db loop 0–15). In ERP scale, this is the first thing that will hurt.
//You already track keys (`_cacheKeyListService.AddKey(key)`), but you’re not using that to delete deterministically. Right now you maintain a “list of keys” and still do broad scans.
//Empty result caching bug : your paged cache hit requires `cachedCount 0`. If a query legitimately returns 0 rows, you’ll never hit cache and will keep hammering the DB.
//Swallowed exceptions (`var aa = 7;`): this will hide correctness issues (serialization mismatches, connection timeouts, partial invalidations). For sagas, hidden failures become “ghost states.”
//`IDistributedCache.Refresh` is a no op : if any upstream uses sliding expiration semantics, behavior will diverge.
//Async isn’t parallelized in the paged method (you `await` each call immediately), so you don’t get the intended concurrency benefit.
//Analyzing cache and transaction handling in Redis stack
//The code involves a Redis cache adapter implementing IDistributedCache, with RedisCacheService and EfRepository utilizing transactions. Key concerns: shared unit of work via BeginTransaction/CommitTransaction can lead to leakage across modules. Consider using a TransactionScope or explicit UoW interface. Also, Redis RemoveByPattern likely uses SCAN under the hood.
//Evaluating Redis key scanning and serialization practices
//In StackExchange.Redis, server.Keys uses SCAN but can be resource heavy when enumerating across multiple databases. It's better to avoid scanning unnecessary database indexes and consider using key sets for invalidation. Regarding serialization, DistributedCacheAdapter stores JSON serialized byte arrays, which results in base64 encoding. While this works, it adds slight overhead compared to using raw bytes.
//Improving cache handling, expiration, and async misuse
//There are issues with cache expiration and sliding TTL not being implemented properly; it uses absolute TTL, making refresh a no op. Cache invalidation is risky due to pattern overuse. Storing keys using "tagging" by entity and tenant for simplicity could help. Also, caching should handle empty results efficiently instead of repeated queries. Async operations in GetPagedResultAsync aren’t parallel, and naming confusion adds to the issues.
//Improving thread safety, key naming, and cache handling
//For thread safety, avoid casting the connection multiplexer and ensure proper database interaction. Key naming using ' ' or ' ' could lead to collisions, so consider a structured format like {tenant}:{entity}:{specHash}. Cache repository should stay in the application layer, especially for cross module caching. Also, improve parallelization by using WhenAll for tasks.



/// <summary>
/// **//
/// </summary>
/// <typeparam name="T"></typeparam>
public class RedisCacheRepository<T> : IReadRedisRepository<T> where T : class, IAggregateRoot
{
    private readonly IRedisCacheService _cache;
    private readonly TimeSpan _cacheExpiration = TimeSpan.FromSeconds(20);
    private readonly ICacheKeyListService _cacheKeyListService;
    private readonly IKeyCloakTenantService _iKeyCloakTenantService;
    private readonly IAppLoggerService<RedisCacheRepository<T>> _seriLogger;
    private readonly EfRepository<T> _sourceRepository;
    public RedisCacheRepository(IRedisCacheService cache,
        EfRepository<T> sourceRepository, ICacheKeyListService cacheKeyListService,
        IKeyCloakTenantService iKeyCloakTenantService, IAppLoggerService<RedisCacheRepository<T>> seriLogger)
    {
        _cache = cache;
        _sourceRepository = sourceRepository;
        _cacheKeyListService = cacheKeyListService;
        _iKeyCloakTenantService = iKeyCloakTenantService;
        _seriLogger = seriLogger;
    }
    public async Task InvalidateEntityCacheAsync<TEntity>(
        Guid? entityId = null,
        string? customSpecificationName = null,
        int? pageSize = null,
        int? pageNumber = null,
        Type? projectionType = null) where TEntity : class, IAggregateRoot
    {
        string entityName = typeof(TEntity).Name;
        string? tenantId = _iKeyCloakTenantService.GetPCIShieldSolutionuperUserId();
        List<string> patterns = new List<string>();
        if (entityId == null && pageSize == null && pageNumber == null && customSpecificationName == null)
        {
            patterns.Add($"{entityName}*--{tenantId}-*");
        }
        else
        {
            if (entityId.HasValue)
            {
                patterns.Add($"{entityName}AdvancedGraphSpec-{entityId}");
                patterns.Add($"{entityName}ByIdSpec-{entityId}--{tenantId}-*");
                patterns.Add($"*{entityId}*--{tenantId}-*");
                patterns.Add($"{entityName}AdvancedGraphSpec-{entityId}--{tenantId}-*");
            }
            if (!string.IsNullOrEmpty(customSpecificationName))
            {
                patterns.Add($"{customSpecificationName}--{tenantId}-*");
            }
            if (pageSize.HasValue && pageNumber.HasValue)
            {
                patterns.Add($"{entityName}ListPagedSpec-{pageNumber}-{pageSize}--{tenantId}-*");
                patterns.Add($"{entityName}ListPagedSpec--{tenantId}-TotalCount");
                patterns.Add($"{entityName}ListPagedSpec--{tenantId}-PagedItems-*-{pageNumber}-{pageSize}");
                patterns.Add($"{entityName}ListPagedSpec--{tenantId}-*");
            }
            else if (pageSize.HasValue || pageNumber.HasValue)
            {
                patterns.Add($"{entityName}ListPagedSpec-*-*--{tenantId}-*");
            }
            if (projectionType != null)
            {
                string projectionName = projectionType.Name;
                patterns.Add($"*--{tenantId}-FirstOrDefaultAsync-{projectionName}");
                patterns.Add($"*--{tenantId}-ListAsync-{projectionName}");
                if (pageSize.HasValue && pageNumber.HasValue)
                {
                    patterns.Add($"*--{tenantId}-PagedItems-{projectionName}-{pageNumber}-{pageSize}");
                }
                else
                {
                    patterns.Add($"*--{tenantId}-PagedItems-{projectionName}-*-*");
                }
            }
        }
        foreach (string pattern in patterns)
        {
            await _cache.RemoveByPatternAsync(pattern);
        }
    }
    public async Task<T?> FirstOrDefaultAsync(ISpecification<T> specification, CancellationToken cancellationToken = default)
    {
        try
        {
            if (specification.CacheEnabled)
            {
                string? pciShieldappAppSuperUserId = _iKeyCloakTenantService.GetPCIShieldSolutionuperUserId();
                string? key = $"{specification.CacheKey}-{pciShieldappAppSuperUserId}-GetBySpecAsync";
                T? cachedResult = await _cache.GetAsync<T>(key);
                if (cachedResult != null)
                {
                    _seriLogger.LogWarning("Used Cache data for " + key);
                    return cachedResult;
                }
                var result = await _sourceRepository.FirstOrDefaultAsync(specification, cancellationToken);
                if (result != null)
                {
                    await _cache.SetAsync(key, result, _cacheExpiration);
                    _cacheKeyListService.AddKey(key);
                }
                return result;
            }
            return await _sourceRepository.FirstOrDefaultAsync(specification, cancellationToken);
        }
        catch (Exception e)
        {
        }
        return await _sourceRepository.FirstOrDefaultAsync(specification, cancellationToken);
    }
    public async Task<T?> FirstOrDefaultAsyncFromOtherMicroservice(ISpecification<T> specification, string pciShieldappAppSuperUserId,
       CancellationToken cancellationToken = default)
    {
        try
        {
            if (specification.CacheEnabled)
            {
                string? key = $"{specification.CacheKey}-{pciShieldappAppSuperUserId}-GetBySpecAsync";
                var cachedResult = await _cache.GetAsync<T>(key);
                if (cachedResult != null)
                {
                    _seriLogger.LogWarning("Used Cache data for " + key);
                    return cachedResult;
                }
                var result = await _sourceRepository.FirstOrDefaultAsync(specification, cancellationToken);
                if (result != null)
                {
                    await _cache.SetAsync(key, result, _cacheExpiration);
                    _cacheKeyListService.AddKey(key);
                }
                return result;
            }
        }
        catch (Exception e)
        {
        }
        return await _sourceRepository.FirstOrDefaultAsync(specification, cancellationToken);
    }
    public async Task<List<T>> ListAsync(ISpecification<T> specification, CancellationToken cancellationToken = default)
    {
        try
        {
            if (specification.CacheEnabled)
            {
                string pciShieldappAppSuperUserId = _iKeyCloakTenantService.GetPCIShieldSolutionuperUserId();
                string? key = $"{specification.CacheKey}-{pciShieldappAppSuperUserId}-ListAsync";
                var cachedResult = await _cache.GetAsync<List<T>>(key);
                if (cachedResult != null)
                {
                    _seriLogger.LogWarning("Used Cache data for " + key);
                    return cachedResult;
                }
                var result = await _sourceRepository.ListAsync(specification, cancellationToken);
                if (result != null)
                {
                    await _cache.SetAsync(key, result, _cacheExpiration);
                    _cacheKeyListService.AddKey(key);
                }
                return result;
            }
            return await _sourceRepository.ListAsync(specification, cancellationToken);
        }
        catch (Exception e)
        {
            var aa = 6;
        }
        return await _sourceRepository.ListAsync(specification, cancellationToken);
    }
    public async Task<List<T>> ListAsyncWithCustomExpiration(ISpecification<T> specification, TimeSpan? absoluteExpiration,
        CancellationToken cancellationToken = default)
    {
        try
        {
            if (specification.CacheEnabled)
            {
                string? pciShieldappAppSuperUserId = _iKeyCloakTenantService.GetPCIShieldSolutionuperUserId();
                string? key = $"{specification.CacheKey}-{pciShieldappAppSuperUserId}-ListAsync";
                var cachedResult = await _cache.GetAsync<List<T>>(key);
                if (cachedResult != null)
                {
                    _seriLogger.LogWarning("Used Cache data for " + key);
                    return cachedResult;
                }
                var result = await _sourceRepository.ListAsync(specification, cancellationToken);
                if (result != null)
                {
                    await _cache.SetAsync(key, result, absoluteExpiration);
                    _cacheKeyListService.AddKey(key);
                }
                return result;
            }
        }
        catch (Exception e)
        {
        }
        return await _sourceRepository.ListAsync(specification, cancellationToken);
    }
    public async Task<List<T>> ListAsyncFromOtherMicroservice(ISpecification<T> specification, string pciShieldappAppSuperUserId,
        CancellationToken cancellationToken = default)
    {
        try
        {
            if (specification.CacheEnabled)
            {
                string? key = $"{specification.CacheKey}-{pciShieldappAppSuperUserId}-ListAsync";
                var cachedResult = await _cache.GetAsync<List<T>>(key);
                if (cachedResult != null)
                {
                    _seriLogger.LogWarning("Used Cache data for " + key);
                    return cachedResult;
                }
                var result = await _sourceRepository.ListAsync(specification, cancellationToken);
                if (result != null)
                {
                    await _cache.SetAsync(key, result, _cacheExpiration);
                    _cacheKeyListService.AddKey(key);
                }
                return result;
            }
        }
        catch (Exception e)
        {
        }
        return await _sourceRepository.ListAsync(specification, cancellationToken);
    }
    public async Task<PagedResult<T>> GetPagedResultAsync(
      PagedSpecification<T> specification,
      int pageNumber,
      int pageSize,
      CancellationToken cancellationToken = default)
    {
        try
        {
            if (specification.CacheEnabled)
            {
                string pciShieldappAppSuperUserId = _iKeyCloakTenantService.GetPCIShieldSolutionuperUserId();
                string itemsKey = $"{specification.CacheKey}-{pciShieldappAppSuperUserId}-PagedItems-{pageNumber}-{pageSize}";
                string countKey = $"{specification.CacheKey}-{pciShieldappAppSuperUserId}-TotalCount";
                var cachedItemsTask = await _cache.GetAsync<List<T>>(itemsKey);
                var cachedCountTask = await _cache.GetAsync<int>(countKey);
                var cachedItems =  cachedItemsTask;
                var cachedCount =  cachedCountTask;
                if (cachedItems != null && cachedCount > 0)
                {
                    _seriLogger.LogWarning($"Used cache data for {itemsKey} and {countKey}");
                    return new PagedResult<T>
                    {
                        Items = cachedItems,
                        TotalCount = cachedCount,
                        PageNumber = pageNumber,
                        PageSize = pageSize
                    };
                }
                var countSpec = specification.GetCountSpecification();
                var itemsTask = await _sourceRepository.ListAsync(specification, cancellationToken);
                var countTask = await _sourceRepository.CountAsync(countSpec, cancellationToken);
                var items = itemsTask;
                var count = countTask;
                if (items != null && items.Any())
                {
                    await _cache.SetAsync(itemsKey, items, _cacheExpiration);
                    _cacheKeyListService.AddKey(itemsKey);
                }
                await _cache.SetAsync(countKey, count, _cacheExpiration);
                _cacheKeyListService.AddKey(countKey);
                return new PagedResult<T>
                {
                    Items = items,
                    TotalCount = count,
                    PageNumber = pageNumber,
                    PageSize = pageSize
                };
            }
            var nonCachedCountSpec = specification.GetCountSpecification();
            var nonCachedItemsTask = await _sourceRepository.ListAsync(specification, cancellationToken);
            var nonCachedCountTask = await  _sourceRepository.CountAsync(nonCachedCountSpec, cancellationToken);
            return new PagedResult<T>
            {
                Items = nonCachedItemsTask,
                TotalCount = nonCachedCountTask,
                PageNumber = pageNumber,
                PageSize = pageSize
            };
        }
        catch (Exception ex)
        {
            _seriLogger.LogError(ex.Message, "Error in GetPagedResultAsync for entity type {EntityType}", typeof(T).Name);
            var countSpec = specification.GetCountSpecification();
            var itemsTask = await _sourceRepository.ListAsync(specification, cancellationToken);
            var countTask = await _sourceRepository.CountAsync(countSpec, cancellationToken);
            return new PagedResult<T>
            {
                Items = itemsTask,
                TotalCount = countTask,
                PageNumber = pageNumber,
                PageSize = pageSize
            };
        }
    }
    public async Task<PagedResult<TResult>> GetPagedResultAsync<TResult>(
        PagedSpecification<T, TResult> specification,
        int pageNumber,
        int pageSize,
        CancellationToken cancellationToken = default)
    {
        try
        {
            if (specification.CacheEnabled)
            {
                string pciShieldappAppSuperUserId = _iKeyCloakTenantService.GetPCIShieldSolutionuperUserId();
                string itemsKey = $"{specification.CacheKey}-{pciShieldappAppSuperUserId}-PagedItems-{typeof(TResult).Name}-{pageNumber}-{pageSize}";
                string countKey = $"{specification.CacheKey}-{pciShieldappAppSuperUserId}-TotalCount";
                var cachedItemsTask = await _cache.GetAsync<List<TResult>>(itemsKey);
                var cachedCountTask = await  _cache.GetAsync<int>(countKey);
                var cachedItems = cachedItemsTask;
                var cachedCount = cachedCountTask;
                if (cachedItems != null && cachedCount > 0)
                {
                    _seriLogger.LogWarning($"Used cache data for {itemsKey} and {countKey}");
                    return new PagedResult<TResult>
                    {
                        Items = cachedItems,
                        TotalCount = cachedCount,
                        PageNumber = pageNumber,
                        PageSize = pageSize
                    };
                }
                var countSpec = specification.GetCountSpecification();
                var itemsTask = await _sourceRepository.ListAsync(specification, cancellationToken);
                var countTask = await _sourceRepository.CountAsync(countSpec, cancellationToken);
                var items = itemsTask;
                var count = countTask;
                if (items != null && items.Any())
                {
                    await _cache.SetAsync(itemsKey, items, _cacheExpiration);
                    _cacheKeyListService.AddKey(itemsKey);
                }
                await _cache.SetAsync(countKey, count, _cacheExpiration);
                _cacheKeyListService.AddKey(countKey);
                return new PagedResult<TResult>
                {
                    Items = items,
                    TotalCount = count,
                    PageNumber = pageNumber,
                    PageSize = pageSize
                };
            }
            var nonCachedCountSpec = specification.GetCountSpecification();
            var nonCachedItemsTask = await _sourceRepository.ListAsync(specification, cancellationToken);
            var nonCachedCountTask = await _sourceRepository.CountAsync(nonCachedCountSpec, cancellationToken);
            return new PagedResult<TResult>
            {
                Items = nonCachedItemsTask,
                TotalCount = nonCachedCountTask,
                PageNumber = pageNumber,
                PageSize = pageSize
            };
        }
        catch (Exception ex)
        {
            _seriLogger.LogError(ex.Message, "Error in GetPagedResultAsync<TResult> for entity type {EntityType} and result type {ResultType}",
                typeof(T).Name, typeof(TResult).Name);
            var countSpec = specification.GetCountSpecification();
            var itemsTask = await _sourceRepository.ListAsync(specification, cancellationToken);
            var countTask = await _sourceRepository.CountAsync(countSpec, cancellationToken);
            return new PagedResult<TResult>
            {
                Items = itemsTask,
                TotalCount = countTask,
                PageNumber = pageNumber,
                PageSize = pageSize
            };
        }
    }
    public Task<T?> SingleOrDefaultAsync(ISingleResultSpecification<T> specification,
                        CancellationToken cancellationToken = new CancellationToken())
    {
        throw new NotImplementedException();
    }
    public Task<TResult?> SingleOrDefaultAsync<TResult>(ISingleResultSpecification<T, TResult> specification,
        CancellationToken cancellationToken = new CancellationToken())
    {
        throw new NotImplementedException();
    }
    public Task<T?> GetByIdAsync<TId>(TId id, CancellationToken cancellationToken = new CancellationToken()) where TId : notnull
    {
        throw new NotImplementedException();
    }
    public Task<T?> GetBySpecAsync(ISpecification<T> specification, CancellationToken cancellationToken = new CancellationToken())
    {
        throw new NotImplementedException();
    }
    public Task<TResult?> GetBySpecAsync<TResult>(ISpecification<T, TResult> specification,
        CancellationToken cancellationToken = new CancellationToken())
    {
        throw new NotImplementedException();
    }
    public Task<bool> AnyAsync(ISpecification<T> specification, CancellationToken cancellationToken = new CancellationToken())
    {
        throw new NotImplementedException();
    }
    public Task<bool> AnyAsync(CancellationToken cancellationToken = new CancellationToken())
    {
        throw new NotImplementedException();
    }
    public IAsyncEnumerable<T> AsAsyncEnumerable(ISpecification<T> specification)
    {
        throw new NotImplementedException();
    }
    public Task<int> CountAsync(ISpecification<T> specification, CancellationToken cancellationToken = new CancellationToken())
    {
        throw new NotImplementedException();
    }
    public Task<int> CountAsync(CancellationToken cancellationToken = new CancellationToken())
    {
        throw new NotImplementedException();
    }
    public async Task<TResult?> FirstOrDefaultAsync<TResult>(ISpecification<T, TResult> specification,
        CancellationToken cancellationToken = default)
    {
        try
        {
            if (specification.CacheEnabled)
            {
                var pciShieldappAppSuperUserId = _iKeyCloakTenantService.GetPCIShieldSolutionuperUserId();
                var key = $"{specification.CacheKey}-{pciShieldappAppSuperUserId}-FirstOrDefaultAsync-{typeof(TResult).Name}";
                var cachedResult = await _cache.GetAsync<TResult>(key);
                if (cachedResult != null)
                {
                    _seriLogger.LogWarning($"Cache hit for {key}");
                    return cachedResult;
                }
                var result = await _sourceRepository.FirstOrDefaultAsync(specification, cancellationToken);
                if (result != null)
                {
                    await _cache.SetAsync(key, result, _cacheExpiration);
                    _cacheKeyListService.AddKey(key);
                }
                return result;
            }
            return await _sourceRepository.FirstOrDefaultAsync(specification, cancellationToken);
        }
        catch (Exception ex)
        {
            _seriLogger.LogError(ex, "Error in FirstOrDefaultAsync<TResult>");
            return await _sourceRepository.FirstOrDefaultAsync(specification, cancellationToken);
        }
    }
    public Task<List<T>> ListAsync(CancellationToken cancellationToken = new CancellationToken())
    {
        throw new NotImplementedException();
    }
    public async Task<List<TResult>> ListAsync<TResult>(ISpecification<T, TResult> specification,
        CancellationToken cancellationToken = default)
    {
        try
        {
            if (specification.CacheEnabled)
            {
                var pciShieldappAppSuperUserId = _iKeyCloakTenantService.GetPCIShieldSolutionuperUserId();
                var key = $"{specification.CacheKey}-{pciShieldappAppSuperUserId}-ListAsync-{typeof(TResult).Name}";
                var cachedResult = await _cache.GetAsync<List<TResult>>(key);
                if (cachedResult != null)
                {
                    _seriLogger.LogWarning($"Cache hit for {key}");
                    return cachedResult;
                }
                var result = await _sourceRepository.ListAsync(specification, cancellationToken);
                if (result != null && result.Any())
                {
                    await _cache.SetAsync(key, result, _cacheExpiration);
                    _cacheKeyListService.AddKey(key);
                }
                return result;
            }
            return await _sourceRepository.ListAsync(specification, cancellationToken);
        }
        catch (Exception ex)
        {
            _seriLogger.LogError(ex, "Error in ListAsync<TResult>");
            return await _sourceRepository.ListAsync(specification, cancellationToken);
        }
    }
}