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
    
    public class Script : BaseEntityEv<Guid>, IAggregateRoot, ITrackedEntity, ITenantEntity
    {
        [Key]
        public Guid ScriptId { get; private set; }
        
        public Guid TenantId { get; private set; }
        
        public string ScriptUrl { get; private set; }
        
        public string ScriptHash { get; private set; }
        
        public string ScriptType { get; private set; }
        
        public bool IsAuthorized { get; private set; }
        
        public DateTime FirstSeen { get; private set; }
        
        public DateTime LastSeen { get; private set; }
        
        public DateTime CreatedAt { get; private set; }
        
        public Guid CreatedBy { get; private set; }
        
        public DateTime? UpdatedAt { get; private set; }
        
        public Guid? UpdatedBy { get; private set; }
        
        public bool IsDeleted { get; private set; }
        
        public void SetTenantId(Guid tenantId)
        {
            TenantId = Guard.Against.Default(tenantId, nameof(tenantId));
        }
        public void SetPaymentPageId(Guid paymentPageId)
        {
            PaymentPageId = Guard.Against.Default(paymentPageId, nameof(paymentPageId));
        }
        public void SetScriptUrl(string scriptUrl)
        {
            ScriptUrl = Guard.Against.NullOrEmpty(scriptUrl, nameof(scriptUrl));
        }
        public void SetScriptHash(string scriptHash)
        {
            ScriptHash = Guard.Against.NullOrEmpty(scriptHash, nameof(scriptHash));
        }
        public void SetScriptType(string scriptType)
        {
            ScriptType = Guard.Against.NullOrEmpty(scriptType, nameof(scriptType));
        }
        public void SetIsAuthorized(bool isAuthorized)
        {
            IsAuthorized = Guard.Against.Null(isAuthorized, nameof(isAuthorized));
        }
        public void SetFirstSeen(DateTime firstSeen)
        {
            FirstSeen = Guard.Against.OutOfSQLDateRange(firstSeen, nameof(firstSeen));
        }
        public void SetLastSeen(DateTime lastSeen)
        {
            LastSeen = Guard.Against.OutOfSQLDateRange(lastSeen, nameof(lastSeen));
        }
        public void SetCreatedAt(DateTime createdAt)
        {
            CreatedAt = Guard.Against.OutOfSQLDateRange(createdAt, nameof(createdAt));
        }
        public void SetCreatedBy(Guid createdBy)
        {
            CreatedBy = Guard.Against.Default(createdBy, nameof(createdBy));
        }
        public void SetUpdatedAt(DateTime? updatedAt)
        {
            UpdatedAt = updatedAt;
        }
        public void SetUpdatedBy(Guid? updatedBy)
        {
            UpdatedBy = updatedBy;
        }
        public void SetIsDeleted(bool isDeleted)
        {
            IsDeleted = Guard.Against.Null(isDeleted, nameof(isDeleted));
        }
        
        public virtual PaymentPage PaymentPage { get; private set; }
        
        public Guid PaymentPageId { get; private set; }
        
        private Script() { }
        
        public Script(Guid scriptId, Guid paymentPageId, Guid tenantId, string scriptUrl, string scriptHash, string scriptType, bool isAuthorized, DateTime firstSeen, DateTime lastSeen, DateTime createdAt, Guid createdBy, bool isDeleted)
        {
            this.ScriptId = Guard.Against.Default(scriptId, nameof(scriptId));
            this.PaymentPageId = Guard.Against.Default(paymentPageId, nameof(paymentPageId));
            this.TenantId = Guard.Against.Default(tenantId, nameof(tenantId));
            this.ScriptUrl = Guard.Against.NullOrEmpty(scriptUrl, nameof(scriptUrl));
            this.ScriptHash = Guard.Against.NullOrEmpty(scriptHash, nameof(scriptHash));
            this.ScriptType = Guard.Against.NullOrEmpty(scriptType, nameof(scriptType));
            this.IsAuthorized = Guard.Against.Null(isAuthorized, nameof(isAuthorized));
            this.FirstSeen = Guard.Against.OutOfSQLDateRange(firstSeen, nameof(firstSeen));
            this.LastSeen = Guard.Against.OutOfSQLDateRange(lastSeen, nameof(lastSeen));
            this.CreatedAt = Guard.Against.OutOfSQLDateRange(createdAt, nameof(createdAt));
            this.CreatedBy = Guard.Against.Default(createdBy, nameof(createdBy));
            this.IsDeleted = Guard.Against.Null(isDeleted, nameof(isDeleted));
            
        }
        public override bool Equals(object? obj) =>
        obj is Script script && Equals(script);
        
        public bool Equals(Script other)
        {
            if (ReferenceEquals(null, other))
            {
                return false;
            }
            
            if (ReferenceEquals(this, other))
            {
                return true;
            }
            
            return ScriptId.Equals(other.ScriptId);
        }
        
        public override int GetHashCode() => ScriptId.GetHashCode();
        
        public static bool operator !=(Script left, Script right) => !(left == right);
        
        public static bool operator ==(Script left, Script right) => left?.Equals(right) ?? right is null;
        
        private void ValidateInvariants()
        {
            if (string.IsNullOrWhiteSpace(ScriptUrl))
            throw new InvalidOperationException("ScriptUrl cannot be null or whitespace.");
            if (ScriptUrl?.Length > 500)
            throw new InvalidOperationException("ScriptUrl cannot exceed 500 characters.");
            if (string.IsNullOrWhiteSpace(ScriptHash))
            throw new InvalidOperationException("ScriptHash cannot be null or whitespace.");
            if (ScriptHash?.Length > 128)
            throw new InvalidOperationException("ScriptHash cannot exceed 128 characters.");
            if (string.IsNullOrWhiteSpace(ScriptType))
            throw new InvalidOperationException("ScriptType cannot be null or whitespace.");
            if (ScriptType?.Length > 50)
            throw new InvalidOperationException("ScriptType cannot exceed 50 characters.");
            if (UpdatedAt == default)
            throw new InvalidOperationException("UpdatedAt must be set.");
        }
        
    }
}

