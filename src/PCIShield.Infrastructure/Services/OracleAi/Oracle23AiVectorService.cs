/*
public class ProductService
{
    private readonly IOracle23AiVectorService _vectorService;

    public async Task<List<ProductDto>> FindSimilarProducts(string query)
    {
        var results = await _vectorService.TopKSearchAsync(
            schemaName: "MYAPP",
            baseTableName: "Products",
            idColumnName: "ProductId",
            labelColumnName: "Title",
            vectorTableName: "Products_Vectors",
            vectorColumnName: "Embedding_Description",
            modelNameInDb: "ALL_MINILM_L12_V2",
            queryText: query,
            topK: 10,
            targetAccuracy: 90,
            CancellationToken.None
        );

        return results.Match(
            Right: items => items.Select(i => new ProductDto 
            { 
                Id = i.id, 
                Title = i.label, 
                Similarity = 1.0 - (i.distance / 2.0)
            }).ToList(),
            Left: error => throw new Exception(error)
        );
    }
}
 
 */
using System;
using System;
using System.Collections.Generic;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Linq;
using System.Reactive.Linq;
using System.Reactive.Subjects;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading;
using System.Threading.Tasks;
using System.Threading.Tasks;

using LanguageExt;

using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Diagnostics.HealthChecks;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

using Oracle.ManagedDataAccess.Client;
using Oracle.ManagedDataAccess.Types;

using static LanguageExt.Prelude;
namespace PCIShield.Infrastructure.Services.OracleAi
{
    public abstract record DbError
    {
        public sealed record Timeout(string Message) : DbError;
        public sealed record Connectivity(string Message) : DbError;
        public sealed record SqlSyntax(string Message) : DbError;
        public sealed record Privilege(string Message) : DbError;
        public sealed record Unknown(string Message, Exception Exception) : DbError;

        public static DbError FromException(Exception ex)
        {
            if (ex is OracleException oex)
            {
                return oex.Number switch
                {
                    3113 or 3114 => new Connectivity($"Connection lost: {oex.Message}"),
                    1017 or 1005 or 1031 => new Privilege($"Auth/privilege error: {oex.Message}"),
                    900 or 904 or 942 or 6550 => new SqlSyntax($"SQL/plsql error: {oex.Message}"),
                    _ => new Unknown(oex.Message, oex)
                };

            }
            return new Unknown(ex.Message, ex);
        }

        public override string ToString() => this switch
        {
            Timeout t => $"Timeout: {t.Message}",
            Connectivity c => $"Connectivity: {c.Message}",
            SqlSyntax s => $"SQL Syntax: {s.Message}",
            Privilege p => $"Privilege: {p.Message}",
            Unknown u => $"Unknown: {u.Message}",
            _ => "Unknown error"
        };
    }

    public interface IOracle23AiVectorService
    {
        Task<Either<DbError, Unit>> UpsertEmbeddingsAsync(
            string schemaName, string vectorTableName, string idColumnName, long id,
            IReadOnlyDictionary<string, float[]> embeddings, CancellationToken ct);
        Task<Either<DbError, Unit>> UpsertEmbeddingsBatchAsync(
      string schemaName, string vectorTableName, string idColumnName,
      long[] ids, IReadOnlyDictionary<string, float[][]> batchEmbeddings, CancellationToken ct);
        Task<Either<DbError, int>> BackfillEmbeddingsAsync(
                        string schemaName,
                        string baseTableName,
                        string idColumnName,
                        string[] textColumnNames,
                        string vectorTableName,
                        string modelNameInDb,
                        int batchSize,
                        CancellationToken ct);
        Task<Either<DbError, IReadOnlyList<(long id, string label, double distance)>>> TopKSearchAsync(
            string schemaName, string baseTableName, string idColumnName, string labelColumnName,
            string vectorTableName, string vectorColumnName, string modelNameInDb,
            string queryText, int topK, int targetAccuracy, CancellationToken ct);

        Task<Either<DbError, IReadOnlyList<(long id, string label, double distance)>>> TopKSearchByVectorAsync(
            string schemaName, string baseTableName, string idColumnName, string labelColumnName,
            string vectorTableName, string vectorColumnName, float[] queryVector,
            int topK, int targetAccuracy, CancellationToken ct);

        Task<Either<DbError, IReadOnlyList<(long id, string label, double distance)>>> HybridSearchAsync(
            string schemaName, string baseTableName, string idColumnName, string labelColumnName,
            string filterClause, IReadOnlyDictionary<string, object> filterParameters,
            string vectorTableName, string vectorColumnName, string modelNameInDb,
            string queryText, int topK, int targetAccuracy, CancellationToken ct);

        Task<Either<DbError, IReadOnlyList<(long id, string label, double distance)>>> HybridSearchPrefilterAsync(
            string schemaName, string baseTableName, string idColumnName, string labelColumnName,
            string filterClause, IReadOnlyDictionary<string, object> filterParameters,
            string vectorTableName, string vectorColumnName, string modelNameInDb,
            string queryText, int topK, int targetAccuracy, CancellationToken ct);

        Task<Either<DbError, IReadOnlyList<(long docId, int chunkId, string chunkText, double distance)>>> MultiVectorTopPerDocAsync(
            string schemaName, string chunkTableName, string docIdColumn, string chunkIdColumn,
            string chunkTextColumn, string vectorColumn, float[] queryVector,
            int chunksPerDoc, int topDocs, int targetAccuracy, CancellationToken ct);

        Task<(int Dimensions, string DType)> GetModelInfoAsync(string modelName, CancellationToken ct);
    }

    public sealed class Oracle23AiVectorService : IOracle23AiVectorService
    {
        private readonly string _connectionString;
        private readonly Microsoft.Extensions.Logging.ILogger<Oracle23AiVectorService> _logger;
        private static readonly Regex IdentifierRegex = new(@"^[A-Za-z0-9_#$]+$", RegexOptions.Compiled);

        public Oracle23AiVectorService(string connectionString, Microsoft.Extensions.Logging.ILogger<Oracle23AiVectorService> logger)
        {
            _connectionString = connectionString ?? throw new ArgumentNullException(nameof(connectionString));
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        }

        private static void GuardIdentifier(string name, string paramName)
        {
            if (string.IsNullOrWhiteSpace(name) || !IdentifierRegex.IsMatch(name))
                throw new ArgumentException($"Invalid SQL identifier: {name}", paramName);
        }

        private static ulong HashQuery(string text)
        {
            unchecked
            {
                ulong hash = 14695981039346656037UL;
                foreach (var c in text)
                {
                    hash ^= c;
                    hash *= 1099511628211UL;
                }
                return hash;
            }
        }

