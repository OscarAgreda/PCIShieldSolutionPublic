using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Threading.Tasks;

using Ardalis.GuardClauses;

using PCIShield.Infrastructure.Data;
using PCIShield.Infrastructure.Data.Specifications;
using PCIShield.Infrastructure.Services;

using PCIShieldLib.SharedKernel.Interfaces;

using LanguageExt;

using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.Localization;
using Microsoft.Extensions.Logging;

using static LanguageExt.Prelude;

namespace PCIShield.Api.Auth.AuthServices
{

    public class DbStringLocalizer : IStringLocalizer
    {
        private readonly IDbLocalizationService _locService;

        public DbStringLocalizer(IDbLocalizationService locService)
        {
            _locService = Guard.Against.Null(locService, nameof(locService));
        }

        public LocalizedString this[string name]
        {
            get
            {
                Guard.Against.NullOrWhiteSpace(name, nameof(name));
                Either<string, string> result = _locService.GetString(name);

                return result.Match(
                    Right: value => new LocalizedString(name, value, resourceNotFound: false),
                    Left: error => new LocalizedString(name, $"[{name}]", resourceNotFound: true)
                );
            }
        }

        public LocalizedString this[string name, params object[] arguments]
        {
            get
            {
                Guard.Against.NullOrWhiteSpace(name, nameof(name));
                Either<string, string> result = _locService.GetString(name);

                return result.Match(
                    Right: value => new LocalizedString(name, string.Format(value, arguments), resourceNotFound: false),
                    Left: error => new LocalizedString(name, $"[{name}]", resourceNotFound: true)
                );
            }
        }

        public IEnumerable<LocalizedString> GetAllStrings(bool includeParentCultures)
        {
            return Enumerable.Empty<LocalizedString>();
        }
    }

    public class DbStringLocalizerFactory : IStringLocalizerFactory
    {
        private readonly IDbLocalizationService _locService;

        public DbStringLocalizerFactory(IDbLocalizationService locService)
        {
            _locService = Guard.Against.Null(locService, nameof(locService));
        }

        public IStringLocalizer Create(Type resourceSource)
        {
            return new DbStringLocalizer(_locService);
        }

        public IStringLocalizer Create(string baseName, string location)
        {
            return new DbStringLocalizer(_locService);
        }
    }

    public interface IDbLocalizationService
    {
        Task<Either<string, string>> GetStringAsync(string key);
        Either<string, string> GetString(string key);
    }

    public class DbLocalizationService : IDbLocalizationService
    {
        private readonly IAuthRepository<AppLocalization> _localizationRepository;
        private readonly IHttpContextAccessor _httpContextAccessor;
        private readonly IMemoryCache _memoryCache;
        private readonly IKeyCloakTenantService _keyCloakTenantService;
        private readonly IAppLoggerService<DbLocalizationService> _logger;
        private readonly TimeSpan _cacheExpiration = TimeSpan.FromHours(24);

        public DbLocalizationService(
            IAuthRepository<AppLocalization> localizationRepository,
            IHttpContextAccessor httpContextAccessor,
            IMemoryCache memoryCache,
            IKeyCloakTenantService keyCloakTenantService,
            IAppLoggerService<DbLocalizationService> logger)
        {
            _localizationRepository = Guard.Against.Null(localizationRepository, nameof(localizationRepository));
            _httpContextAccessor = Guard.Against.Null(httpContextAccessor, nameof(httpContextAccessor));
            _memoryCache = Guard.Against.Null(memoryCache, nameof(memoryCache));
            _keyCloakTenantService = Guard.Against.Null(keyCloakTenantService, nameof(keyCloakTenantService));
            _logger = Guard.Against.Null(logger, nameof(logger));
        }

        public async Task<Either<string, string>> GetStringAsync(string key)
        {
            try
            {
                Guard.Against.NullOrWhiteSpace(key, nameof(key));

                string cultureName = CultureInfo.CurrentUICulture.Name;
                string cacheKey = $"localization_{cultureName}_{key}";
                var tenantId = _keyCloakTenantService.GetTenantId();

                if (_memoryCache.TryGetValue(cacheKey, out string cachedValue))
                {
                    return Right<string, string>(cachedValue);
                }
                var tenantIdGuid = Guid.Parse(tenantId);
                var spec = new AppLocalizationByKeyAndCultureSpec(key, cultureName, tenantIdGuid);
                var localization = await _localizationRepository.FirstOrDefaultAsync(spec);

                if (localization == null)
                {
                    var defaultSpec = new AppLocalizationByKeyDefaultCultureSpec(key, tenantIdGuid);
                    localization = await _localizationRepository.FirstOrDefaultAsync(defaultSpec);
                }

                string result = localization?.Text ?? $"[{key}]";
                _memoryCache.Set(cacheKey, result, _cacheExpiration);

                return Right<string, string>(result);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error retrieving localization string for key: {Key}", key);
                return Left<string, string>($"Error retrieving localization: {ex.Message}");
            }
        }

        public Either<string, string> GetString(string key)
        {
            try
            {
                return GetStringAsync(key).GetAwaiter().GetResult();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error retrieving localization string for key: {Key}", key);
                return Left<string, string>($"Error retrieving localization: {ex.Message}");
            }
        }
    }
}