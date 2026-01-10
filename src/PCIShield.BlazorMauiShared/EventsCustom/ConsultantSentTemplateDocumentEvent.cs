using PCIShield.Domain.ModelsDto;
using PCIShieldLib.SharedKernel;
namespace PCIShield.BlazorMauiShared.EventsCustom;
public class ComplianceOfficerSentTemplateDocumentEvent : BaseDomainEvent
{
    public ComplianceOfficerSentTemplateDocumentEvent(ComplianceOfficerSendTemplateDocumentMessage complianceOfficerMsg, string message)
    {
        ActionOnMessageReceived = message;
        EntityNameType = "ComplianceOfficer";
        Content = System.Text.Json.JsonSerializer.Serialize(complianceOfficerMsg, JsonSerializerSettingsSingleton.Instance);
        EventType = "ComplianceOfficerSentTemplateDocumentEvent";
        EventId = Guid.NewGuid();
        OccurredOnUtc = DateTime.UtcNow;
        Message = message;
    }
}