        public async Task<Either<DbError, Unit>> UpsertEmbeddingsAsync(
            string schemaName, string vectorTableName, string idColumnName, long id,
            IReadOnlyDictionary<string, float[]> embeddings, CancellationToken ct)
        {
            try
            {
                GuardIdentifier(schemaName, nameof(schemaName));
                GuardIdentifier(vectorTableName, nameof(vectorTableName));
                GuardIdentifier(idColumnName, nameof(idColumnName));
                foreach (var col in embeddings.Keys) GuardIdentifier(col, nameof(embeddings));

                if (!embeddings.Any()) return Unit.Default;

                var columns = string.Join(", ", embeddings.Keys);
                var sourceSelects = string.Join(", ", embeddings.Keys.Select(k => $":{k} AS {k}"));
                var updateSetters = string.Join(", ", embeddings.Keys.Select(k => $"{k} = s.{k}"));
                var insertValues = string.Join(", ", embeddings.Keys.Select(k => $"s.{k}"));

                var sql = $@"
MERGE INTO {schemaName}.{vectorTableName} t
USING (SELECT :id AS {idColumnName}, {sourceSelects} FROM dual) s
ON (t.{idColumnName} = s.{idColumnName})
WHEN MATCHED THEN UPDATE SET {updateSetters}
WHEN NOT MATCHED THEN INSERT ({idColumnName}, {columns})
VALUES (s.{idColumnName}, {insertValues})";

                await using var connection = new OracleConnection(_connectionString);
                await connection.OpenAsync(ct);
                await using var cmd = new OracleCommand(sql, connection) { BindByName = true };

                cmd.Parameters.Add("id", OracleDbType.Int64).Value = id;
                foreach (var (colName, vector) in embeddings)
                {
                    cmd.Parameters.Add(colName, OracleDbType.Vector).Value = new OracleVector(vector);
                }

                await cmd.ExecuteNonQueryAsync(ct);
                _logger.LogDebug("Upserted embeddings for ID {Id} into {TableName}", id, vectorTableName);
                return Unit.Default;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Failed to upsert embeddings for ID {Id} into {TableName}", id, vectorTableName);
                return DbError.FromException(ex);
            }
        }
        public async Task<Either<DbError, int>> BackfillEmbeddingsAsync(
    string schemaName,
    string baseTableName,
    string idColumnName,
    string[] textColumnNames,
    string vectorTableName,
    string modelNameInDb,
    int batchSize,
    CancellationToken ct)
        {
            try
            {
                GuardIdentifier(schemaName, nameof(schemaName));
                GuardIdentifier(baseTableName, nameof(baseTableName));
                GuardIdentifier(idColumnName, nameof(idColumnName));
                GuardIdentifier(vectorTableName, nameof(vectorTableName));
                if (textColumnNames is null || textColumnNames.Length == 0)
                    return new DbError.SqlSyntax("At least one text column is required");

                foreach (var c in textColumnNames) GuardIdentifier(c, nameof(textColumnNames));
                var embedCols = textColumnNames.Select(c => $"EMBED_{c.ToUpperInvariant()}").ToArray();
                foreach (var ec in embedCols) GuardIdentifier(ec, "embedCols");
                await using var con = new OracleConnection(_connectionString);
                await con.OpenAsync(ct);

                int processed = 0;
                while (true)
                {
                    var needsSql = $@"
SELECT b.{idColumnName}
FROM   {schemaName}.{baseTableName} b
LEFT JOIN {schemaName}.{vectorTableName} v ON v.{idColumnName} = b.{idColumnName}
WHERE  {string.Join(" OR ",
                    embedCols.Select(ec => $"v.{ec} IS NULL"))}
FETCH FIRST :take ROWS ONLY";

                    var ids = new List<long>();
                    await using (var pick = new OracleCommand(needsSql, con) { BindByName = true })
                    {
                        pick.Parameters.Add("take", OracleDbType.Int32).Value = batchSize;
                        await using var rdr = await pick.ExecuteReaderAsync(ct);
                        while (await rdr.ReadAsync(ct)) ids.Add(rdr.GetInt64(0));
                    }
                    if (ids.Count == 0) break;
                    var selectCols = string.Join(", ", textColumnNames.Select(c =>
                        $"VECTOR_EMBEDDING(:m USING b.{c} AS data) AS EMBED_{c.ToUpperInvariant()}"));

                    var setCols = string.Join(", ", embedCols.Select(ec => $"{ec} = s.{ec}"));
                    var insertCols = string.Join(", ", new[] { idColumnName }.Concat(embedCols));
                    var insertVals = string.Join(", ", new[] { $"s.{idColumnName}" }.Concat(embedCols.Select(ec => $"s.{ec}")));

                    var mergeSql = $@"
MERGE INTO {schemaName}.{vectorTableName} t
USING (
  SELECT b.{idColumnName}, {selectCols}
  FROM   {schemaName}.{baseTableName} b
  WHERE  b.{idColumnName} IN ({string.Join(",", ids)})
) s
ON (t.{idColumnName} = s.{idColumnName})
WHEN MATCHED THEN UPDATE SET {setCols}
WHEN NOT MATCHED THEN INSERT ({insertCols}) VALUES ({insertVals})";

                    await using (var cmd = new OracleCommand(mergeSql, con) { BindByName = true })
                    {
                        cmd.Parameters.Add("m", OracleDbType.Varchar2).Value = modelNameInDb;
                        var affected = await cmd.ExecuteNonQueryAsync(ct);
                        processed += ids.Count;
                    }
                }

                _logger.LogInformation("Backfill completed for {Base} -> {Vector}. Rows processed: {Count}",
                    baseTableName, vectorTableName, processed);
                return processed;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "BackfillEmbeddingsAsync failed");
                return DbError.FromException(ex);
            }
        }

