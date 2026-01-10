namespace PCIShieldLib.SharedKernel.Interfaces
{
    public interface IApplicationMessagePublisher
    {
        void Publish(BaseDomainEvent baseDomainEvent);
    }
}