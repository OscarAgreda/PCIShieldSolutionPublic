using System;

using PCIShieldLib.SharedKernel;
namespace PCIShield.Infrastructure.Services
{
    public interface IMerchantComplianceOfficerMessagePublisherService
    {
        void PublishDomainEventToQueue(BaseDomainEvent domainEvent, string queueType, Guid responseMessageId);
    }
}