        public async Task<Either<DbError, Unit>> UpsertEmbeddingsBatchAsync(
            string schemaName, string vectorTableName, string idColumnName,
            long[] ids, IReadOnlyDictionary<string, float[][]> batchEmbeddings, CancellationToken ct)
        {
            try
            {
                GuardIdentifier(schemaName, nameof(schemaName));
                GuardIdentifier(vectorTableName, nameof(vectorTableName));
                GuardIdentifier(idColumnName, nameof(idColumnName));
                foreach (var col in batchEmbeddings.Keys) GuardIdentifier(col, nameof(batchEmbeddings));

                if (ids.Length == 0 || !batchEmbeddings.Any()) return Unit.Default;

                var columns = string.Join(", ", batchEmbeddings.Keys);
                var sourceSelects = string.Join(", ", batchEmbeddings.Keys.Select(k => $":{k} AS {k}"));
                var updateSetters = string.Join(", ", batchEmbeddings.Keys.Select(k => $"{k} = s.{k}"));
                var insertValues = string.Join(", ", batchEmbeddings.Keys.Select(k => $"s.{k}"));

                var sql = $@"
MERGE INTO {schemaName}.{vectorTableName} t
USING (SELECT :id AS {idColumnName}, {sourceSelects} FROM dual) s
ON (t.{idColumnName} = s.{idColumnName})
WHEN MATCHED THEN UPDATE SET {updateSetters}
WHEN NOT MATCHED THEN INSERT ({idColumnName}, {columns})
VALUES (s.{idColumnName}, {insertValues})";

                await using var connection = new OracleConnection(_connectionString);
                await connection.OpenAsync(ct);
                await using var cmd = new OracleCommand(sql, connection) { BindByName = true };

                cmd.ArrayBindCount = ids.Length;
                var pId = cmd.Parameters.Add("id", OracleDbType.Int64);
                pId.Value = ids;

                foreach (var (name, vectors) in batchEmbeddings)
                {
                    var oracleVectors = vectors.Select(v => new OracleVector(v)).ToArray();
                    cmd.Parameters.Add(name, OracleDbType.Vector).Value = oracleVectors;
                }

                await cmd.ExecuteNonQueryAsync(ct);
                _logger.LogInformation("Batch upserted {Count} embeddings into {TableName}", ids.Length, vectorTableName);
                return Unit.Default;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Failed batch upsert into {TableName}", vectorTableName);
                return DbError.FromException(ex);
            }
        }
        private static OracleParameter AddParam(OracleCommand cmd, string name, object value)
        {
            var p = cmd.Parameters.Add(name, value switch
            {
                int => OracleDbType.Int32,
                long => OracleDbType.Int64,
                bool => OracleDbType.Int16,
                decimal => OracleDbType.Decimal,
                double => OracleDbType.Double,
                float => OracleDbType.Single,
                DateTime => OracleDbType.TimeStamp,
                byte[] => OracleDbType.Blob,
                _ => OracleDbType.NVarchar2
            });
            p.Value = value;
            return p;
        }
        public async Task<Either<DbError, IReadOnlyList<(long id, string label, double distance)>>> TopKSearchAsync(
            string schemaName, string baseTableName, string idColumnName, string labelColumnName,
            string vectorTableName, string vectorColumnName, string modelNameInDb,
            string queryText, int topK, int targetAccuracy, CancellationToken ct)
        {
            try
            {
                GuardIdentifier(schemaName, nameof(schemaName));
                GuardIdentifier(baseTableName, nameof(baseTableName));
                GuardIdentifier(vectorTableName, nameof(vectorTableName));
                GuardIdentifier(idColumnName, nameof(idColumnName));
                GuardIdentifier(labelColumnName, nameof(labelColumnName));
                GuardIdentifier(vectorColumnName, nameof(vectorColumnName));

                if (topK < 1 || topK > 1000) return new DbError.SqlSyntax("topK must be between 1 and 1000");
                if (targetAccuracy < 0 || targetAccuracy > 100) return new DbError.SqlSyntax("targetAccuracy must be between 0 and 100");

                var queryHash = HashQuery(queryText);
                _logger.LogDebug("Top-K search: model={Model}, k={K}, acc={Acc}, queryHash={Hash}",
                    modelNameInDb, topK, targetAccuracy, queryHash);

                var sql = $@"
WITH scored AS (
  SELECT v.{idColumnName} AS id,
         VECTOR_DISTANCE(v.{vectorColumnName}, VECTOR_EMBEDDING(:model_name USING :query_text AS data), COSINE) AS dist
  FROM {schemaName}.{vectorTableName} v
  ORDER BY dist
  FETCH APPROX FIRST {topK} ROWS ONLY WITH TARGET ACCURACY {targetAccuracy}
)
SELECT b.{idColumnName}, b.{labelColumnName}, s.dist
FROM {schemaName}.{baseTableName} b
JOIN scored s ON s.id = b.{idColumnName}
ORDER BY s.dist";

                await using var connection = new OracleConnection(_connectionString);
                await connection.OpenAsync(ct);
                await using var cmd = new OracleCommand(sql, connection) { BindByName = true };

                cmd.Parameters.Add("model_name", OracleDbType.Varchar2).Value = modelNameInDb;
                cmd.Parameters.Add("query_text", OracleDbType.NVarchar2).Value = queryText;

                var results = new List<(long, string, double)>();
                await using var reader = await cmd.ExecuteReaderAsync(ct);
                while (await reader.ReadAsync(ct))
                {
                    results.Add((reader.GetInt64(0), reader.GetString(1), reader.GetDouble(2)));
                }

                _logger.LogDebug("Top-K search returned {Count} results", results.Count);
                return Right<DbError, IReadOnlyList<(long, string, double)>>(results.AsReadOnly());
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Failed Top-K vector search");
                return DbError.FromException(ex);
            }
        }

        public async Task<Either<DbError, IReadOnlyList<(long id, string label, double distance)>>> TopKSearchByVectorAsync(
            string schemaName, string baseTableName, string idColumnName, string labelColumnName,
            string vectorTableName, string vectorColumnName, float[] queryVector,
            int topK, int targetAccuracy, CancellationToken ct)
        {
            try
            {
                GuardIdentifier(schemaName, nameof(schemaName));
                GuardIdentifier(baseTableName, nameof(baseTableName));
                GuardIdentifier(vectorTableName, nameof(vectorTableName));
                GuardIdentifier(idColumnName, nameof(idColumnName));
                GuardIdentifier(labelColumnName, nameof(labelColumnName));
                GuardIdentifier(vectorColumnName, nameof(vectorColumnName));

                if (topK < 1 || topK > 1000) return new DbError.SqlSyntax("topK must be between 1 and 1000");
                if (targetAccuracy < 0 || targetAccuracy > 100) return new DbError.SqlSyntax("targetAccuracy must be between 0 and 100");

                _logger.LogDebug("Top-K search by vector: dims={Dims}, k={K}, acc={Acc}",
                    queryVector.Length, topK, targetAccuracy);

                var sql = $@"
WITH scored AS (
  SELECT v.{idColumnName} AS id,
         VECTOR_DISTANCE(v.{vectorColumnName}, :qvec, COSINE) AS dist
  FROM {schemaName}.{vectorTableName} v
  ORDER BY dist
  FETCH APPROX FIRST {topK} ROWS ONLY WITH TARGET ACCURACY {targetAccuracy}
)
SELECT b.{idColumnName}, b.{labelColumnName}, s.dist
FROM {schemaName}.{baseTableName} b
JOIN scored s ON s.id = b.{idColumnName}
ORDER BY s.dist";

                await using var connection = new OracleConnection(_connectionString);
                await connection.OpenAsync(ct);
                await using var cmd = new OracleCommand(sql, connection) { BindByName = true };

                cmd.Parameters.Add("qvec", OracleDbType.Vector).Value = new OracleVector(queryVector);

                var results = new List<(long, string, double)>();
                await using var reader = await cmd.ExecuteReaderAsync(ct);
                while (await reader.ReadAsync(ct))
                {
                    results.Add((reader.GetInt64(0), reader.GetString(1), reader.GetDouble(2)));
                }

                _logger.LogDebug("Top-K search by vector returned {Count} results", results.Count);
                return Right<DbError, IReadOnlyList<(long, string, double)>>(results.AsReadOnly());
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Failed Top-K vector search by vector");
                return DbError.FromException(ex);
            }
        }

