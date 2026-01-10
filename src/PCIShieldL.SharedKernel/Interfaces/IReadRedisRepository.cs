using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Ardalis.Specification;
namespace PCIShieldLib.SharedKernel.Interfaces
{
    public interface IReadRedisRepository<T> : IReadRepositoryBase<T> where T : class, IAggregateRoot
    {
        Task<T?> FirstOrDefaultAsyncFromOtherMicroservice(ISpecification<T> specification, string pciShieldappAppSuperUserId,
            CancellationToken cancellationToken = default);
        Task<List<T>> ListAsyncFromOtherMicroservice(ISpecification<T> specification, string pciShieldappAppSuperUserId,
            CancellationToken cancellationToken = default);
        Task<List<T>> ListAsyncWithCustomExpiration(ISpecification<T> specification, TimeSpan? absoluteExpiration,
            CancellationToken cancellationToken = default);
        Task<PagedResult<T>> GetPagedResultAsync(
            PagedSpecification<T> specification,
            int pageNumber,
            int pageSize,
            CancellationToken cancellationToken = default);
        Task<PagedResult<TResult>> GetPagedResultAsync<TResult>(
            PagedSpecification<T, TResult> specification,
            int pageNumber,
            int pageSize,
            CancellationToken cancellationToken = default);
        Task InvalidateEntityCacheAsync<TEntity>(
            Guid? entityId = null,
            string? customSpecificationName = null,
            int? pageSize = null,
            int? pageNumber = null,
            Type? projectionType = null) where TEntity : class, IAggregateRoot;
    }
}