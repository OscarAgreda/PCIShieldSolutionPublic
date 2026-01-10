using PCIShield.Domain.ModelsDto;
using PCIShieldLib.SharedKernel;
namespace PCIShield.BlazorMauiShared.EventsCustom;
public class ComplianceOfficerEndedChatOnInactivityEvent : BaseDomainEvent
{
    public ComplianceOfficerEndedChatOnInactivityEvent(ComplianceOfficerEndedChatDueToInactivity complianceOfficerMsg, string message)
    {
        ActionOnMessageReceived = message;
        EntityNameType = "ComplianceOfficer";
        Content = System.Text.Json.JsonSerializer.Serialize(complianceOfficerMsg, JsonSerializerSettingsSingleton.Instance);
        EventType = "ComplianceOfficerEndedChatOnInactivityEvent";
        EventId = Guid.NewGuid();
        OccurredOnUtc = DateTime.UtcNow;
        Message = message;
    }
}