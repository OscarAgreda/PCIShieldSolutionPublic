using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;
using System.Linq;
using Ardalis.GuardClauses;
using System.Collections.Immutable;
using System.Text.Json;
using LanguageExt;
using static LanguageExt.Prelude;
using PCIShield.Domain.Exceptions;
using PCIShield.Domain.Events;
using PCIShieldLib.SharedKernel;
using PCIShieldLib.SharedKernel.Interfaces;

namespace PCIShield.Domain.Entities
{
    
    public class AuditLog : BaseEntityEv<Guid>, IAggregateRoot, ITenantEntity
    {
        [Key]
        public Guid AuditLogId { get; private set; }
        
        public Guid TenantId { get; private set; }
        
        public string EntityType { get; private set; }
        
        public Guid EntityId { get; private set; }
        
        public string Action { get; private set; }
        
        public string? OldValues { get; private set; }
        
        public string? NewValues { get; private set; }
        
        public Guid UserId { get; private set; }
        
        public string? IPAddress { get; private set; }
        
        public void SetTenantId(Guid tenantId)
        {
            TenantId = Guard.Against.Default(tenantId, nameof(tenantId));
        }
        public void SetEntityType(string entityType)
        {
            EntityType = Guard.Against.NullOrEmpty(entityType, nameof(entityType));
        }
        public void SetEntityId(Guid entityId)
        {
            EntityId = Guard.Against.Default(entityId, nameof(entityId));
        }
        public void SetAction(string action)
        {
            Action = Guard.Against.NullOrEmpty(action, nameof(action));
        }
        public void SetOldValues(string oldValues)
        {
            OldValues = oldValues;
        }
        public void SetNewValues(string newValues)
        {
            NewValues = newValues;
        }
        public void SetUserId(Guid userId)
        {
            UserId = Guard.Against.Default(userId, nameof(userId));
        }
        public void SetIPAddress(string ipaddress)
        {
            IPAddress = ipaddress;
        }
        private AuditLog() { }
        
        public AuditLog(Guid auditLogId, Guid tenantId, string entityType, Guid entityId, string action, Guid userId)
        {
            this.AuditLogId = Guard.Against.Default(auditLogId, nameof(auditLogId));
            this.TenantId = Guard.Against.Default(tenantId, nameof(tenantId));
            this.EntityType = Guard.Against.NullOrEmpty(entityType, nameof(entityType));
            this.EntityId = Guard.Against.Default(entityId, nameof(entityId));
            this.Action = Guard.Against.NullOrEmpty(action, nameof(action));
            this.UserId = Guard.Against.Default(userId, nameof(userId));
            
        }
        public override bool Equals(object? obj) =>
        obj is AuditLog auditLog && Equals(auditLog);
        
        public bool Equals(AuditLog other)
        {
            if (ReferenceEquals(null, other))
            {
                return false;
            }
            
            if (ReferenceEquals(this, other))
            {
                return true;
            }
            
            return AuditLogId.Equals(other.AuditLogId);
        }
        
        public override int GetHashCode() => AuditLogId.GetHashCode();
        
        public static bool operator !=(AuditLog left, AuditLog right) => !(left == right);
        
        public static bool operator ==(AuditLog left, AuditLog right) => left?.Equals(right) ?? right is null;
        
        private void ValidateInvariants()
        {
            if (string.IsNullOrWhiteSpace(EntityType))
            throw new InvalidOperationException("EntityType cannot be null or whitespace.");
            if (EntityType?.Length > 100)
            throw new InvalidOperationException("EntityType cannot exceed 100 characters.");
            if (string.IsNullOrWhiteSpace(Action))
            throw new InvalidOperationException("Action cannot be null or whitespace.");
            if (Action?.Length > 50)
            throw new InvalidOperationException("Action cannot exceed 50 characters.");
            if (OldValues?.Length > -1)
            throw new InvalidOperationException("OldValues cannot exceed -1 characters.");
            if (NewValues?.Length > -1)
            throw new InvalidOperationException("NewValues cannot exceed -1 characters.");
            if (IPAddress?.Length > 45)
            throw new InvalidOperationException("IPAddress cannot exceed 45 characters.");
        }
        
    }
}

