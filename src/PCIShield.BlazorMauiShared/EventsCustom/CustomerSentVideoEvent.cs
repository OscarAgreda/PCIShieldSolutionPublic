using PCIShield.Domain.ModelsDto;
using PCIShieldLib.SharedKernel;
namespace PCIShield.Domain.Events;
public class MerchantSentVideoEvent : BaseDomainEvent
{
    public MerchantSentVideoEvent(MerchantSentVideoMessage MerchantInitiatedChatMsg, string message)
    {
        ActionOnMessageReceived = message;
        EntityNameType = "Merchant";
        Content = System.Text.Json.JsonSerializer.Serialize(MerchantInitiatedChatMsg, JsonSerializerSettingsSingleton.Instance);
        EventType = "MerchantSentVideoEvent";
        EventId = Guid.NewGuid();
        OccurredOnUtc = DateTime.UtcNow;
        Message = message;
    }
}