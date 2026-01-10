using MongoDB.Bson;
using MongoDB.Driver;
using MongoDB.Driver.GridFS;
using PCIShield.Infrastructure.Services;
using System;
using System.Collections.Generic;
using System.IO;
using System.Threading;
using System.Threading.Tasks;
namespace PCIShield.Infrastructure.Services
{
    public interface IGridFsBlobStorageService
    {
        Task<ObjectId> StoreVideoInGridFSAsync(string videoFileName, Stream videoStream = null);
        Task EnsureIndexes();
        Task DeleteBlobAsync(string name);
        Task<ObjectId> SaveBlobAsync(string name, Stream data, string contentType);
        Task<Stream> GetBlobAsync(string name);
        Task<Stream> GetBlobByIdAsync(ObjectId id);
        Task<GridFSFileInfo> GetFileInfoAsync(string name);
        Task<IEnumerable<GridFSFileInfo>> GetAllFileInfosAsync();
        Task DeleteBlobByIdAsync(ObjectId id);
    }
    public class GridFsBlobStorageService : IGridFsBlobStorageService
    {
        private readonly IMongoDatabase _database;
        private readonly IGridFSBucket _gridFSBucket;
        private readonly string _bucketName;
        private readonly int _chunkSize;
        private readonly IAppLoggerService<GridFsBlobStorageService> _logger;
        public GridFsBlobStorageService(
            IAppLoggerService<GridFsBlobStorageService> logger,
            IMongoDatabase database,
            string bucketName = "fs",
            int chunkSize = 255 * 1024
        )
        {
            _database = database;
            _bucketName = bucketName;
            _chunkSize = chunkSize;
            _logger = logger;
            var bucketOptions = new GridFSBucketOptions
            {
                BucketName = _bucketName,
                ChunkSizeBytes = _chunkSize
            };
            _gridFSBucket = new GridFSBucket(_database, bucketOptions);
            EnsureIndexes().GetAwaiter().GetResult();
        }
        public async Task<GridFSFileInfo> GetFileInfoAsync(string name)
        {
            try
            {
                var filter = Builders<GridFSFileInfo>.Filter.Eq(info => info.Filename, name);
                var fileInfo = await _gridFSBucket.Find(filter).FirstOrDefaultAsync();
                return fileInfo;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Failed to retrieve file info for {name}.");
                throw new BlobStorageException("Failed to retrieve file info.", ex);
            }
        }
        public async Task<ObjectId> StoreVideoInGridFSAsync(string videoFileName, Stream videoStream = null)
        {
            if (videoStream == null)
            {
                string contentRootPath = Directory.GetCurrentDirectory();
                string videoFolderPath = Path.Combine(contentRootPath, "YourFolderName");
                string videoFilePath = Path.Combine(videoFolderPath, videoFileName);
                if (!File.Exists(videoFilePath))
                {
                    _logger.LogError($"Video file {videoFileName} not found.");
                    throw new FileNotFoundException($"Video file {videoFileName} not found.");
                }
                using (var fileStream = new FileStream(videoFilePath, FileMode.Open, FileAccess.Read))
                {
                    return await SaveBlobAsync(videoFileName, fileStream, "video/mp4");
                }
            }
            else
            {
                return await SaveBlobAsync(videoFileName, videoStream, "video/mp4");
            }
        }
        public async Task EnsureIndexes()
        {
            try
            {
                var chunksCollectionName = $"{_bucketName}.chunks";
                var filesCollectionName = $"{_bucketName}.files";
                var chunksCollection = _database.GetCollection<BsonDocument>(chunksCollectionName);
                var filesCollection = _database.GetCollection<BsonDocument>(filesCollectionName);
                var indexKeysDefinitionChunks = Builders<BsonDocument>.IndexKeys
                    .Ascending("files_id")
                    .Ascending("n");
                var createIndexOptions = new CreateIndexOptions { Unique = true };
                var indexModelChunks = new CreateIndexModel<BsonDocument>(
                    indexKeysDefinitionChunks,
                    createIndexOptions
                );
                var indexKeysDefinitionFiles = Builders<BsonDocument>.IndexKeys
                    .Ascending("filename")
                    .Ascending("uploadDate");
                var indexModelFiles = new CreateIndexModel<BsonDocument>(indexKeysDefinitionFiles);
                await chunksCollection.Indexes.CreateOneAsync(indexModelChunks);
                await filesCollection.Indexes.CreateOneAsync(indexModelFiles);
            }
            catch (Exception e)
            {
                _logger.LogError(e, "Failed to ensure indexes.");
            }
        }
        public async Task DeleteBlobAsync(string name)
        {
            try
            {
                await _gridFSBucket.DeleteAsync(new ObjectId(name));
            }
            catch (GridFSFileNotFoundException)
            {
                _logger.LogWarning($"Blob {name} not found.");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Failed to delete blob {name}.");
                throw new BlobStorageException($"Failed to delete blob {name}.", ex);
            }
        }
        public async Task<ObjectId> SaveBlobAsync(string name, Stream data, string contentType)
        {
            if (string.IsNullOrWhiteSpace(name))
            {
                throw new ArgumentException("Blob name cannot be null or whitespace.", nameof(name));
            }
            if (data == null || !data.CanRead)
            {
                throw new ArgumentException("Data stream is null or not readable.", nameof(data));
            }
            try
            {
                var options = new GridFSUploadOptions
                {
                    Metadata = new BsonDocument { { "ContentType", contentType ?? "application/octet-stream" } }
                };
                return await _gridFSBucket.UploadFromStreamAsync(name, data, options);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Failed to save blob '{name}'.");
                throw new BlobStorageException("Failed to save blob.", ex);
            }
        }
        public async Task<Stream> GetBlobByIdAsync(ObjectId id)
        {
            try
            {
                return await _gridFSBucket.OpenDownloadStreamAsync(id);
            }
            catch (GridFSFileNotFoundException)
            {
                return null;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Failed to retrieve blob by id {id}.");
                throw new BlobStorageException("Failed to retrieve blob by id.", ex);
            }
        }
        public async Task<Stream> GetBlobAsync(string name)
        {
            try
            {
                return await _gridFSBucket.OpenDownloadStreamByNameAsync(name);
            }
            catch (GridFSFileNotFoundException)
            {
                return null;
            }
            catch (Exception ex)
            {
                var responseBody =  ex.GetBaseException();
                _logger.LogError(ex, $"Failed to retrieve blob {name}.");
                throw new BlobStorageException("Failed to retrieve blob.", ex);
            }
        }
        public async Task<IEnumerable<GridFSFileInfo>> GetAllFileInfosAsync()
        {
            try
            {
                var filter = Builders<GridFSFileInfo>.Filter.Empty;
                var files = new List<GridFSFileInfo>();
                using (var cursor = await _gridFSBucket.FindAsync(filter))
                {
                    await cursor.ForEachAsync(file => files.Add(file));
                }
                return files;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Failed to retrieve all file infos.");
                throw new BlobStorageException("Failed to retrieve all file infos.", ex);
            }
        }
        public async Task DeleteBlobByIdAsync(ObjectId id)
        {
            try
            {
                await _gridFSBucket.DeleteAsync(id);
            }
            catch (GridFSFileNotFoundException)
            {
                _logger.LogWarning($"Blob with id {id} not found.");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Failed to delete blob with id {id}.");
                throw new BlobStorageException($"Failed to delete blob with id {id}.", ex);
            }
        }
        public class BlobStorageException : Exception
        {
            public BlobStorageException(string message, Exception innerException)
                : base(message, innerException) { }
        }
    }
}
