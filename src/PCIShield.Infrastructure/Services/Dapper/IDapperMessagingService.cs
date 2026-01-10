using System.Collections.Generic;
using System.Threading.Tasks;
using Dapper;
namespace PCIShield.Infrastructure.Services
{
    public interface IDapperMessagingService
    {
        Task<IEnumerable<T>> QueryAsync<T>(string sql, object parameters = null);
        Task<T> QueryFirstOrDefaultAsync<T>(string sql, object parameters = null);
        Task<T> QuerySingleAsync<T>(string sql, object parameters = null);
        Task<T> QuerySingleOrDefaultAsync<T>(string sql, object parameters = null);
        Task<int> ExecuteAsync(string sql, object parameters = null);
        Task<SqlMapper.GridReader> QueryMultipleAsync(string sql, object parameters = null);
    }
}