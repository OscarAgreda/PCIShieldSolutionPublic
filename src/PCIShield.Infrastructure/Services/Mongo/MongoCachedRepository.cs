using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Ardalis.Specification;
using Microsoft.Extensions.Logging;
using PCIShield.Infrastructure.Data;
using PCIShieldLib.SharedKernel.Interfaces;
namespace PCIShield.Infrastructure.Services.Mongo;
public class MongoCacheRepository<T> : IReadMongoRepository<T> where T : class, IAggregateRoot
{
    private readonly IMongoCacheService _cache;
    private readonly TimeSpan _cacheExpiration = TimeSpan.FromSeconds(10);
    private readonly ICacheKeyListService _cacheKeyListService;
    private readonly IKeyCloakTenantService _iKeyCloakTenantService;
    private readonly IAppLoggerService<MongoCacheRepository<T>> _seriLogger;
    private readonly EfRepository<T> _sourceRepository;
    public MongoCacheRepository(IMongoCacheService cache,
        EfRepository<T> sourceRepository, ICacheKeyListService cacheKeyListService,
        IKeyCloakTenantService iKeyCloakTenantService, IAppLoggerService<MongoCacheRepository<T>> seriLogger)
    {
        _cache = cache;
        _sourceRepository = sourceRepository;
        _cacheKeyListService = cacheKeyListService;
        _iKeyCloakTenantService = iKeyCloakTenantService;
        _seriLogger = seriLogger;
    }
    public async Task<T?> FirstOrDefaultAsync(ISpecification<T> specification, CancellationToken cancellationToken = default)
    {
        try
        {
            if (specification.CacheEnabled)
            {
                string? tenantId = _iKeyCloakTenantService.GetPCIShieldSolutionuperUserId();
                string? key = $"{specification.CacheKey}-{tenantId}-GetBySpecAsync";
                T? cachedResult = await _cache.GetAsync<T>(key);
                if (cachedResult != null)
                {
                    _seriLogger.LogInformation($"Mongo Cached {typeof(T).Name} with key {key}");
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
                    _seriLogger.LogInformation($"Mongo Cached {typeof(T).Name} with key {key}");
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
    public async Task<List<T>> ListAsync(ISpecification<T> specification,
                            CancellationToken cancellationToken = default)
    {
        try
        {
            if (specification.CacheEnabled)
            {
                string tenantId = _iKeyCloakTenantService.GetPCIShieldSolutionuperUserId();
                string key = $"{specification.CacheKey}-{tenantId}-ListAsync";
                var cachedResult = await _cache.GetAsync<List<T>>(key);
                if (cachedResult != null)
                {
                    _seriLogger.LogInformation($"Mongo Cached {typeof(T).Name} with key {key}");
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
                string? tenantId = _iKeyCloakTenantService.GetPCIShieldSolutionuperUserId();
                string? key = $"{specification.CacheKey}-{tenantId}-ListAsync";
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
                string? key = $"{specification.CacheKey}-{pciShieldappAppSuperUserId}-GetBySpecAsync";
                var cachedResult = await _cache.GetAsync<List<T>>(key);
                if (cachedResult != null)
                {
                    _seriLogger.LogInformation($"Mongo Cached {typeof(T).Name} with key {key}");
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
    public Task<TResult?> FirstOrDefaultAsync<TResult>(ISpecification<T, TResult> specification,
        CancellationToken cancellationToken = new CancellationToken())
    {
        throw new NotImplementedException();
    }
    public Task<List<T>> ListAsync(CancellationToken cancellationToken = new CancellationToken())
    {
        throw new NotImplementedException();
    }
    public Task<List<TResult>> ListAsync<TResult>(ISpecification<T, TResult> specification, CancellationToken cancellationToken = new CancellationToken())
    {
        throw new NotImplementedException();
    }
}