using System.Reactive.Linq;
using System.Text.Json;
using PCIShield.Domain.ModelsDto;
using Magic.IndexedDb;
using Magic.IndexedDb.SchemaAnnotations;
using Microsoft.AspNetCore.Components;
using Microsoft.Extensions.Logging;
using Microsoft.JSInterop;
namespace PCIShield.Client.Services.Common.Cache
{
    [MagicTable("RenderCache", "BlazorCache")]
    public class RenderCacheEntry
    {
        [MagicPrimaryKey("id")]
        public string Id { get; set; }
        [MagicIndex("componentType")]
        public string ComponentType { get; set; }
        [MagicIndex("lastAccess")]
        public DateTime LastAccess { get; set; }
        [MagicEncrypt]
        public string SerializedContent { get; set; }
        [MagicIndex("shouldRender")]
        public bool ShouldRender { get; set; }
        [MagicIndex("expiresAt")]
        public DateTime ExpiresAt { get; set; }
        [MagicNotMapped]
        public RenderFragment CachedRender { get; set; }
    }
    [MagicTable("CustomerCache", "BlazorCache")]
    public class CustomerCacheEntry
    {
        [MagicPrimaryKey("id")]
        public string Id { get; set; }
        [MagicIndex("searchTerm")]
        public string SearchTerm { get; set; }
        [MagicIndex("lastAccess")]
        public DateTime LastAccess { get; set; }
        [MagicEncrypt]
        public string SerializedData { get; set; }
    }
    public class MagicCacheService
    {
        private readonly IMagicDbFactory _magicDb;
        private readonly Dictionary<string, object> _memoryCache = new();
        private readonly ILogger<MagicCacheService> _logger;
        private const string DB_NAME = "BlazorCache";
        public MagicCacheService(IMagicDbFactory magicDb, ILogger<MagicCacheService> logger)
        {
            _magicDb = magicDb;
            _logger = logger;
        }
        public async Task<T> GetOrSetAsync<T>(string key, Func<Task<T>> factory, TimeSpan? expiration = null)
        {
            try
            {
                var manager = await _magicDb.GetDbManager(DB_NAME);
                if (_memoryCache.TryGetValue(key, out var cached))
                    return (T)cached;
                var entries = await manager.Where<RenderCacheEntry>(x => x.Id == key).Execute();
                var entry = entries.FirstOrDefault();
                if (entry != null && DateTime.UtcNow < entry.ExpiresAt)
                {
                    var data = JsonSerializer.Deserialize<T>(entry.SerializedContent);
                    _memoryCache[key] = data;
                    return data;
                }
                var value = await factory();
                _memoryCache[key] = value;
                await manager.Add(new RenderCacheEntry
                {
                    Id = key,
                    ComponentType = typeof(T).Name,
                    LastAccess = DateTime.UtcNow,
                    SerializedContent = JsonSerializer.Serialize(value),
                    ExpiresAt = DateTime.UtcNow.Add(expiration ?? TimeSpan.FromHours(1)),
                    ShouldRender = true
                });
                return value;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Cache error for key: {Key}", key);
                return await factory();
            }
        }
        public async Task<RenderFragment> GetOrCreateRenderFragmentAsync(
            string key,
            Func<RenderFragment> factory,
            bool shouldCache = true)
        {
            if (!shouldCache) return factory();
            var manager = await _magicDb.GetDbManager(DB_NAME);
            if (_memoryCache.TryGetValue(key, out var cached))
                return (RenderFragment)cached;
            var fragment = factory();
            _memoryCache[key] = fragment;
            await manager.Add(new RenderCacheEntry
            {
                Id = key,
                ComponentType = "RenderFragment",
                LastAccess = DateTime.UtcNow,
                SerializedContent = "cached",
                ShouldRender = false
            });
            return fragment;
        }
        public async Task<bool> ShouldRenderComponent(string key)
        {
            var manager = await _magicDb.GetDbManager(DB_NAME);
            var entries = await manager.Where<RenderCacheEntry>(x => x.Id == key).Execute();
            var entry = entries.FirstOrDefault();
            return entry?.ShouldRender ?? true;
        }
        public async Task InvalidateAsync(string key)
        {
            var manager = await _magicDb.GetDbManager(DB_NAME);
            var entries = await manager.Where<RenderCacheEntry>(x => x.Id == key).Execute();
            var entry = entries.FirstOrDefault();
            if (entry != null)
            {
                await manager.Delete(entry);
                _memoryCache.Remove(key);
            }
        }
        public async Task InvalidateAllAsync()
        {
            var manager = await _magicDb.GetDbManager(DB_NAME);
            await manager.ClearTable<RenderCacheEntry>();
            _memoryCache.Clear();
        }
    }
    public abstract class OptimizedComponent : ComponentBase, IHandleEvent
    {
        [Inject] protected MagicCacheService Cache { get; set; }
        protected virtual string CacheKey => $"{GetType().Name}_{GetHashCode()}";
        private bool _shouldRender = true;
        protected override async Task OnParametersSetAsync()
        {
            _shouldRender = await Cache.ShouldRenderComponent(CacheKey);
            await base.OnParametersSetAsync();
        }
        protected override bool ShouldRender() => _shouldRender;
        Task IHandleEvent.HandleEventAsync(EventCallbackWorkItem callback, object arg)
        {
            var task = callback.InvokeAsync(arg);
            if (_shouldRender)
            {
                Observable.StartAsync(async () =>
                {
                    await InvokeAsync(StateHasChanged);
                }).Subscribe(onNext: _ =>
                {
                });
            }
            return task;
        }
        protected RenderFragment CreateCachedFragment(string key, RenderFragment fragment) =>
            new(builder =>
            {
                builder.OpenComponent<CacheFragment>(0);
                builder.AddAttribute(1, "Key", key);
                builder.AddAttribute(2, "ChildContent", fragment);
                builder.CloseComponent();
            });
    }
    public class CacheFragment : ComponentBase
    {
        [Parameter] public string Key { get; set; }
        [Parameter] public RenderFragment ChildContent { get; set; }
        [Inject] private MagicCacheService Cache { get; set; }
        protected override async Task OnParametersSetAsync()
        {
            await Cache.GetOrCreateRenderFragmentAsync(Key, () => ChildContent);
        }
    }
}
