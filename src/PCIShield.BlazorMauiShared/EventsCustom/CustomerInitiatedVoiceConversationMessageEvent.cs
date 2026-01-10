using PCIShield.Domain.ModelsDto;
using PCIShieldLib.SharedKernel;
namespace PCIShield.Domain.Events;
public class MerchantInitiatedVoiceConversationMessageEvent : BaseDomainEvent
{
    public MerchantInitiatedVoiceConversationMessageEvent(MerchantInitiatedVoiceConversationMessage MerchantInitiatedChatMsg, string message)
    {
        ActionOnMessageReceived = message;
        EntityNameType = "Merchant";
        Content = System.Text.Json.JsonSerializer.Serialize(MerchantInitiatedChatMsg, JsonSerializerSettingsSingleton.Instance);
        EventType = "MerchantInitiatedVoiceConversationMessageEvent";
        EventId = Guid.NewGuid();
        OccurredOnUtc = DateTime.UtcNow;
        Message = message;
    }
}