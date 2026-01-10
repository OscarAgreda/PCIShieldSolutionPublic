using System;
using PCIShield.Domain.Entities;
using PCIShieldLib.SharedKernel;
using System.Text.Json;

namespace PCIShield.Domain.Events
{
    public class ServiceProviderUpdatedEvent : BaseDomainEvent
    {
        public ServiceProviderUpdatedEvent(ServiceProvider serviceProvider, string action)
        {
            ActionOnMessageReceived = action;
            
            EntityNameType = "ServiceProvider";
            
            Content = System.Text.Json.JsonSerializer.Serialize(serviceProvider, JsonSerializerSettingsSingleton.Instance);
            
            EventType = "ServiceProviderUpdated";
            
            EventId = UuidV7Generator.NewUuidV7();
            
            OccurredOnUtc = DateTime.UtcNow;
        }
    }
}

