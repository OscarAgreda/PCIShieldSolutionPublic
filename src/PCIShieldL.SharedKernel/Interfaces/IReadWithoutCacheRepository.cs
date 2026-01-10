using Ardalis.Specification;
namespace PCIShieldLib.SharedKernel.Interfaces
{
    public interface IReadWithoutCacheRepository<T> : IReadRepositoryBase<T> where T : class, IAggregateRoot
    {
    }
}