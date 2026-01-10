using System.Threading.Tasks;
namespace PCIShieldLib.SharedKernel.Interfaces
{
    public interface IHandle<T> where T : BaseDomainEvent
    {
        Task HandleAsync(T args);
    }
}