        public async Task<Either<DbError, IReadOnlyList<(long, string, double)>>> HybridSearchAsync(
            string schemaName, string baseTableName, string idColumnName, string labelColumnName,
            string filterClause, IReadOnlyDictionary<string, object> filterParameters,
            string vectorTableName, string vectorColumnName, string modelNameInDb,
            string queryText, int topK, int targetAccuracy, CancellationToken ct)
        {
            try
            {
                GuardIdentifier(schemaName, nameof(schemaName));
                GuardIdentifier(baseTableName, nameof(baseTableName));
                GuardIdentifier(vectorTableName, nameof(vectorTableName));
                GuardIdentifier(idColumnName, nameof(idColumnName));
                GuardIdentifier(labelColumnName, nameof(labelColumnName));
                GuardIdentifier(vectorColumnName, nameof(vectorColumnName));

                if (string.IsNullOrWhiteSpace(filterClause)) return new DbError.SqlSyntax("filterClause cannot be empty");
                if (topK < 1 || topK > 1000) return new DbError.SqlSyntax("topK must be between 1 and 1000");
                if (targetAccuracy < 0 || targetAccuracy > 100) return new DbError.SqlSyntax("targetAccuracy must be between 0 and 100");

                var queryHash = HashQuery(queryText);
                _logger.LogDebug("Hybrid search: model={Model}, k={K}, acc={Acc}, queryHash={Hash}",
                    modelNameInDb, topK, targetAccuracy, queryHash);

                var sql = $@"
WITH scored AS (
  SELECT v.{idColumnName} AS id,
         VECTOR_DISTANCE(v.{vectorColumnName}, VECTOR_EMBEDDING(:model_name USING :query_text AS data), COSINE) AS dist
  FROM {schemaName}.{vectorTableName} v
  ORDER BY dist
  FETCH APPROX FIRST {topK} ROWS ONLY WITH TARGET ACCURACY {targetAccuracy}
)
SELECT b.{idColumnName}, b.{labelColumnName}, s.dist
FROM {schemaName}.{baseTableName} b
JOIN scored s ON s.id = b.{idColumnName}
WHERE {filterClause}
ORDER BY s.dist";

                await using var connection = new OracleConnection(_connectionString);
                await connection.OpenAsync(ct);
                await using var cmd = new OracleCommand(sql, connection) { BindByName = true };

                cmd.Parameters.Add("model_name", OracleDbType.Varchar2).Value = modelNameInDb;
                cmd.Parameters.Add("query_text", OracleDbType.NVarchar2).Value = queryText;

                if (filterParameters != null)
                {
                    foreach (var (key, value) in filterParameters)
                    {
                        cmd.Parameters.Add(key, value);
                    }
                }

                var results = new List<(long, string, double)>();
                await using var reader = await cmd.ExecuteReaderAsync(ct);
                while (await reader.ReadAsync(ct))
                {
                    results.Add((reader.GetInt64(0), reader.GetString(1), reader.GetDouble(2)));
                }

                _logger.LogDebug("Hybrid search returned {Count} results", results.Count);
                return Right<DbError, IReadOnlyList<(long, string, double)>>(results.AsReadOnly());
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Failed hybrid search");
                return DbError.FromException(ex);
            }
        }

        public async Task<Either<DbError, IReadOnlyList<(long, string, double)>>> HybridSearchPrefilterAsync(
            string schemaName, string baseTableName, string idColumnName, string labelColumnName,
            string filterClause, IReadOnlyDictionary<string, object> filterParameters,
            string vectorTableName, string vectorColumnName, string modelNameInDb,
            string queryText, int topK, int targetAccuracy, CancellationToken ct)
        {
            try
            {
                GuardIdentifier(schemaName, nameof(schemaName));
                GuardIdentifier(baseTableName, nameof(baseTableName));
                GuardIdentifier(vectorTableName, nameof(vectorTableName));
                GuardIdentifier(idColumnName, nameof(idColumnName));
                GuardIdentifier(labelColumnName, nameof(labelColumnName));
                GuardIdentifier(vectorColumnName, nameof(vectorColumnName));

                if (string.IsNullOrWhiteSpace(filterClause)) return new DbError.SqlSyntax("filterClause cannot be empty");
                if (topK < 1 || topK > 1000) return new DbError.SqlSyntax("topK must be between 1 and 1000");
                if (targetAccuracy < 0 || targetAccuracy > 100) return new DbError.SqlSyntax("targetAccuracy must be between 0 and 100");

                var queryHash = HashQuery(queryText);
                _logger.LogDebug("Hybrid prefilter search: model={Model}, k={K}, acc={Acc}, queryHash={Hash}",
                    modelNameInDb, topK, targetAccuracy, queryHash);
                var sql = $@"
WITH filtered AS (
  SELECT b.{idColumnName}
  FROM {schemaName}.{baseTableName} b
  WHERE {filterClause}
),
scored AS (
  SELECT v.{idColumnName} AS id,
         VECTOR_DISTANCE(v.{vectorColumnName}, VECTOR_EMBEDDING(:model_name USING :query_text AS data), COSINE) AS dist
  FROM {schemaName}.{vectorTableName} v
  JOIN filtered f ON f.{idColumnName} = v.{idColumnName}
  ORDER BY dist
  FETCH APPROX FIRST {topK} ROWS ONLY WITH TARGET ACCURACY {targetAccuracy}
)
SELECT b.{idColumnName}, b.{labelColumnName}, s.dist
FROM {schemaName}.{baseTableName} b 
JOIN scored s ON b.{idColumnName} = s.id
ORDER BY s.dist";

                await using var connection = new OracleConnection(_connectionString);
                await connection.OpenAsync(ct);
                await using var cmd = new OracleCommand(sql, connection) { BindByName = true };

                cmd.Parameters.Add("model_name", OracleDbType.Varchar2).Value = modelNameInDb;
                cmd.Parameters.Add("query_text", OracleDbType.NVarchar2).Value = queryText;

                if (filterParameters != null)
                {
                    foreach (var (key, value) in filterParameters)
                    {
                        cmd.Parameters.Add(key, value);
                    }
                }

                var results = new List<(long, string, double)>();
                await using var reader = await cmd.ExecuteReaderAsync(ct);
                while (await reader.ReadAsync(ct))
                {
                    results.Add((reader.GetInt64(0), reader.GetString(1), reader.GetDouble(2)));
                }

                _logger.LogDebug("Hybrid prefilter search returned {Count} results", results.Count);
                return Right<DbError, IReadOnlyList<(long, string, double)>>(results.AsReadOnly());
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Failed hybrid prefilter search");
                return DbError.FromException(ex);
            }
        }

        public async Task<Either<DbError, IReadOnlyList<(long docId, int chunkId, string chunkText, double distance)>>> MultiVectorTopPerDocAsync(
            string schemaName, string chunkTableName, string docIdColumn, string chunkIdColumn,
            string chunkTextColumn, string vectorColumn, float[] queryVector,
            int chunksPerDoc, int topDocs, int targetAccuracy, CancellationToken ct)
        {
            try
            {
                GuardIdentifier(schemaName, nameof(schemaName));
                GuardIdentifier(chunkTableName, nameof(chunkTableName));
                GuardIdentifier(docIdColumn, nameof(docIdColumn));
                GuardIdentifier(chunkIdColumn, nameof(chunkIdColumn));
                GuardIdentifier(chunkTextColumn, nameof(chunkTextColumn));
                GuardIdentifier(vectorColumn, nameof(vectorColumn));

                if (chunksPerDoc < 1) return new DbError.SqlSyntax("chunksPerDoc must be >= 1");
                if (topDocs < 1 || topDocs > 1000) return new DbError.SqlSyntax("topDocs must be between 1 and 1000");
                if (targetAccuracy < 0 || targetAccuracy > 100) return new DbError.SqlSyntax("targetAccuracy must be between 0 and 100");

                _logger.LogDebug("Multi-vector search: chunks={Chunks}, docs={Docs}, acc={Acc}",
                    chunksPerDoc, topDocs, targetAccuracy);

                var sql = $@"
SELECT {docIdColumn}, {chunkIdColumn}, {chunkTextColumn}, dist
FROM (
  SELECT c.{docIdColumn}, c.{chunkIdColumn}, c.{chunkTextColumn},
         VECTOR_DISTANCE(c.{vectorColumn}, :qvec, COSINE) AS dist,
         ROW_NUMBER() OVER (PARTITION BY c.{docIdColumn} 
                           ORDER BY VECTOR_DISTANCE(c.{vectorColumn}, :qvec, COSINE)) AS rn
  FROM {schemaName}.{chunkTableName} c
)
WHERE rn <= {chunksPerDoc}
ORDER BY dist
FETCH APPROX FIRST {topDocs} ROWS ONLY WITH TARGET ACCURACY {targetAccuracy}";

                await using var connection = new OracleConnection(_connectionString);
                await connection.OpenAsync(ct);
                await using var cmd = new OracleCommand(sql, connection) { BindByName = true };

                cmd.Parameters.Add("qvec", OracleDbType.Vector).Value = new OracleVector(queryVector);

                var results = new List<(long, int, string, double)>();
                await using var reader = await cmd.ExecuteReaderAsync(ct);
                while (await reader.ReadAsync(ct))
                {
                    results.Add((reader.GetInt64(0), reader.GetInt32(1), reader.GetString(2), reader.GetDouble(3)));
                }

                _logger.LogDebug("Multi-vector search returned {Count} chunk results", results.Count);
                return Right<DbError, IReadOnlyList<(long, int, string, double)>>(results.AsReadOnly());
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Failed multi-vector search");
                return DbError.FromException(ex);
            }
        }

