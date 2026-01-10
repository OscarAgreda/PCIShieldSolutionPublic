using System;
using PCIShield.Domain.Entities;
using PCIShieldLib.SharedKernel;
using System.Text.Json;

namespace PCIShield.Domain.Events
{
    public class ServiceProviderCreatedEvent : BaseDomainEvent
    {
        public ServiceProviderCreatedEvent(ServiceProvider serviceProvider, string action)
        {
            ActionOnMessageReceived = action;
            
            EntityNameType = "ServiceProvider";
            
            Content = System.Text.Json.JsonSerializer.Serialize(serviceProvider, JsonSerializerSettingsSingleton.Instance);
            
            EventType = "ServiceProviderCreated";
            
            EventId = UuidV7Generator.NewUuidV7();
            
            OccurredOnUtc = DateTime.UtcNow;
        }
    }
}

