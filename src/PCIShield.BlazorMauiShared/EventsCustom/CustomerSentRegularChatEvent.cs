using PCIShield.Domain.ModelsDto;
using PCIShieldLib.SharedKernel;
namespace PCIShield.Domain.Events;
public class MerchantSentRegularChatEvent : BaseDomainEvent
{
    public MerchantSentRegularChatEvent(MerchantSentRegularChatMessage MerchantInitiatedChatMsg, string message)
    {
        ActionOnMessageReceived = message;
        EntityNameType = "Merchant";
        Content = System.Text.Json.JsonSerializer.Serialize(MerchantInitiatedChatMsg, JsonSerializerSettingsSingleton.Instance);
        EventType = "MerchantSentRegularChatEvent";
        EventId = Guid.NewGuid();
        OccurredOnUtc = DateTime.UtcNow;
        Message = message;
    }
}