        public async Task<(int Dimensions, string DType)> GetModelInfoAsync(string modelName, CancellationToken ct)
        {
            const string sql = "SELECT m.dimension_count, m.element_type FROM TABLE(DBMS_VECTOR.GET_MODEL_INFO(:model_name)) m";

            await using var connection = new OracleConnection(_connectionString);
            await connection.OpenAsync(ct);
            await using var cmd = new OracleCommand(sql, connection) { BindByName = true };
            cmd.Parameters.Add("model_name", OracleDbType.Varchar2).Value = modelName;

            await using var reader = await cmd.ExecuteReaderAsync(ct);
            if (await reader.ReadAsync(ct))
            {
                return (reader.GetInt32(0), reader.GetString(1));
            }

            throw new InvalidOperationException($"Model {modelName} not found in database");
        }
    }

    public sealed class Oracle23AiBootstrapper
    {
        private readonly string _connectionString;
        private readonly Microsoft.Extensions.Logging.ILogger<Oracle23AiBootstrapper> _logger;

        public Oracle23AiBootstrapper(string connectionString, Microsoft.Extensions.Logging.ILogger<Oracle23AiBootstrapper> logger)
        {
            _connectionString = connectionString ?? throw new ArgumentNullException(nameof(connectionString));
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        }

        public async Task EnsureModelLoadedAsync(
            string directoryPathOnServer,
            string onnxFileName,
            string modelNameInDb,
            int expectedDimensions,
            CancellationToken ct)
        {
            const string OracleDirectoryName = "ONNX_MODEL_DIR";

            const string ensureDirSql = @"
DECLARE
  v_count INT;
BEGIN
  SELECT COUNT(*) INTO v_count FROM ALL_DIRECTORIES WHERE DIRECTORY_NAME = :dir_name;
  IF v_count = 0 THEN
    EXECUTE IMMEDIATE 'CREATE OR REPLACE DIRECTORY ' || :dir_name || ' AS ''' || :dir_path || '''';
    DBMS_OUTPUT.PUT_LINE('Directory created: ' || :dir_name);
  END IF;
END;";

            const string loadModelSql = @"
BEGIN
  BEGIN
    DBMS_VECTOR.DROP_ONNX_MODEL(model_name => :model_name, force => TRUE);
  EXCEPTION
    WHEN OTHERS THEN
      IF SQLCODE = -47410 THEN NULL; -- Model doesn't exist, OK
      ELSE RAISE;
      END IF;
  END;
  
  DBMS_VECTOR.LOAD_ONNX_MODEL(
    directory_name => :dir_name,
    file_name => :file_name,
    model_name => :model_name
  );
  DBMS_OUTPUT.PUT_LINE('Model loaded: ' || :model_name);
END;";

            _logger.LogInformation("Loading ONNX model '{ModelName}' into Oracle DB", modelNameInDb);

            await using var connection = new OracleConnection(_connectionString);
            await connection.OpenAsync(ct);
            _logger.LogDebug("Ensuring directory '{DirName}' -> '{Path}'", OracleDirectoryName, directoryPathOnServer);
            await using (var cmd = new OracleCommand(ensureDirSql, connection) { BindByName = true })
            {
                cmd.Parameters.Add("dir_name", OracleDbType.Varchar2).Value = OracleDirectoryName;
                cmd.Parameters.Add("dir_path", OracleDbType.Varchar2).Value = directoryPathOnServer;
                await cmd.ExecuteNonQueryAsync(ct);
            }
            _logger.LogDebug("Loading ONNX file '{File}' as '{ModelName}'", onnxFileName, modelNameInDb);
            await using (var cmd = new OracleCommand(loadModelSql, connection) { BindByName = true })
            {
                cmd.Parameters.Add("model_name", OracleDbType.Varchar2).Value = modelNameInDb;
                cmd.Parameters.Add("dir_name", OracleDbType.Varchar2).Value = OracleDirectoryName;
                cmd.Parameters.Add("file_name", OracleDbType.Varchar2).Value = onnxFileName;
                await cmd.ExecuteNonQueryAsync(ct);
            }
            _logger.LogDebug("Validating model dimensions");
            const string checkSql = "SELECT m.dimension_count FROM TABLE(DBMS_VECTOR.GET_MODEL_INFO(:m)) m";
            await using (var cmd = new OracleCommand(checkSql, connection) { BindByName = true })
            {
                cmd.Parameters.Add("m", OracleDbType.Varchar2).Value = modelNameInDb;
                var actualDimensions = Convert.ToInt32(await cmd.ExecuteScalarAsync(ct));

                if (actualDimensions != expectedDimensions)
                {
                    var msg = $"Model dimension mismatch! Expected {expectedDimensions}, got {actualDimensions}. " +
                             $"Update your vector table DDL to match.";
                    _logger.LogCritical(msg);
                    throw new InvalidOperationException(msg);
                }
            }

            _logger.LogInformation("Successfully loaded and validated ONNX model '{ModelName}' ({Dims} dimensions)",
                modelNameInDb, expectedDimensions);
        }
    }
    public static class RankFusion
    {
        public static IReadOnlyList<(long id, double score)> RrfFuse(
            IEnumerable<long> vectorOrder,
            IEnumerable<long> lexicalOrder,
            int k = 60)
        {
            var scores = new Dictionary<long, double>();
            int rank = 0;
            foreach (var id in vectorOrder)
            {
                scores[id] = scores.GetValueOrDefault(id) + 1.0 / (k + (++rank));
            }
            rank = 0;
            foreach (var id in lexicalOrder)
            {
                scores[id] = scores.GetValueOrDefault(id) + 1.0 / (k + (++rank));
            }

            return scores
                .OrderByDescending(x => x.Value)
                .Select(x => (x.Key, x.Value))
                .ToList()
                .AsReadOnly();
        }
        public static IReadOnlyList<(long id, double score)> WeightedFuse(
            IEnumerable<long> vectorOrder,
            IEnumerable<long> lexicalOrder,
            double vectorWeight = 0.7,
            double lexicalWeight = 0.3,
            int k = 60)
        {
            var scores = new Dictionary<long, double>();

            int rank = 0;
            foreach (var id in vectorOrder)
            {
                scores[id] = scores.GetValueOrDefault(id) + vectorWeight / (k + (++rank));
            }

            rank = 0;
            foreach (var id in lexicalOrder)
            {
                scores[id] = scores.GetValueOrDefault(id) + lexicalWeight / (k + (++rank));
            }

            return scores
                .OrderByDescending(x => x.Value)
                .Select(x => (x.Key, x.Value))
                .ToList()
                .AsReadOnly();
        }
    }

    public sealed class Oracle23AiHealthCheck : IHealthCheck
    {
        private readonly string _connectionString;
        private readonly string _modelName;
        private readonly int _expectedDimensions;

        public Oracle23AiHealthCheck(string connectionString, string modelName, int expectedDimensions)
        {
            _connectionString = connectionString;
            _modelName = modelName;
            _expectedDimensions = expectedDimensions;
        }

        public async Task<HealthCheckResult> CheckHealthAsync(
            HealthCheckContext context,
            CancellationToken cancellationToken = default)
        {
            try
            {
                await using var connection = new OracleConnection(_connectionString);
                await connection.OpenAsync(cancellationToken);
                const string modelInfoSql = "SELECT m.dimension_count FROM TABLE(DBMS_VECTOR.GET_MODEL_INFO(:model_name)) m";
                await using (var cmd = new OracleCommand(modelInfoSql, connection) { BindByName = true })
                {
                    cmd.Parameters.Add("model_name", OracleDbType.Varchar2).Value = _modelName;
                    var dimensions = Convert.ToInt32(await cmd.ExecuteScalarAsync(cancellationToken));

                    if (dimensions != _expectedDimensions)
                    {
                        return HealthCheckResult.Degraded(
                            $"Model dimension mismatch: expected {_expectedDimensions}, got {dimensions}");
                    }
                }
                const string embeddingSql = "SELECT VECTOR_EMBEDDING(:model_name USING :text AS data) FROM dual";
                await using (var cmd = new OracleCommand(embeddingSql, connection) { BindByName = true })
                {
                    cmd.Parameters.Add("model_name", OracleDbType.Varchar2).Value = _modelName;
                    cmd.Parameters.Add("text", OracleDbType.NVarchar2).Value = "health check";
                    await using var reader = await cmd.ExecuteReaderAsync(cancellationToken);
                    if (!await reader.ReadAsync(cancellationToken))
                    {
                        return HealthCheckResult.Unhealthy("Failed to generate test embedding");
                    }
                }

                return HealthCheckResult.Healthy($"Oracle 23ai vector search operational (model: {_modelName}, {_expectedDimensions}D)");
            }
            catch (Exception ex)
            {
                return HealthCheckResult.Unhealthy("Oracle 23ai vector search failed", ex);
            }
        }
    }
public sealed class CqnObservable : IDisposable
    {
        private readonly Subject<(string Table, string Operation)> _subject = new();
        private readonly ILogger<CqnObservable> _logger;

        private OracleDependency _dependency;
        private OracleConnection _connection;
        private OracleCommand _command;
        private bool _disposed;

        public IObservable<(string Table, string Operation)> Changes => _subject.AsObservable();

        public CqnObservable(
            string connectionString,
            string schemaName,
            string tableName,
            string idColumn,
            ILogger<CqnObservable> logger,
            int? port = null)
        {
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));

            try
            {
                if (port.HasValue)
                    OracleDependency.Port = port.Value;
                _connection = new OracleConnection(connectionString);
                _connection.Open();

                _command = new OracleCommand(
                    $"SELECT {idColumn} FROM {schemaName}.{tableName}",
                    _connection)
                {
                    AddRowid = true
                };
                _command.Notification.IsNotifiedOnce = false;
                _command.Notification.Timeout = 0;
                _dependency = new OracleDependency();
                _dependency.OnChange += OnChange;
                _dependency.AddCommandDependency(_command);
                _command.ExecuteNonQuery();

                _logger.LogInformation(
                    "CQN listening on {Schema}.{Table} (port {Port})",
                    schemaName, tableName, OracleDependency.Port);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Failed to initialize CQN for {Schema}.{Table}", schemaName, tableName);
                Dispose();
                throw;
            }
        }

        private void OnChange(object sender, OracleNotificationEventArgs e)
        {
            try
            {
                if (e.Details is DataTable dt)
                {
                    foreach (DataRow row in dt.Rows)
                    {
                        var tableName = row["ResourceName"]?.ToString() ?? "unknown";
                        var info = (OracleNotificationInfo)row["Info"];

                        _subject.OnNext((tableName, info.ToString()));
                        _logger.LogDebug("CQN: {Table} {Operation}", tableName, info);
                    }
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error processing CQN notification");
            }
        }

        public void Dispose()
        {
            if (_disposed) return;

            try
            {
                if (_dependency != null && _connection != null)
                {
                    _dependency.OnChange -= OnChange;
                    _dependency.RemoveRegistration(_connection);
                }
            }
            catch (Exception ex)
            {
                _logger.LogWarning(ex, "Error removing CQN registration");
            }

            _command?.Dispose();
            _connection?.Dispose();

            _subject.OnCompleted();
            _subject.Dispose();

            _disposed = true;
            _logger.LogInformation("CQN observable disposed");
        }
    }

    public static class ServiceCollectionExtensions
    {
        public static IServiceCollection AddOracle23AiServices(
            this IServiceCollection services,
            IConfiguration configuration)
        {
            var connString = configuration.GetConnectionString("OracleConnection");
            var modelName = configuration["Oracle23Ai:ModelName"] ?? "ALL_MINILM_L12_V2";
            var dimensions = int.Parse(configuration["Oracle23Ai:Dimensions"] ?? "384");
            services.AddSingleton(sp =>
            {
                var logger = sp.GetRequiredService<Microsoft.Extensions.Logging.ILogger<Oracle23AiBootstrapper>>();
                return new Oracle23AiBootstrapper(connString, logger);
            });

            services.AddScoped<IOracle23AiVectorService>(sp =>
            {
                var logger = sp.GetRequiredService<Microsoft.Extensions.Logging.ILogger<Oracle23AiVectorService>>();
                return new Oracle23AiVectorService(connString, logger);
            });
            services.AddHealthChecks()
                .AddCheck<Oracle23AiHealthCheck>("oracle_23ai_vector");

            services.AddSingleton(sp =>
            {
                return new Oracle23AiHealthCheck(connString, modelName, dimensions);
            });
            if (configuration.GetValue<bool>("Oracle23Ai:EnableCqn"))
            {
                services.AddSingleton(sp =>
                {
                    var logger = sp.GetRequiredService<ILogger<CqnObservable>>();
                    var schema = configuration["Oracle23Ai:CqnSchema"] ?? "MYAPP";
                    var table = configuration["Oracle23Ai:CqnTable"] ?? "PRODUCTS";
                    var idCol = configuration["Oracle23Ai:CqnIdColumn"] ?? "PRODUCTID";
                    int? port = configuration.GetValue<int?>("Oracle23Ai:CqnPort");
                    return new CqnObservable(
                        connectionString: configuration.GetConnectionString("OracleConnection"),
                        schemaName: schema,
                        tableName: table,
                        idColumn: idCol,
                        logger: logger,
                        port: port
                    );
                });
            }
            return services;
        }
    }
    internal sealed class OracleHealthChecksOptionsSetup : IConfigureOptions<HealthCheckServiceOptions>
    {
        private readonly IConfiguration _cfg;

        public OracleHealthChecksOptionsSetup(IConfiguration cfg) => _cfg = cfg;

        public void Configure(HealthCheckServiceOptions options)
        {
            const string name = "oracle_23ai_vector";
            if (options.Registrations.Any(r => string.Equals(r.Name, name, StringComparison.OrdinalIgnoreCase)))
                return;

            var conn = _cfg.GetConnectionString("OracleConnection")
                      ?? throw new InvalidOperationException("ConnectionStrings:OracleConnection is missing.");
            var model = _cfg["Oracle23Ai:ModelName"] ?? "ALL_MINILM_L12_V2";
            var dims = int.TryParse(_cfg["Oracle23Ai:Dimensions"], out var d) ? d : 384;

            options.Registrations.Add(
                new HealthCheckRegistration(
                    name,
                    sp => new Oracle23AiHealthCheck(conn, model, dims),
                    failureStatus: HealthStatus.Unhealthy,
                    tags: new[] { "infra", "oracle", "vector" }
                )
            );
        }
    }
    public sealed class VectorMetricPoint
    {
        public DateTime ObservedAtUtc { get; init; }
        public double Value { get; init; }
    }
    public sealed class VectorMetricSeries
    {
        public string Name { get; init; } = string.Empty;
        public IReadOnlyList<VectorMetricPoint> Points { get; init; } = System.Array.Empty<VectorMetricPoint>();
    }
    public sealed class VectorMetricsSeriesResponse
    {
        public bool Success { get; init; }
        public string? Error { get; init; }
        public IReadOnlyList<VectorMetricSeries> Series { get; init; } = System.Array.Empty<VectorMetricSeries>();

        public static VectorMetricsSeriesResponse Ok(IReadOnlyList<VectorMetricSeries> series) =>
            new() { Success = true, Series = series };

        public static VectorMetricsSeriesResponse Fail(string error) =>
            new() { Success = false, Error = error };
    }
    public sealed class VectorMetricsQuery
    {
        public string? Entity { get; set; }
        public int Days { get; set; } = 14;
    }
    public interface IVectorMetricsService
    {
        Task<VectorMetricsSeriesResponse> GetSeriesAsync(VectorMetricsQuery query, CancellationToken ct);
    }
    public sealed class VectorMetricsService : IVectorMetricsService
    {
        private readonly ILogger<VectorMetricsService> _log;
        private readonly IConfiguration _cfg;
        private readonly string _conn;
        private readonly string _schema;

        public VectorMetricsService(IConfiguration cfg, ILogger<VectorMetricsService> log)
        {
            _cfg = cfg ?? throw new ArgumentNullException(nameof(cfg));
            _log = log ?? throw new ArgumentNullException(nameof(log));

            _conn = _cfg.GetConnectionString("OracleConnection")
                    ?? throw new InvalidOperationException("ConnectionStrings:OracleConnection is missing.");
            _schema = (_cfg["Oracle23Ai:Schema"] ?? _cfg["Oracle23Ai:CqnSchema"] ?? "MYAPP").ToUpperInvariant();
        }

        public async Task<VectorMetricsSeriesResponse> GetSeriesAsync(VectorMetricsQuery query, CancellationToken ct)
        {
            try
            {
                using var con = new OracleConnection(_conn);
                await con.OpenAsync(ct);

                var hasSnapshots = await TableExistsAsync(con, ct, _schema, "VECTOR_METRICS_SNAPSHOTS");
                if (hasSnapshots)
                {
                    var snapSeries = await FromSnapshotsAsync(con, ct, query);
                    if (snapSeries.Count > 0)
                        return VectorMetricsSeriesResponse.Ok(snapSeries);
                }
                var live = await ComputeLiveAsync(con, ct, query);
                return VectorMetricsSeriesResponse.Ok(live);
            }
            catch (Exception ex)
            {
                _log.LogError(ex, "[VectorMetrics] GetSeries failed");
                return VectorMetricsSeriesResponse.Fail(ex.Message);
            }
        }

        private async Task<List<VectorMetricSeries>> FromSnapshotsAsync(OracleConnection con, CancellationToken ct, VectorMetricsQuery query)
        {
            string entity = (query.Entity ?? string.Empty).Trim().ToUpperInvariant();
            int days = query.Days <= 0 ? 14 : query.Days;
            var sql = $@"
SELECT ENTITY, METRIC_NAME, METRIC_VALUE, CAST(OBSERVED_AT AS TIMESTAMP) AS OBSERVED_AT
FROM   {_schema}.VECTOR_METRICS_SNAPSHOTS
WHERE  OBSERVED_AT >= SYSTIMESTAMP - INTERVAL '{days}' DAY
{(string.IsNullOrEmpty(entity) ? "" : "AND UPPER(ENTITY) = :p_entity")}
ORDER  BY METRIC_NAME, OBSERVED_AT";

            using var cmd = new OracleCommand(sql, con) { BindByName = true };
            if (!string.IsNullOrEmpty(entity))
                cmd.Parameters.Add(new OracleParameter(":p_entity", OracleDbType.NVarchar2) { Value = entity });

            var dict = new Dictionary<string, List<VectorMetricPoint>>(StringComparer.OrdinalIgnoreCase);

            using var rdr = await cmd.ExecuteReaderAsync(CommandBehavior.SingleResult, ct);
            while (await rdr.ReadAsync(ct))
            {
                var metric = rdr.GetString(1);
                var value = rdr.IsDBNull(2) ? 0.0 : rdr.GetDouble(2);
                var at = rdr.GetDateTime(3).ToUniversalTime();

                if (!dict.TryGetValue(metric, out var list))
                {
                    list = new List<VectorMetricPoint>();
                    dict[metric] = list;
                }
                list.Add(new VectorMetricPoint { ObservedAtUtc = at, Value = value });
            }

            return dict
                .Select(kv => new VectorMetricSeries { Name = kv.Key, Points = kv.Value })
                .OrderBy(s => s.Name, StringComparer.OrdinalIgnoreCase)
                .ToList();
        }
        private async Task<List<VectorMetricSeries>> ComputeLiveAsync(OracleConnection con, CancellationToken ct, VectorMetricsQuery query)
        {
            string entity = (query.Entity ?? string.Empty).Trim().ToUpperInvariant();
            int days = query.Days <= 0 ? 14 : query.Days;

            var series = new List<VectorMetricSeries>();
            if (!string.IsNullOrEmpty(entity))
            {
                var baseName = OracleSafe(entity);
                var vectName = OracleSafe(entity + "_VECTORS");
                long total = await CountRowsAsync(con, ct, _schema, baseName);
                string? embedCol = await FindAnyEmbedColumnAsync(con, ct, _schema, vectName);

                long embedded = 0;
                double avgLen = 0.0;

                if (!string.IsNullOrEmpty(embedCol))
                {
                    embedded = await CountNonNullVectorsAsync(con, ct, _schema, vectName, embedCol);
                    avgLen = await AverageVectorLengthAsync(con, ct, _schema, vectName, embedCol);
                }
                var pct = (total <= 0) ? 0.0 : (embedded / (double)total);
                double rebuildMs = await AvgIndexRebuildMsAsync(con, ct, days, entity);
                double p95ms = await P95QueryLatencyMsAsync(con, ct, days, entity);

                var now = DateTime.UtcNow;

                series.Add(new VectorMetricSeries
                {
                    Name = "percent_embedded",
                    Points = new[] { new VectorMetricPoint { ObservedAtUtc = now, Value = pct } }
                });
                series.Add(new VectorMetricSeries
                {
                    Name = "embedded_rows",
                    Points = new[] { new VectorMetricPoint { ObservedAtUtc = now, Value = embedded } }
                });
                series.Add(new VectorMetricSeries
                {
                    Name = "total_rows",
                    Points = new[] { new VectorMetricPoint { ObservedAtUtc = now, Value = total } }
                });
                series.Add(new VectorMetricSeries
                {
                    Name = "avg_vector_length",
                    Points = new[] { new VectorMetricPoint { ObservedAtUtc = now, Value = avgLen } }
                });
                series.Add(new VectorMetricSeries
                {
                    Name = "index_rebuild_ms",
                    Points = new[] { new VectorMetricPoint { ObservedAtUtc = now, Value = rebuildMs } }
                });
                series.Add(new VectorMetricSeries
                {
                    Name = "query_p95_ms",
                    Points = new[] { new VectorMetricPoint { ObservedAtUtc = now, Value = p95ms } }
                });

                return series;
            }
            return series;
        }

        private static string OracleSafe(string name)
        {
            if (string.IsNullOrWhiteSpace(name)) return "X";
            var cleaned = new string(name
                .ToUpperInvariant()
                .Select(ch => char.IsLetterOrDigit(ch) || ch == '_' ? ch : '_')
                .ToArray());
            return cleaned.Length <= 30 ? cleaned : cleaned.Substring(0, 30);
        }

        private static async Task<bool> TableExistsAsync(OracleConnection con, CancellationToken ct, string owner, string table)
        {
            using var cmd = new OracleCommand(
                "SELECT 1 FROM ALL_TABLES WHERE OWNER = :p_owner AND TABLE_NAME = :p_table",
                con)
            { BindByName = true };
            cmd.Parameters.Add(new OracleParameter(":p_owner", OracleDbType.NVarchar2) { Value = owner });
            cmd.Parameters.Add(new OracleParameter(":p_table", OracleDbType.NVarchar2) { Value = table.ToUpperInvariant() });

            var o = await cmd.ExecuteScalarAsync(ct);
            return o != null && o != DBNull.Value;
        }

        private static async Task<long> CountRowsAsync(OracleConnection con, CancellationToken ct, string owner, string table)
        {
            using var cmd = new OracleCommand($"SELECT COUNT(*) FROM {owner}.{table}", con);
            var o = await cmd.ExecuteScalarAsync(ct);
            return (o == null || o == DBNull.Value) ? 0L : Convert.ToInt64(o);
        }

        private static async Task<string?> FindAnyEmbedColumnAsync(OracleConnection con, CancellationToken ct, string owner, string table)
        {
            using var cmd = new OracleCommand(@"
SELECT COLUMN_NAME
FROM   ALL_TAB_COLUMNS
WHERE  OWNER = :p_owner AND TABLE_NAME = :p_table
  AND  COLUMN_NAME LIKE 'EMBED_%'
  AND  DATA_TYPE = 'VECTOR'
FETCH FIRST 1 ROWS ONLY", con) { BindByName = true };
            cmd.Parameters.Add(new OracleParameter(":p_owner", OracleDbType.NVarchar2) { Value = owner });
            cmd.Parameters.Add(new OracleParameter(":p_table", OracleDbType.NVarchar2) { Value = table });

            var o = await cmd.ExecuteScalarAsync(ct);
            return o?.ToString();
        }

        private static async Task<long> CountNonNullVectorsAsync(OracleConnection con, CancellationToken ct, string owner, string table, string vectorCol)
        {
            using var cmd = new OracleCommand($"SELECT COUNT(*) FROM {owner}.{table} WHERE {vectorCol} IS NOT NULL", con);
            var o = await cmd.ExecuteScalarAsync(ct);
            return (o == null || o == DBNull.Value) ? 0L : Convert.ToInt64(o);
        }

        private static async Task<double> AverageVectorLengthAsync(OracleConnection con, CancellationToken ct, string owner, string table, string vectorCol)
        {
            try
            {
                using var cmd = new OracleCommand(
                    $"SELECT AVG(VECTOR_LENGTH({vectorCol})) FROM {owner}.{table} WHERE {vectorCol} IS NOT NULL",
                    con);
                var o = await cmd.ExecuteScalarAsync(ct);
                return (o == null || o == DBNull.Value) ? 0.0 : Convert.ToDouble(o);
            }
            catch
            {
                return 0.0;
            }
        }

        private async Task<double> AvgIndexRebuildMsAsync(OracleConnection con, CancellationToken ct, int days, string entity)
        {
            var has = await TableExistsAsync(con, ct, _schema, "VECTOR_INDEX_LOG");
            if (!has) return 0.0;

            var sql = $@"
SELECT NVL(AVG(ELAPSED_MS), 0)
FROM   {_schema}.VECTOR_INDEX_LOG
WHERE  UPPER(ENTITY) = :p_entity
  AND  FINISHED_AT >= SYSTIMESTAMP - INTERVAL '{days}' DAY";

            using var cmd = new OracleCommand(sql, con) { BindByName = true };
            cmd.Parameters.Add(new OracleParameter(":p_entity", OracleDbType.NVarchar2) { Value = entity.ToUpperInvariant() });
            var o = await cmd.ExecuteScalarAsync(ct);
            return (o == null || o == DBNull.Value) ? 0.0 : Convert.ToDouble(o);
        }

        private async Task<double> P95QueryLatencyMsAsync(OracleConnection con, CancellationToken ct, int days, string entity)
        {
            var has = await TableExistsAsync(con, ct, _schema, "VECTOR_QUERY_LOG");
            if (!has) return 0.0;
            var sql = $@"
SELECT CAST(PERCENTILE_DISC(0.95) WITHIN GROUP (ORDER BY LATENCY_MS) AS NUMBER)
FROM   {_schema}.VECTOR_QUERY_LOG
WHERE  UPPER(ENTITY) = :p_entity
  AND  STARTED_AT >= SYSTIMESTAMP - INTERVAL '{days}' DAY";

            using var cmd = new OracleCommand(sql, con) { BindByName = true };
            cmd.Parameters.Add(new OracleParameter(":p_entity", OracleDbType.NVarchar2) { Value = entity.ToUpperInvariant() });
            var o = await cmd.ExecuteScalarAsync(ct);
            return (o == null || o == DBNull.Value) ? 0.0 : Convert.ToDouble(o);
        }
    }

}
