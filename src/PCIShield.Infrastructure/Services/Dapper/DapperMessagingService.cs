using System;
using System.Collections.Generic;
using System.Data;
using System.Threading.Tasks;
using Dapper;
using Microsoft.Data.SqlClient;
namespace PCIShield.Infrastructure.Services
{
    public class DapperMessagingService : IDapperMessagingService
    {
        private readonly IDbConnection _dbConnection;
        private readonly IAppLoggerService<DapperMessagingService> _logger;
        public DapperMessagingService(
            IDbConnection dbConnection,
            IAppLoggerService<DapperMessagingService> logger
        )
        {
            _dbConnection = dbConnection;
            _logger = logger;
        }
        public async Task<int> ExecuteAsync(string sql, object parameters = null)
        {
            int result;
            var connectionString = _dbConnection.ConnectionString;
            var dbConnection = new SqlConnection(connectionString);
            try
            {
                if (dbConnection.State != ConnectionState.Open)
                {
                    await dbConnection.OpenAsync();
                }
                result = await dbConnection.ExecuteAsync(sql, parameters);
            }
            catch (Exception e)
            {
                _logger.LogError(
                    e,
                    "A SQL error occurred. with the command: "
                        + sql.ToString()
                        + " with parameters: "
                        + parameters.ToString()
                );
                throw;
            }
            finally
            {
                if (dbConnection.State != ConnectionState.Closed)
                {
                    dbConnection.Close();
                }
            }
            return result;
        }
        public async Task<IEnumerable<T>> QueryAsync<T>(string sql, object parameters = null)
        {
            IEnumerable<T> result;
            var connectionString = _dbConnection.ConnectionString;
            using (var dbConnection = new SqlConnection(connectionString))
            {
                try
                {
                    if (dbConnection.State != ConnectionState.Open)
                    {
                        await dbConnection.OpenAsync();
                    }
                    result = await dbConnection.QueryAsync<T>(sql, parameters);
                }
                catch (Exception e)
                {
                    throw;
                }
                finally
                {
                    if (dbConnection.State != ConnectionState.Closed)
                    {
                        dbConnection.Close();
                    }
                }
            }
            return result;
        }
        public async Task<T> QueryFirstOrDefaultAsync<T>(string sql, object parameters = null)
        {
            T result;
            var connectionString = _dbConnection.ConnectionString;
            var dbConnection = new SqlConnection(connectionString);
            try
            {
                if (dbConnection.State != ConnectionState.Open)
                {
                    await dbConnection.OpenAsync();
                }
                result = await dbConnection.QueryFirstOrDefaultAsync<T>(sql, parameters);
            }
            catch (Exception e)
            {
                throw;
            }
            finally
            {
                if (dbConnection.State != ConnectionState.Closed)
                {
                    dbConnection.Close();
                }
            }
            return result;
        }
        public async Task<SqlMapper.GridReader> QueryMultipleAsync(
            string sql,
            object parameters = null
        )
        {
            SqlMapper.GridReader result;
            var connectionString = _dbConnection.ConnectionString;
            var dbConnection = new SqlConnection(connectionString);
            try
            {
                if (dbConnection.State != ConnectionState.Open)
                {
                    await dbConnection.OpenAsync();
                }
                result = await dbConnection.QueryMultipleAsync(sql, parameters);
            }
            catch (Exception e)
            {
                throw;
            }
            finally
            {
                if (dbConnection.State != ConnectionState.Closed)
                {
                    dbConnection.Close();
                }
            }
            return result;
        }
        public async Task<T> QuerySingleAsync<T>(string sql, object parameters = null)
        {
            T result;
            var connectionString = _dbConnection.ConnectionString;
            var dbConnection = new SqlConnection(connectionString);
            try
            {
                if (dbConnection.State != ConnectionState.Open)
                {
                    await dbConnection.OpenAsync();
                }
                result = await dbConnection.QuerySingleAsync<T>(sql, parameters);
            }
            catch (Exception e)
            {
                throw;
            }
            finally
            {
                if (dbConnection.State != ConnectionState.Closed)
                {
                    dbConnection.Close();
                }
            }
            return result;
        }
        public async Task<T> QuerySingleOrDefaultAsync<T>(string sql, object parameters = null)
        {
            T result;
            var connectionString = _dbConnection.ConnectionString;
            var dbConnection = new SqlConnection(connectionString);
            try
            {
                if (dbConnection.State != ConnectionState.Open)
                {
                    await dbConnection.OpenAsync();
                }
                result = await dbConnection.QuerySingleOrDefaultAsync<T>(sql, parameters);
            }
            catch (Exception e)
            {
                throw;
            }
            finally
            {
                if (dbConnection.State != ConnectionState.Closed)
                {
                    dbConnection.Close();
                }
            }
            return result;
        }
    }
}