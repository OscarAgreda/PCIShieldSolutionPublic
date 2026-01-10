using System.Collections.Generic;
using System.Threading.Tasks;
namespace PCIShieldLib.SharedKernel.Interfaces
{
    public interface IDomainEventDispatcher
    {
        Task DispatchAndClearEvents(IEnumerable<EntityBase> entitiesWithEvents);
    }
}