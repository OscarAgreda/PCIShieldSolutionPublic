using System;
using PCIShieldLib.SharedKernel;
using PCIShield.Domain.ModelsDto;
namespace PCIShield.Domain.Events
{
    public class ServerSentToComplianceOfficerEvent : BaseDomainEvent
    {
        public ServerSentToComplianceOfficerEvent(CurrentMerchantServerSentToComplianceOfficerDto availableAdvisdor, string message)
        {
            ActionOnMessageReceived = message;
            EntityNameType = "ComplianceOfficer";
            Content = System.Text.Json.JsonSerializer.Serialize(availableAdvisdor, JsonSerializerSettingsSingleton.Instance);
            EventType = "ComplianceOfficerUpdated";
            EventId = Guid.NewGuid();
            OccurredOnUtc = DateTime.UtcNow;
            Message = message;
        }
    }
}
