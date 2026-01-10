using System;
using PCIShield.Domain.Entities;
using PCIShieldLib.SharedKernel;
using System.Text.Json;

namespace PCIShield.Domain.Events
{
    public class ApplicationUserUpdatedEvent : BaseDomainEvent
    {
        public ApplicationUserUpdatedEvent(ApplicationUser applicationUser, string action)
        {
            ActionOnMessageReceived = action;
            
            EntityNameType = "ApplicationUser";
            
            Content = System.Text.Json.JsonSerializer.Serialize(applicationUser, JsonSerializerSettingsSingleton.Instance);
            
            EventType = "ApplicationUserUpdated";
            
            EventId = UuidV7Generator.NewUuidV7();
            
            OccurredOnUtc = DateTime.UtcNow;
        }
    }
}

