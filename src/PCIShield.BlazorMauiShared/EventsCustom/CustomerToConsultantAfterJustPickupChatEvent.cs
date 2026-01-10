using System.Text.Json.Serialization;
using PCIShield.Domain.ModelsDto;
using PCIShieldLib.SharedKernel;
namespace PCIShield.Domain.Events;
public class MerchantToComplianceOfficerAfterJustPickupChatEvent : BaseDomainEvent
{
    [JsonConstructor]
    public MerchantToComplianceOfficerAfterJustPickupChatEvent()
    {
    }
    public MerchantToComplianceOfficerAfterJustPickupChatEvent(MerchantChatMessageSinalRModel merchantResponse, string message)
    {
        ActionOnMessageReceived = message;
        EntityNameType = "Merchant";
        Content = System.Text.Json.JsonSerializer.Serialize(merchantResponse, JsonSerializerSettingsSingleton.Instance);
        EventType = "MerchantToComplianceOfficerToPickupChatEvent";
        EventId = Guid.NewGuid();
        OccurredOnUtc = DateTime.UtcNow;
        Message = message;
    }
    public Guid UnansweredConversationId { get; set; }
}