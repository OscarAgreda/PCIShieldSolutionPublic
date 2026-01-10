using PCIShield.Domain.ModelsDto;
using PCIShieldLib.SharedKernel;
namespace PCIShield.Domain.Events;
public class MerchantAbruptlyEndedChatEvent : BaseDomainEvent
{
    public MerchantAbruptlyEndedChatEvent(MerchantAbruptlyEndedChat MerchantInitiatedChatMsg, string message)
    {
        ActionOnMessageReceived = message;
        EntityNameType = "Merchant";
        Content = System.Text.Json.JsonSerializer.Serialize(MerchantInitiatedChatMsg, JsonSerializerSettingsSingleton.Instance);
        EventType = "MerchantAbruptlyEndedChatEvent";
        EventId = Guid.NewGuid();
        OccurredOnUtc = DateTime.UtcNow;
        Message = message;
    }
}