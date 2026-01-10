using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using MediatR;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.Logging;
using PCIShield.Infrastructure.Services;
using PCIShieldLib.SharedKernel.Interfaces;
using Microsoft.Extensions.Caching.Hybrid;
using Microsoft.Extensions.DependencyInjection;
namespace PCIShield.Infrastructure.Helpers
{
#pragma warning disable EXTEXP0018
    public class EntityChangedHandler<T> : INotificationHandler<EntityChangedEvent<T>> where T : class, IAggregateRoot
    {
        private readonly HybridCache _cache;
        private readonly IAppLoggerService<EntityChangedHandler<T>> _logger;
        private readonly ICacheKeyListService _cacheKeyListService;
        private readonly IKeyCloakTenantService _iKeyCloakTenantService;
        public EntityChangedHandler(
            HybridCache cache,
            IAppLoggerService<EntityChangedHandler<T>> logger,
            ICacheKeyListService cacheKeyListService,
            IKeyCloakTenantService iKeyCloakTenantService)
        {
            _cache = cache;
            _logger = logger;
            _cacheKeyListService = cacheKeyListService;
            _iKeyCloakTenantService = iKeyCloakTenantService;
        }
        public async Task Handle(EntityChangedEvent<T> notification, CancellationToken cancellationToken)
        {
            var idProperty = typeof(T).GetProperties().FirstOrDefault(p => p.GetCustomAttributes(typeof(KeyAttribute), false).Length > 0);
            if (idProperty != null)
            {
                var idValue = idProperty.GetValue(notification.Entity);
                var superUserId = _iKeyCloakTenantService.GetPCIShieldSolutionuperUserId();
                var keysToRemove = _cacheKeyListService.GetKeys()
                    .Where(key => key.Contains($"-{superUserId}-{idValue}-") ||
                                  key.Contains($"{typeof(T).Name}-{superUserId}-{idValue}"))
                    .ToList();
                foreach (var key in keysToRemove)
                {
                    await _cache.RemoveAsync(key, cancellationToken);
                    _cacheKeyListService.RemoveKey(key);
                    _logger.LogInformation($"Cache entry for {key} invalidated due to entity change.");
                }
            }
        }
    }
#pragma warning restore EXTEXP0018
}
