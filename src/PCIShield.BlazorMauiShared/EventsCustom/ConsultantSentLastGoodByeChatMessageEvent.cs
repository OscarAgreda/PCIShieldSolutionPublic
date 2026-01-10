using PCIShield.Domain.ModelsDto;
using PCIShieldLib.SharedKernel;
namespace PCIShield.BlazorMauiShared.EventsCustom;
public class ComplianceOfficerSentLastGoodByeChatMessageEvent : BaseDomainEvent
{
    public ComplianceOfficerSentLastGoodByeChatMessageEvent(ComplianceOfficerSentLastGoodByeChatMessage complianceOfficerMsg, string message)
    {
        ActionOnMessageReceived = message;
        EntityNameType = "ComplianceOfficer";
        Content = System.Text.Json.JsonSerializer.Serialize(complianceOfficerMsg, JsonSerializerSettingsSingleton.Instance);
        EventType = "ComplianceOfficerSentLastGoodByeChatMessageEvent";
        EventId = Guid.NewGuid();
        OccurredOnUtc = DateTime.UtcNow;
        Message = message;
    }
}