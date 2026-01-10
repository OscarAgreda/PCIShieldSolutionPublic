using NRedisStack;
using StackExchange.Redis;
using System.Text.Json.Serialization;
using System.Text.Json;
using System.Threading.Tasks;
using System;
using System.Collections.Generic;
using NRedisStack.RedisStackCommands;
using Microsoft.Extensions.Caching.Distributed;
using System.Threading;
using System.Linq;
using System.Text;

using Microsoft.Extensions.Configuration;

using Newtonsoft.Json;
using JsonException = System.Text.Json.JsonException;
using JsonSerializer = System.Text.Json.JsonSerializer;
namespace PCIShield.Infrastructure.Services.Redis
{
    public class RedisDistributedCacheAdapter : IDistributedCache
    {
        private readonly IRedisCacheService _redisCacheService;
        public RedisDistributedCacheAdapter(IRedisCacheService redisCacheService)
        {
            _redisCacheService = redisCacheService;
        }
        public byte[]? Get(string key)
        {
            return GetAsync(key).GetAwaiter().GetResult();
        }
        public async Task<byte[]?> GetAsync(string key, CancellationToken token = default)
        {
            var value = await _redisCacheService.GetAsync<byte[]>(key);
            return value;
        }
        public void Set(string key, byte[] value, DistributedCacheEntryOptions options)
        {
            SetAsync(key, value, options).GetAwaiter().GetResult();
        }
        public async Task SetAsync(string key, byte[] value, DistributedCacheEntryOptions options, CancellationToken token = default)
        {
            TimeSpan? expiry = GetExpiryTimeSpan(options);
            await _redisCacheService.SetAsync(key, value, expiry);
        }
        public void Refresh(string key)
        {
        }
        public Task RefreshAsync(string key, CancellationToken token = default)
        {
            return Task.CompletedTask;
        }
        public void Remove(string key)
        {
            RemoveAsync(key).GetAwaiter().GetResult();
        }
        public Task RemoveAsync(string key, CancellationToken token = default)
        {
            return _redisCacheService.RemoveAsync(key);
        }
        private TimeSpan? GetExpiryTimeSpan(DistributedCacheEntryOptions options)
        {
            if (options.AbsoluteExpiration.HasValue)
            {
                return options.AbsoluteExpiration.Value - DateTimeOffset.UtcNow;
            }
            if (options.AbsoluteExpirationRelativeToNow.HasValue)
            {
                return options.AbsoluteExpirationRelativeToNow;
            }
            if (options.SlidingExpiration.HasValue)
            {
                return options.SlidingExpiration;
            }
            return null;
        }
    }
    public interface IRedisCacheService
    {
        Task<T?> GetAsync<T>(string key);
        Task SetAsync<T>(string key, T value, TimeSpan? expiry = null);
        Task RemoveAsync(string key);
        Task RemoveByPatternAsync(string pattern);
    }

    public class RedisCacheService : IRedisCacheService
    {
        private readonly IDatabase _database;
        private readonly JsonSerializerOptions _jsonSerializerOptions;
        private readonly ConnectionMultiplexer _connectionMultiplexer;

        public RedisCacheService(IConnectionMultiplexer connectionMultiplexer)
        {
            _connectionMultiplexer = (ConnectionMultiplexer)connectionMultiplexer;
            _database = _connectionMultiplexer.GetDatabase();

            _jsonSerializerOptions = new JsonSerializerOptions
            {
                PropertyNameCaseInsensitive = true,
                DefaultIgnoreCondition = JsonIgnoreCondition.WhenWritingNull,
                ReferenceHandler = ReferenceHandler.IgnoreCycles,
                WriteIndented = false,
                MaxDepth = 64,
                Converters =
                {
                    new JsonStringEnumConverter(JsonNamingPolicy.CamelCase)
                }
            };
        }

        public async Task RemoveByPatternAsync(string pattern)
        {
            try
            {
                if (string.IsNullOrEmpty(pattern))
                    return;

                var endpoints = _connectionMultiplexer.GetEndPoints();
                int totalKeysRemoved = 0;

                foreach (var endpoint in endpoints)
                {
                    var server = _connectionMultiplexer.GetServer(endpoint);
                    if (server == null || !server.IsConnected)
                        continue;

                    for (int dbIndex = 0; dbIndex < 16; dbIndex++)
                    {
                        try
                        {
                            var db = _connectionMultiplexer.GetDatabase(dbIndex);
                            var patternToUse = pattern.Contains("*") ? pattern : pattern + "*";
                            var keys = server.Keys(dbIndex, patternToUse).ToArray();

                            if (keys.Length > 0)
                            {
                                await db.KeyDeleteAsync(keys);
                                totalKeysRemoved += keys.Length;
                            }
                        }
                        catch (Exception ex)
                        {
                            var aa = 7;
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                var aa = 7;
            }
        }

        public async Task<T?> GetAsync<T>(string key)
        {
            try
            {
                var redisValue = await _database.StringGetAsync(key);

                if (!redisValue.HasValue || redisValue.IsNullOrEmpty)
                    return default;

                string json = redisValue.ToString();
                try
                {
                    return JsonSerializer.Deserialize<T>(json, _jsonSerializerOptions);
                }
                catch
                {
                    try
                    {
                        return JsonConvert.DeserializeObject<T>(json);
                    }
                    catch
                    {
                        return default;
                    }
                }
            }
            catch
            {
                return default;
            }
        }

        public async Task SetAsync<T>(string key, T value, TimeSpan? expiry = null)
        {
            try
            {
                if (value == null)
                    throw new ArgumentNullException(nameof(value));
                string json = JsonSerializer.Serialize(value, _jsonSerializerOptions);
                bool success = await _database.StringSetAsync(key, json, expiry);

                if (!success)
                    throw new Exception($"Failed to set value in Redis for key: {key}");
            }
            catch (Exception ex)
            {
                var aa = 7;
            }
        }

        public async Task RemoveAsync(string key)
        {
            try
            {
                await _database.KeyDeleteAsync(key);
            }
            catch (Exception ex)
            {
                var aa = 7;
            }
        }
    }
}
