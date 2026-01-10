using PCIShield.Domain.ModelsDto;
using PCIShieldLib.SharedKernel;
namespace PCIShield.Domain.Events;
public class MerchantSentFinalFarewellEvent : BaseDomainEvent
{
    public MerchantSentFinalFarewellEvent(MerchantSentFinalFarewellMessage MerchantInitiatedChatMsg, string message)
    {
        ActionOnMessageReceived = message;
        EntityNameType = "Merchant";
        Content = System.Text.Json.JsonSerializer.Serialize(MerchantInitiatedChatMsg, JsonSerializerSettingsSingleton.Instance);
        EventType = "MerchantSentFinalFarewellEvent";
        EventId = Guid.NewGuid();
        OccurredOnUtc = DateTime.UtcNow;
        Message = message;
    }
}