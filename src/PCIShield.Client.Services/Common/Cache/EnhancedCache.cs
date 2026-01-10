/*
 *
 *Here's how the caching system works in your code:
   ```csharp
   1. CacheEntry - Database schema for storing cached items
      - Id: Primary key
      - TableName: Index to group cache entries
      - SerializedContent: Encrypted storage of serialized data
      - LastAccess: Index for tracking access times
   2. EnhancedCache<T> - Base caching class
      - Maintains in-memory cache (_memoryCache)
      - Persists to IndexedDB via Magic.IndexedDb
      - Methods: GetAsync(), SetAsync(), ClearAsync()
   3. CustomerCache/InvoiceCache - Type-specific implementations
      - Inherit from EnhancedCache<T>
      - Add domain-specific method names
   Usage flow:
   ```ts
   await CustomerCache.GetCustomerAsync()
   ↓
   Check _memoryCache
   ↓ 
   If not found, query IndexedDB
   ↓
   Deserialize and cache in memory
   await CustomerCache.AddCustomerAsync(customer)
   ↓
   Update _memoryCache
   ↓
   Clear existing entries in IndexedDB
   ↓
   Serialize and store new entry
   ```
   Register in Program.cs:
   ```csharp
   builder.Services.AddBlazorDB(options => {
       options.Name = "BlazorCache";
       options.Version = "1"; 
       options.EncryptionKey = "key";
   });
   builder.Services.AddScoped<CustomerCache>();
   builder.Services.AddScoped<InvoiceCache>();
   ```
 *
 *
 */
using System.Text.Json;
using PCIShield.Domain.ModelsDto;
using Magic.IndexedDb;
using Magic.IndexedDb.SchemaAnnotations;
using Microsoft.AspNetCore.Components;
using Microsoft.Extensions.Logging;
using Microsoft.JSInterop;
namespace PCIShield.Client.Services.Common.Cache
{
    [MagicTable("CacheEntries", "BlazorCache")]
    public class CacheEntry
    {
        [MagicPrimaryKey("id")]
        public string Id { get; set; }
        [MagicIndex("tableName")]
        public string TableName { get; set; }
        [MagicEncrypt]
        public string SerializedContent { get; set; }
        [MagicIndex("lastAccess")]
        public DateTime LastAccess { get; set; }
    }
    public abstract class EnhancedCache<T> where T : class
    {
        private readonly IMagicDbFactory _magicDb;
        private readonly ILogger _logger;
        private readonly string _tableName;
        private T _memoryCache;
        private const string DB_NAME = "BlazorCache";
        protected EnhancedCache(IMagicDbFactory magicDb, ILogger logger, string tableName)
        {
            _magicDb = magicDb;
            _logger = logger;
            _tableName = tableName;
        }
        public async Task<T> GetAsync()
        {
            try
            {
                if (_memoryCache != null) return _memoryCache;
                var manager = await _magicDb.GetDbManager(DB_NAME);
                var entries = await manager.Where<CacheEntry>(x => x.TableName == _tableName).Execute();
                var entry = entries.FirstOrDefault();
                if (entry != null)
                {
                    _memoryCache = JsonSerializer.Deserialize<T>(entry.SerializedContent);
                    return _memoryCache;
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Cache retrieval error for {TableName}", _tableName);
            }
            return null;
        }
        public async Task SetAsync(T item)
        {
            try
            {
                _memoryCache = item;
                var manager = await _magicDb.GetDbManager(DB_NAME);
                var entries = await manager.Where<CacheEntry>(x => x.TableName == _tableName).Execute();
                foreach (var entry in entries)
                {
                    await manager.Delete(entry);
                }
                await manager.Add(new CacheEntry
                {
                    Id = Guid.NewGuid().ToString(),
                    TableName = _tableName,
                    SerializedContent = JsonSerializer.Serialize(item),
                    LastAccess = DateTime.UtcNow
                });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Cache set error for {TableName}", _tableName);
            }
        }
        public async Task ClearAsync()
        {
            _memoryCache = null;
            try
            {
                var manager = await _magicDb.GetDbManager(DB_NAME);
                var entries = await manager.Where<CacheEntry>(x => x.TableName == _tableName).Execute();
                foreach (var entry in entries)
                {
                    await manager.Delete(entry);
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Cache clear error for {TableName}", _tableName);
            }
        }
    }
}
