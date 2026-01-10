using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Ardalis.Specification;
namespace PCIShieldLib.SharedKernel.Interfaces
{
    public interface IReadMongoRepository<T> : IReadRepositoryBase<T> where T : class, IAggregateRoot
    {
        Task<T?> FirstOrDefaultAsyncFromOtherMicroservice(ISpecification<T> specification, string pciShieldappAppSuperUserId,
            CancellationToken cancellationToken = default);
        Task<List<T>> ListAsyncFromOtherMicroservice(ISpecification<T> specification, string pciShieldappAppSuperUserId,
            CancellationToken cancellationToken = default);
        Task<List<T>> ListAsyncWithCustomExpiration(ISpecification<T> specification, TimeSpan? absoluteExpiration,
            CancellationToken cancellationToken = default);
    }
}