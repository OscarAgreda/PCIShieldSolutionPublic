using System.Text.Json.Serialization;
using PCIShield.Domain.ModelsDto;
using PCIShieldLib.SharedKernel;
namespace PCIShield.Domain.Events
{
    public class MerchantToComplianceOfficerToPickupChatEvent : BaseDomainEvent
    {
        [JsonConstructor]
        public MerchantToComplianceOfficerToPickupChatEvent()
        {
        }
        public MerchantToComplianceOfficerToPickupChatEvent(IList<AvailableComplianceOfficerServerSentToMerchantDto> availableComplianceOfficers, string message)
        {
            ActionOnMessageReceived = message;
            EntityNameType = "Merchant";
            Content = System.Text.Json.JsonSerializer.Serialize(availableComplianceOfficers, JsonSerializerSettingsSingleton.Instance);
            EventType = "MerchantToComplianceOfficerToPickupChatEvent";
            EventId = Guid.NewGuid();
            OccurredOnUtc = DateTime.UtcNow;
            Message = message;
        }
        [JsonPropertyName("MerchantId")]
        public Guid MerchantId { get; set; }
        [JsonPropertyName("MessageId")]
        public Guid MessageId { get; set; }
        [JsonPropertyName("MessageTypeId")]
        public Guid MessageTypeId { get; set; }
        [JsonPropertyName("UnansweredConversationId")]
        public Guid UnansweredConversationId { get; set; }
    }
}