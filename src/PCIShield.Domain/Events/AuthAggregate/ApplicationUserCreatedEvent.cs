using System;
using PCIShield.Domain.Entities;
using PCIShieldLib.SharedKernel;
using System.Text.Json;

namespace PCIShield.Domain.Events
{
    public class ApplicationUserCreatedEvent : BaseDomainEvent
    {
        public ApplicationUserCreatedEvent(ApplicationUser applicationUser, string action)
        {
            ActionOnMessageReceived = action;
            
            EntityNameType = "ApplicationUser";
            
            Content = System.Text.Json.JsonSerializer.Serialize(applicationUser, JsonSerializerSettingsSingleton.Instance);
            
            EventType = "ApplicationUserCreated";
            
            EventId = UuidV7Generator.NewUuidV7();
            
            OccurredOnUtc = DateTime.UtcNow;
        }
    }
}

