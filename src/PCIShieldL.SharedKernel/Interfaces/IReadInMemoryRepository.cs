using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Ardalis.Specification;
namespace PCIShieldLib.SharedKernel.Interfaces
{
    public interface IReadInMemoryRepository<T> : IReadRepositoryBase<T> where T : class, IAggregateRoot
    {
        ValueTask<T> FirstOrDefaultAsyncFromOtherMicroservice(
            ISpecification<T> specification,
            string pciShieldappAppSuperUserId,
            CancellationToken cancellationToken = default);
        ValueTask<List<T>> ListAsyncFromOtherMicroservice(
            ISpecification<T> specification,
            string pciShieldappAppSuperUserId,
            CancellationToken cancellationToken = default);
    }
}