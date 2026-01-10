/*
   To use this caching behavior, you would implement your queries like this:
   ```csharp
   public class GetCustomerQuery : IRequest<CustomerDto>, ICacheableQuery
   {
       public int CustomerId { get; set; }
       public string CacheKey => $"customer-{CustomerId}";
       public TimeSpan? CacheExpiration => TimeSpan.FromMinutes(30);
   }
   ```
   Key features of this implementation:
   1. **Integration with Existing Infrastructure**:
      - Uses your `IRedisCacheService`
      - Integrates with your tenant service
      - Uses your logging infrastructure
   2. **Flexibility**:
      - Optional cache expiration time per query
      - Tenant-aware caching
      - Type-safe cache keys
   3. **Error Handling**:
      - Graceful fallback to normal execution if caching fails
      - Proper logging of cache hits/misses
      - Null checking and validation
   4. **Performance Considerations**:
      - Short-circuits on cache hits
      - Only caches non-null responses
      - Async throughout
 */
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using PCIShield.Infrastructure.Services.Redis;
using PCIShield.Infrastructure.Services;
using MediatR;
namespace PCIShield.Infrastructure.Behaviors
{
    public interface ICacheableQuery
    {
        string CacheKey { get; }
        TimeSpan? CacheExpiration { get; }
    }
    public class CachingBehavior<TRequest, TResponse> : IPipelineBehavior<TRequest, TResponse>
        where TRequest : ICacheableQuery
    {
        private readonly IRedisCacheService _redisCacheService;
        private readonly IKeyCloakTenantService _keyCloakTenantService;
        private readonly IAppLoggerService<CachingBehavior<TRequest, TResponse>> _logger;
        public CachingBehavior(
            IRedisCacheService redisCacheService,
            IKeyCloakTenantService keyCloakTenantService,
            IAppLoggerService<CachingBehavior<TRequest, TResponse>> logger)
        {
            _redisCacheService = redisCacheService ?? throw new ArgumentNullException(nameof(redisCacheService));
            _keyCloakTenantService = keyCloakTenantService ?? throw new ArgumentNullException(nameof(keyCloakTenantService));
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        }
        public async Task<TResponse> Handle(
            TRequest request,
            RequestHandlerDelegate<TResponse> next,
            CancellationToken cancellationToken)
        {
            if (request is null)
            {
                throw new ArgumentNullException(nameof(request));
            }
            if (string.IsNullOrWhiteSpace(request.CacheKey))
            {
                return await next();
            }
            string tenantId = _keyCloakTenantService.GetPCIShieldSolutionuperUserId();
            string fullCacheKey = $"{request.CacheKey}-{tenantId}-{typeof(TResponse).Name}";
            try
            {
                var cachedResponse = await _redisCacheService.GetAsync<TResponse>(fullCacheKey);
                if (cachedResponse != null)
                {
                    _logger.LogInformation($"Cache hit for key: {fullCacheKey}");
                    return cachedResponse;
                }
                _logger.LogInformation($"Cache miss for key: {fullCacheKey}");
                var response = await next();
                if (response != null)
                {
                    var expiration = request.CacheExpiration ?? TimeSpan.FromMinutes(5);
                    await _redisCacheService.SetAsync(fullCacheKey, response, expiration);
                    _logger.LogInformation($"Cached response for key: {fullCacheKey}, expiration: {expiration}");
                }
                return response;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Error in caching behavior for key: {fullCacheKey}");
                return await next();
            }
        }
    }
}
