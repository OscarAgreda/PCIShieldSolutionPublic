using MediatR;
namespace PCIShieldLib.SharedKernel
{
    public interface IBaseDomainEvent
    {
    }
    public abstract class BaseDomainEvent : OutBoxMessage, MediatR.INotification, IBaseDomainEvent
    {
    }
    public abstract  class DapperDomainEvent : OutBoxMessage, MediatR.INotification
    {
    }
    public abstract class EfDomainEvent : OutBoxMessage, MediatR.INotification
    {
    }
}