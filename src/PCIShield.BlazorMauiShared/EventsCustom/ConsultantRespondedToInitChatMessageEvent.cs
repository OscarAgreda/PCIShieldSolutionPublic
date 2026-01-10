using System;
using PCIShieldLib.SharedKernel;
using PCIShield.Domain.ModelsDto;
using Ardalis.GuardClauses;
using System.Text.Json.Serialization;
namespace PCIShield.BlazorMauiShared.EventsCustom
{
    public class ComplianceOfficerRespondedToInitChatMessageEvent : BaseDomainEvent
    {
        [JsonConstructor]
        public ComplianceOfficerRespondedToInitChatMessageEvent()
        {
        }
        public ComplianceOfficerRespondedToInitChatMessageEvent(
            ComplianceOfficerChatMessageSinalRModel complianceOfficerMsg,   string message,   string pciShieldappAppSuperUserId)
        {
            PCIShieldSolutionuperUserId = pciShieldappAppSuperUserId;
            ActionOnMessageReceived = message;
            EntityNameType = "ComplianceOfficer";
            Content = System.Text.Json.JsonSerializer.Serialize(complianceOfficerMsg, JsonSerializerSettingsSingleton.Instance);
            EventType = "ComplianceOfficerRespondedToInitChatMessageEvent";
            EventId = Guid.NewGuid();
            OccurredOnUtc = DateTime.UtcNow;
            Message = message;
        }
    }
}
