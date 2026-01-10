using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;
using MongoDB.Bson;
using MongoDB.Bson.Serialization;
using MongoDB.Bson.Serialization.Attributes;
using MongoDB.Bson.Serialization.Conventions;
using MongoDB.Driver;
using PCIShield.Domain.Entities;
namespace PCIShield.Infrastructure.Services.Mongo
{
    public interface IMongoCacheService
    {
        Task<T?> GetAsync<T>(string key) where T : class;
        Task SetAsync<T>(string key, T value, TimeSpan? expiry = null) where T : class;
        Task RemoveAsync(string key);
    }
    public class MongoCacheService : IMongoCacheService
    {
        private readonly IMongoCollection<BsonDocument> _cacheCollection;
        private readonly IAppLoggerService<MongoCacheService> _seriLogger;
        public MongoCacheService(MongoDbContext context, IAppLoggerService<MongoCacheService> seriLogger)
        {
            _seriLogger = seriLogger;
            RegisterConventions();
            _cacheCollection = context.GetCollection<BsonDocument>("CacheCollection");
            EnsureIndexes();
            RegisterGenericClassMaps();
        }
        private void RegisterConventions()
        {
            var pack = new ConventionPack
            {
                new IgnoreExtraElementsConvention(true),
                new CamelCaseElementNameConvention(),
                new EnumRepresentationConvention(BsonType.String)
            };
            ConventionRegistry.Register("ApplicationConventions", pack, t => true);
        }
        private void EnsureIndexes()
        {
            var indexOptions = new CreateIndexOptions { ExpireAfter = TimeSpan.FromSeconds(10) };
            var indexModel = new CreateIndexModel<BsonDocument>(
                Builders<BsonDocument>.IndexKeys.Ascending("Expiration"), indexOptions);
            _cacheCollection.Indexes.CreateOne(indexModel);
        }
        public class CachedItem<T> where T : class
        {
            [BsonId]
            [BsonRepresentation(BsonType.String)]
            public string Id { get; set; }
            [BsonElement("Item")]
            public T Item { get; set; }
            [BsonDateTimeOptions(Kind = DateTimeKind.Utc)]
            public DateTime Expiration { get; set; }
        }
        public async Task<T?> GetAsync<T>(string key) where T : class
        {
            var filter = Builders<BsonDocument>.Filter.Eq("_id", key);
            var document = await _cacheCollection.Find(filter).FirstOrDefaultAsync();
            if (document == null) return null;
            return BsonSerializer.Deserialize<T>(document["Item"].AsBsonDocument);
        }
        public async Task SetAsync<T>(string key, T value, TimeSpan? expiry = null) where T : class
        {
            var document = new BsonDocument
            {
                { "_id", key },
                { "Item", value.ToBsonDocument() },
                { "Expiration", DateTime.UtcNow.Add(expiry ?? TimeSpan.Zero) }
            };
            var options = new ReplaceOptions { IsUpsert = true };
            await _cacheCollection.ReplaceOneAsync(Builders<BsonDocument>.Filter.Eq("_id", key), document, options);
            _seriLogger.LogInformation($"Mongo Cached {typeof(T).Name} with key {key}");
        }
        public async Task RemoveAsync(string key)
        {
            await _cacheCollection.DeleteOneAsync(Builders<BsonDocument>.Filter.Eq("_id", key));
        }
        private void RegisterGenericClassMaps()
        {
            var assembly = Assembly.GetAssembly(typeof(ApplicationUser));
            foreach (var type in assembly.GetTypes().Where(t => t.Namespace == "PCIShield.Domain.Entities"))
            {
                var method = typeof(BsonClassMap).GetMethod("RegisterClassMap", new Type[] { });
                var genericMethod = method.MakeGenericMethod(type);
                genericMethod.Invoke(null, new object[] { new Action<BsonClassMap>(cm =>
                {
                    cm.AutoMap();
                    cm.SetIgnoreExtraElements(true);
                    foreach (var prop in type.GetProperties(BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance))
                    {
                        if (prop.GetCustomAttribute(typeof(BsonElementAttribute)) == null)
                        {
                            cm.MapMember(prop);
                        }
                    }
                })});
            }
        }
        private static void RegisterGenericClassMap<T>() where T : class
        {
            var classMapType = typeof(T);
            if (!BsonClassMap.IsClassMapRegistered(classMapType))
            {
                BsonClassMap.RegisterClassMap<T>(cm =>
                {
                    cm.AutoMap();
                    cm.SetIgnoreExtraElements(true);
                    foreach (var propertyInfo in classMapType.GetProperties())
                    {
                        if (typeof(IEnumerable).IsAssignableFrom(propertyInfo.PropertyType) && propertyInfo.PropertyType != typeof(string))
                        {
                        }
                    }
                });
            }
        }
        private static void RegisterNestedClassMaps<T>(BsonClassMap<T> classMap) where T : class
        {
            foreach (var property in typeof(T).GetProperties())
            {
                var propertyType = property.PropertyType;
                if (typeof(IEnumerable<>).IsAssignableFrom(propertyType) && !BsonClassMap.IsClassMapRegistered(propertyType))
                {
                    BsonClassMap.RegisterClassMap(new BsonClassMap(propertyType));
                }
            }
        }
    }
}
