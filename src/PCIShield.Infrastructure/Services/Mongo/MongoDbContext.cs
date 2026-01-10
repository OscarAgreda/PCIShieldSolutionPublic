using System;
using System.Reactive;
using System.Reactive.Linq;
using MongoDB.Bson.Serialization;
using MongoDB.Driver;

using PCIShieldLib.SharedKernel.Interfaces;
namespace PCIShield.Infrastructure.Services
{
    public class MongoDbContext
    {
        private readonly IMongoDatabase _database;
        public MongoDbContext(string connectionString, string databaseName)
        {
            var client = new MongoClient(connectionString);
            _database = client.GetDatabase(databaseName);
            ConfigureMongoCollections();
        }
        private void ConfigureMongoCollections()
        {
        }
        public IMongoCollection<T> GetCollection<T>(string collectionName) where T : class
        {
            return _database.GetCollection<T>(collectionName);
        }
        public static void RegisterGenericClassMap<T>() where T : class
        {
            var classMapType = typeof(T);
            if (!BsonClassMap.IsClassMapRegistered(classMapType))
            {
                BsonClassMap.RegisterClassMap<T>(cm =>
                {
                    cm.AutoMap();
                    cm.SetIgnoreExtraElements(true);
                });
            }
        }
        public IMongoCollection<T> GetCollection<T>()
            where T : class
        {
            return _database.GetCollection<T>(typeof(T).Name);
        }
    }
    public class MongoRepository<T> : IMongoRepository<T>
        where T : class, IMongoAggregateRoot
    {
        private readonly MongoDbContext _context;
        private readonly IMongoCollection<T> _collection;
        public MongoRepository(MongoDbContext context)
        {
            _context = context;
            _collection = _context.GetCollection<T>();
        }
        public IObservable<Unit> CreateAsync(T entity)
        {
            return Observable.FromAsync(() => _collection.InsertOneAsync(entity));
        }
        public IObservable<T> GetByMongoIdAsync(string id)
        {
            var filter = Builders<T>.Filter.Eq("_id", id);
            return Observable.FromAsync(() => _collection.Find(filter).FirstOrDefaultAsync());
        }
        public IObservable<T> GetByCustomerIdAsync(string id)
        {
            var filter = Builders<T>.Filter.Eq("CustomerId", id);
            return Observable.FromAsync(() => _collection.Find(filter).FirstOrDefaultAsync());
        }
        public IObservable<ReplaceOneResult> UpdateByMongoIdAsync(string id, T entity)
        {
            var filter = Builders<T>.Filter.Eq("_id", id);
            return Observable.FromAsync(() => _collection.ReplaceOneAsync(filter, entity));
        }
        public IObservable<ReplaceOneResult> UpdateByCustomerIdAsync(string id, T entity)
        {
            var filter = Builders<T>.Filter.Eq("CustomerId", id);
            return Observable.FromAsync(() => _collection.ReplaceOneAsync(filter, entity));
        }
        public IObservable<DeleteResult> DeleteAsync(string id)
        {
            var filter = Builders<T>.Filter.Eq("CustomerId", id);
            return Observable.FromAsync(() => _collection.DeleteOneAsync(filter));
        }
    }
    public interface IMongoRepository<T>
        where T : class
    {
        IObservable<Unit> CreateAsync(T entity);
        IObservable<T> GetByMongoIdAsync(string id);
        IObservable<T> GetByCustomerIdAsync(string id);
        IObservable<ReplaceOneResult> UpdateByMongoIdAsync(string id, T entity);
        IObservable<ReplaceOneResult> UpdateByCustomerIdAsync(string id, T entity);
        IObservable<DeleteResult> DeleteAsync(string id);
    }
}
