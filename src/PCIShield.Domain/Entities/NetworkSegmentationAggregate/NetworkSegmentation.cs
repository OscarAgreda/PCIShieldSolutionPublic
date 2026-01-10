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
    
    public class NetworkSegmentation : BaseEntityEv<Guid>, IAggregateRoot, ITrackedEntity, ITenantEntity
    {
        [Key]
        public Guid NetworkSegmentationId { get; private set; }
        
        public Guid TenantId { get; private set; }
        
        public string SegmentName { get; private set; }
        
        public int? VLANId { get; private set; }
        
        public string IPRange { get; private set; }
        
        public string? FirewallRules { get; private set; }
        
        public bool IsInCDE { get; private set; }
        
        public DateTime? LastValidated { get; private set; }
        
        public DateTime CreatedAt { get; private set; }
        
        public Guid CreatedBy { get; private set; }
        
        public DateTime? UpdatedAt { get; private set; }
        
        public Guid? UpdatedBy { get; private set; }
        
        public bool IsDeleted { get; private set; }
        
        public void SetTenantId(Guid tenantId)
        {
            TenantId = Guard.Against.Default(tenantId, nameof(tenantId));
        }
        public void SetMerchantId(Guid merchantId)
        {
            MerchantId = Guard.Against.Default(merchantId, nameof(merchantId));
        }
        public void SetSegmentName(string segmentName)
        {
            SegmentName = Guard.Against.NullOrEmpty(segmentName, nameof(segmentName));
        }
        public void SetVLANId(int? vlanid)
        {
            VLANId = vlanid;
        }
        public void SetIPRange(string iprange)
        {
            IPRange = Guard.Against.NullOrEmpty(iprange, nameof(iprange));
        }
        public void SetFirewallRules(string firewallRules)
        {
            FirewallRules = firewallRules;
        }
        public void SetIsInCDE(bool isInCDE)
        {
            IsInCDE = Guard.Against.Null(isInCDE, nameof(isInCDE));
        }
        public void SetLastValidated(DateTime? lastValidated)
        {
            LastValidated = lastValidated;
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
        
        public virtual Merchant Merchant { get; private set; }
        
        public Guid MerchantId { get; private set; }
        
        private NetworkSegmentation() { }
        
        public NetworkSegmentation(Guid networkSegmentationId, Guid merchantId, Guid tenantId, string segmentName, string iprange, bool isInCDE, DateTime createdAt, Guid createdBy, bool isDeleted)
        {
            this.NetworkSegmentationId = Guard.Against.Default(networkSegmentationId, nameof(networkSegmentationId));
            this.MerchantId = Guard.Against.Default(merchantId, nameof(merchantId));
            this.TenantId = Guard.Against.Default(tenantId, nameof(tenantId));
            this.SegmentName = Guard.Against.NullOrEmpty(segmentName, nameof(segmentName));
            this.IPRange = Guard.Against.NullOrEmpty(iprange, nameof(iprange));
            this.IsInCDE = Guard.Against.Null(isInCDE, nameof(isInCDE));
            this.CreatedAt = Guard.Against.OutOfSQLDateRange(createdAt, nameof(createdAt));
            this.CreatedBy = Guard.Against.Default(createdBy, nameof(createdBy));
            this.IsDeleted = Guard.Against.Null(isDeleted, nameof(isDeleted));
            
        }
        public override bool Equals(object? obj) =>
        obj is NetworkSegmentation networkSegmentation && Equals(networkSegmentation);
        
        public bool Equals(NetworkSegmentation other)
        {
            if (ReferenceEquals(null, other))
            {
                return false;
            }
            
            if (ReferenceEquals(this, other))
            {
                return true;
            }
            
            return NetworkSegmentationId.Equals(other.NetworkSegmentationId);
        }
        
        public override int GetHashCode() => NetworkSegmentationId.GetHashCode();
        
        public static bool operator !=(NetworkSegmentation left, NetworkSegmentation right) => !(left == right);
        
        public static bool operator ==(NetworkSegmentation left, NetworkSegmentation right) => left?.Equals(right) ?? right is null;
        
        private void ValidateInvariants()
        {
            if (string.IsNullOrWhiteSpace(SegmentName))
            throw new InvalidOperationException("SegmentName cannot be null or whitespace.");
            if (SegmentName?.Length > 100)
            throw new InvalidOperationException("SegmentName cannot exceed 100 characters.");
            if (string.IsNullOrWhiteSpace(IPRange))
            throw new InvalidOperationException("IPRange cannot be null or whitespace.");
            if (IPRange?.Length > 50)
            throw new InvalidOperationException("IPRange cannot exceed 50 characters.");
            if (FirewallRules?.Length > -1)
            throw new InvalidOperationException("FirewallRules cannot exceed -1 characters.");
            if (LastValidated == default)
            throw new InvalidOperationException("LastValidated must be set.");
            if (UpdatedAt == default)
            throw new InvalidOperationException("UpdatedAt must be set.");
        }
        
    }
}

