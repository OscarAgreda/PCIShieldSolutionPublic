using PCIShield.Domain.ModelsDto;
using PCIShieldLib.SharedKernel;
namespace PCIShield.BlazorMauiShared.EventsCustom;
public class ComplianceOfficerSentRegularChatEvent : BaseDomainEvent
{
    public ComplianceOfficerSentRegularChatEvent(ComplianceOfficerSentRegularChatMessage complianceOfficerMsg, string message)
    {
        ActionOnMessageReceived = message;
        EntityNameType = "ComplianceOfficer";
        Content = System.Text.Json.JsonSerializer.Serialize(complianceOfficerMsg, JsonSerializerSettingsSingleton.Instance);
        EventType = "ComplianceOfficerSentRegularChatEvent";
        EventId = Guid.NewGuid();
        OccurredOnUtc = DateTime.UtcNow;
        Message = message;
    }
}