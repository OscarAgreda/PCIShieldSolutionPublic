using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using System.Threading;
using System.Threading.Tasks;

using Ardalis.Specification;
using Ardalis.Specification.EntityFrameworkCore;

using PCIShield.Infrastructure.Services;
using PCIShield.Infrastructure.Services.Redis;

using PCIShieldLib.SharedKernel.Interfaces;

using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Storage;

using Z.EntityFramework.Plus;
namespace PCIShield.Infrastructure.Data
{
    public class EfRepository<T> : RepositoryBase<T>, IRepository<T> where T : class, IAggregateRoot
    {
        private readonly IAppLoggerService<EfRepository<T>> _seriLogger;
        private readonly DbContext _dbContext;
        private IDbContextTransaction _transaction;
        public EfRepository(AppDbContext dbContext, IAppLoggerService<EfRepository<T>> seriLogger) : base(dbContext)
        {
            _dbContext = dbContext;
            _seriLogger = seriLogger;
        }
        public void BeginTransaction()
        {
            _transaction = _dbContext.Database.BeginTransaction();
        }
        public void CommitTransaction()
        {
            try
            {
                _transaction?.Commit();
            }
            finally
            {
                _transaction?.Dispose();
            }
        }
        public async Task<T> FirstOrDefaultAsyncWithIncludeOptimized(ISpecification<T> spec, CancellationToken cancellationToken = default)
        {
            try
            {
                _seriLogger.LogInformation("Executing FirstOrDefaultAsyncWithIncludeOptimized for {EntityType} with spec: {SpecificationName}",
                    typeof(T).Name, spec.GetType().Name);

                var queryable = SpecificationEvaluator.Default.GetQuery(_dbContext.Set<T>().AsQueryable(), spec);
                foreach (var includeExpressionInfo in spec.IncludeExpressions)
                {
                    queryable = ApplyIncludeOptimized(queryable, includeExpressionInfo);
                }
                var result = await queryable.FirstOrDefaultAsync(cancellationToken);
                return result;
            }
            catch (Exception ex)
            {
                _seriLogger.LogError(ex, "Error in FirstOrDefaultAsyncWithIncludeOptimized for {EntityType} with spec: {SpecificationName}",
                    typeof(T).Name, spec.GetType().Name);
                throw;
            }
        }
        public void RollbackTransaction()
        {
            try
            {
                _transaction?.Rollback();
                _seriLogger.LogWarning("Rolled back database transaction for {EntityType}.", typeof(T).Name);
            }
            catch (Exception ex)
            {
                _seriLogger.LogError(ex, "Error rolling back database transaction for {EntityType}.", typeof(T).Name);
                throw;
            }
            finally
            {
                _transaction?.Dispose();
                _transaction = null;
            }
        }
        private static IQueryable<T>? ApplyIncludeOptimized<T>(IQueryable<T>? query, IncludeExpressionInfo includeExpressionInfo) where T : class
        {
            var lambdaExpression = includeExpressionInfo.LambdaExpression;
            var isCollection = typeof(IEnumerable).IsAssignableFrom(includeExpressionInfo.PropertyType) && includeExpressionInfo.PropertyType.IsGenericType;
            MethodInfo? method;
            if (isCollection)
            {
                method = typeof(QueryIncludeOptimizedExtensions).GetMethods().FirstOrDefault(m =>
                    m.Name == "IncludeOptimized" && m.GetParameters().Length == 2 && m.GetParameters()[1].ParameterType.GetGenericTypeDefinition() == typeof(Expression<>).MakeGenericType(typeof(Func<,>).MakeGenericType(typeof(T), typeof(IEnumerable<>))));
            }
            else
            {
                method = typeof(QueryIncludeOptimizedExtensions).GetMethods().FirstOrDefault(m =>
                    m.Name == "IncludeOptimized" && m.GetParameters().Length == 2 && m.GetParameters()[1].ParameterType.GetGenericTypeDefinition() == typeof(Expression<>).MakeGenericType(typeof(Func<,>)));
            }
            if (method != null)
            {
                var genericMethod = method.MakeGenericMethod(typeof(T), includeExpressionInfo.PropertyType);
                if (query != null)
                {
                    query = genericMethod.Invoke(null, new object[] { query, lambdaExpression }) as IQueryable<T>;
                }
            }
            return query;
        }

        public async Task<PagedResult<T>> GetPagedResultAsync(
            PagedSpecification<T> specification,
            int pageNumber,
            int pageSize,
            CancellationToken cancellationToken = default)
        {
            try
            {
                var countSpec = specification.GetCountSpecification();
                var itemsTask = await ListAsync(specification, cancellationToken);
                var countTask = await CountAsync(countSpec, cancellationToken);
                return new PagedResult<T>
                {
                    Items = itemsTask,
                    TotalCount = countTask,
                    PageNumber = pageNumber,
                    PageSize = pageSize
                };
            }
            catch (Exception ex)
            {
                _seriLogger.LogError(ex,
                    "Error in GetPagedResultAsync for {EntityType} (non-projected). Page: {PageNumber}, Size: {PageSize}, Spec: {SpecificationName}",
                    typeof(T).Name, pageNumber, pageSize, specification.GetType().Name);
                throw;
            }

        }
        public async Task<PagedResult<TResult>> GetPagedResultAsync<TResult>(
           PagedSpecification<T, TResult> specification,
           int pageNumber,
           int pageSize,
           CancellationToken cancellationToken = default)
        {

            try
            {
                var countSpec = specification.GetCountSpecification();
                var items = await ListAsync(specification, cancellationToken); 
                var totalCount = await CountAsync(countSpec, cancellationToken);    
                return new PagedResult<TResult>
                {
                    Items = items ?? new List<TResult>(),
                    TotalCount = totalCount,
                    PageNumber = pageNumber, 
                    PageSize = pageSize      
                };
            }
            catch (System.Data.SqlTypes.SqlNullValueException sqlEx)
            {
                _seriLogger.LogError(sqlEx,
                    "SqlNullValueException in GetPagedResultAsync for EntityType: {EntityType} to ResultType: {ResultType}. Spec: {SpecificationName}. Page: {PageNumber}, Size: {PageSize}. Ensure projections and DTO properties handle potential DB NULLs.",
                    typeof(T).Name,
                    typeof(TResult).Name,
                    specification.GetType().Name,
                    pageNumber,
                    pageSize);
                throw;
            }
            catch (Exception ex)
            {
                _seriLogger.LogError(ex,
                    "Unexpected exception in GetPagedResultAsync for EntityType: {EntityType} to ResultType: {ResultType}. Spec: {SpecificationName}. Page: {PageNumber}, Size: {PageSize}.",
                    typeof(T).Name,
                    typeof(TResult).Name,
                    specification.GetType().Name,
                    pageNumber,
                    pageSize);
                throw;
            }
        }
    }
}