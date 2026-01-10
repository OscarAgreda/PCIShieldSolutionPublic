using PCIShieldLib.SharedKernel;
namespace PCIShield.Api.Saga.SignalR
{
    public class ChatProcessEventResult
    {
        public BaseDomainEvent DomainEvent { get; set; }
        public string RoutingKey { get; set; }
    }
}