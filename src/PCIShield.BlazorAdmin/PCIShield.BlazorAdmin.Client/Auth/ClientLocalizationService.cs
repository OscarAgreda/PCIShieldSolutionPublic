using System;
using System.Collections.Generic;
using System.Globalization;
using System.Reactive.Linq;
using System.Reactive.Subjects;
using System.Threading.Tasks;

using PCIShield.Client.Services.Auth;

using LanguageExt;

using Microsoft.Extensions.Caching.Memory;

using static LanguageExt.Prelude;

namespace PCIShield.BlazorAdmin.Client.Auth
{
    public interface IClientLocalizationService
    {
        string GetString(string key);
        Task<string> GetStringAsync(string key);
        Either<string, bool> SetCulture(string culture);
        Task<Either<string, bool>> SetCultureAsync(string culture);
    }

    public class ClientLocalizationService : IClientLocalizationService
    {
        private readonly IHttpAppLocalizationClientService _localizationClient;
        private readonly IMemoryCache _memoryCache;
        private readonly TimeSpan _cacheExpiration = TimeSpan.FromMinutes(30);
        private readonly Subject<string> _cacheUpdates = new Subject<string>();
        private readonly System.Collections.Generic.HashSet<string> _cacheKeys = new System.Collections.Generic.HashSet<string>();

        public IObservable<string> CacheUpdates => _cacheUpdates.AsObservable();

        public ClientLocalizationService(
            IHttpAppLocalizationClientService localizationClient,
            IMemoryCache memoryCache)
        {
            _localizationClient = localizationClient ?? throw new ArgumentNullException(nameof(localizationClient));
            _memoryCache = memoryCache ?? throw new ArgumentNullException(nameof(memoryCache));
        }

        public string GetString(string key)
        {
            string cacheKey = $"localization_{CultureInfo.CurrentUICulture.Name}_{key}";
            if (_memoryCache.TryGetValue(cacheKey, out string cachedValue))
            {
                return cachedValue;
            }
            string placeholder = $"[{key}]";
            Observable
                .FromAsync(() => GetStringAsync(key))
                .Timeout(TimeSpan.FromSeconds(6))
                .Catch<string, Exception>(ex =>
                {
                    Console.Error.WriteLine($"Error fetching localization: {ex.Message}");
                    return Observable.Return(placeholder);
                })
                .Subscribe(result =>
                {
                    _cacheUpdates.OnNext(key);
                });

            return placeholder;
        }

        public async Task<string> GetStringAsync(string key)
        {
            string cacheKey = $"localization_{CultureInfo.CurrentUICulture.Name}_{key}";
            if (_memoryCache.TryGetValue(cacheKey, out string cachedValue))
            {
                return cachedValue;
            }
            Either<string, string> result = await _localizationClient.GetStringAsync(key, CultureInfo.CurrentUICulture.Name);
            string value = result.Match(
                Right: value => value,
                Left: error => $"[{key}]"
            );
            _memoryCache.Set(cacheKey, value, _cacheExpiration);
            _cacheKeys.Add(cacheKey);

            return value;
        }

        public Either<string, bool> SetCulture(string culture)
        {
            Observable
                .FromAsync(() => SetCultureAsync(culture))
                .Timeout(TimeSpan.FromSeconds(6))
                .Catch<Either<string, bool>, Exception>(ex =>
                {
                    Console.Error.WriteLine($"Background operation failed: {ex.Message}");
                    return Observable.Return(Left<string, bool>("Background operation failed"));
                })
                .Subscribe(result => { });
            return Right<string, bool>(true);
        }

        public Task<Either<string, bool>> SetCultureAsync(string culture)
        {
            foreach (var key in _cacheKeys)
            {
                _memoryCache.Remove(key);
            }
            _cacheKeys.Clear();
            return _localizationClient.SetUserPreferredCultureAsync(culture);
        }
    }
}