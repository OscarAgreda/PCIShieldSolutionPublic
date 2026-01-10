using System;
using PCIShield.Domain.Entities;
using PCIShieldLib.SharedKernel;
using System.Text.Json;

namespace PCIShield.Domain.Events
{
    public class AuditLogCreatedEvent : BaseDomainEvent
    {
        public AuditLogCreatedEvent(AuditLog auditLog, string action)
        {
            ActionOnMessageReceived = action;
            
            EntityNameType = "AuditLog";
            
            Content = System.Text.Json.JsonSerializer.Serialize(auditLog, JsonSerializerSettingsSingleton.Instance);
            
            EventType = "AuditLogCreated";
            
            EventId = UuidV7Generator.NewUuidV7();
            
            OccurredOnUtc = DateTime.UtcNow;
        }
    }
}

