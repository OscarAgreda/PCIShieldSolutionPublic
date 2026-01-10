using System;
namespace PCIShieldLib.SharedKernel
{
    public class GenericOccuredEvent : BaseDomainEvent
    {
        public GenericOccuredEvent(object entity, string action, string entityname, string consumer, string eventtype)
        {
            ActionOnMessageReceived = action;
            EntityNameType = entityname;
            Consumer = consumer;
            Content = System.Text.Json.JsonSerializer.Serialize(entity, JsonSerializerSettingsSingleton.Instance);
            EventType = eventtype;
            EventId = Guid.NewGuid();
            OccurredOnUtc = DateTime.UtcNow;
        }
    }
}