using PCIShieldLib.SharedKernel;
namespace PCIShield.Domain.Events;
public class GenericChatEvent : BaseDomainEvent
{
    public GenericChatEvent(object dto, string eventType, string message)
    {
        ActionOnMessageReceived = message;
        EntityNameType = eventType;
        Content = System.Text.Json.JsonSerializer.Serialize(dto, JsonSerializerSettingsSingleton.Instance);
        EventType = eventType;
        EventId = Guid.NewGuid();
        OccurredOnUtc = DateTime.UtcNow;
        Message = message;
    }
    public Guid UnansweredConversationId { get; set; }
}