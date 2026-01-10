using MediatR;
namespace PCIShieldLib.SharedKernel
{
    public abstract class BaseIntegrationEvent : OutBoxMessage, INotification
    {
    }
}