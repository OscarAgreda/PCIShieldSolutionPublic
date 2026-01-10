using PCIShield.Domain.ModelsDto;
using PCIShieldLib.SharedKernel;
namespace PCIShield.Domain.Events;
public class MerchantSentRegularChatConversationEvent : BaseDomainEvent
{
    public MerchantSentRegularChatConversationEvent(MerchantChatMessageSinalRModel MerchantSentRegularChatMsg, string message)
    {
        ActionOnMessageReceived = message;
        EntityNameType = "Merchant";
        Content = System.Text.Json.JsonSerializer.Serialize(MerchantSentRegularChatMsg, JsonSerializerSettingsSingleton.Instance);
        EventType = "MerchantSentRegularChatConversationEvent";
        EventId = Guid.NewGuid();
        OccurredOnUtc = DateTime.UtcNow;
        Message = message;
    }
}