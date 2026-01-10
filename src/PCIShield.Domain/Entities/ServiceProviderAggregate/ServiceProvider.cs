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
    
    public class ServiceProvider : BaseEntityEv<Guid>, IAggregateRoot, ITrackedEntity, ITenantEntity
    {
        [Key]
        public Guid ServiceProviderId { get; private set; }
        
        public Guid TenantId { get; private set; }
        
        public string ProviderName { get; private set; }
        
        public string ServiceType { get; private set; }
        
        public bool IsPCICompliant { get; private set; }
        
        public DateTime? AOCExpiryDate { get; private set; }
        
        public string? ResponsibilityMatrix { get; private set; }
        
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
        public void SetProviderName(string providerName)
        {
            ProviderName = Guard.Against.NullOrEmpty(providerName, nameof(providerName));
        }
        public void SetServiceType(string serviceType)
        {
            ServiceType = Guard.Against.NullOrEmpty(serviceType, nameof(serviceType));
        }
        public void SetIsPCICompliant(bool isPcicompliant)
        {
            IsPCICompliant = Guard.Against.Null(isPcicompliant, nameof(isPcicompliant));
        }
        public void SetAOCExpiryDate(DateTime? aocexpiryDate)
        {
            AOCExpiryDate = aocexpiryDate;
        }
        public void SetResponsibilityMatrix(string responsibilityMatrix)
        {
            ResponsibilityMatrix = responsibilityMatrix;
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
        
        private ServiceProvider() { }
        
        public ServiceProvider(Guid serviceProviderId, Guid merchantId, Guid tenantId, string providerName, string serviceType, bool isPcicompliant, DateTime createdAt, Guid createdBy, bool isDeleted)
        {
            this.ServiceProviderId = Guard.Against.Default(serviceProviderId, nameof(serviceProviderId));
            this.MerchantId = Guard.Against.Default(merchantId, nameof(merchantId));
            this.TenantId = Guard.Against.Default(tenantId, nameof(tenantId));
            this.ProviderName = Guard.Against.NullOrEmpty(providerName, nameof(providerName));
            this.ServiceType = Guard.Against.NullOrEmpty(serviceType, nameof(serviceType));
            this.IsPCICompliant = Guard.Against.Null(isPcicompliant, nameof(isPcicompliant));
            this.CreatedAt = Guard.Against.OutOfSQLDateRange(createdAt, nameof(createdAt));
            this.CreatedBy = Guard.Against.Default(createdBy, nameof(createdBy));
            this.IsDeleted = Guard.Against.Null(isDeleted, nameof(isDeleted));
            
        }
        public override bool Equals(object? obj) =>
        obj is ServiceProvider serviceProvider && Equals(serviceProvider);
        
        public bool Equals(ServiceProvider other)
        {
            if (ReferenceEquals(null, other))
            {
                return false;
            }
            
            if (ReferenceEquals(this, other))
            {
                return true;
            }
            
            return ServiceProviderId.Equals(other.ServiceProviderId);
        }
        
        public override int GetHashCode() => ServiceProviderId.GetHashCode();
        
        public static bool operator !=(ServiceProvider left, ServiceProvider right) => !(left == right);
        
        public static bool operator ==(ServiceProvider left, ServiceProvider right) => left?.Equals(right) ?? right is null;
        
        private void ValidateInvariants()
        {
            if (string.IsNullOrWhiteSpace(ProviderName))
            throw new InvalidOperationException("ProviderName cannot be null or whitespace.");
            if (ProviderName?.Length > 200)
            throw new InvalidOperationException("ProviderName cannot exceed 200 characters.");
            if (string.IsNullOrWhiteSpace(ServiceType))
            throw new InvalidOperationException("ServiceType cannot be null or whitespace.");
            if (ServiceType?.Length > 100)
            throw new InvalidOperationException("ServiceType cannot exceed 100 characters.");
            if (AOCExpiryDate == default)
            throw new InvalidOperationException("AOCExpiryDate must be set.");
            if (ResponsibilityMatrix?.Length > -1)
            throw new InvalidOperationException("ResponsibilityMatrix cannot exceed -1 characters.");
            if (UpdatedAt == default)
            throw new InvalidOperationException("UpdatedAt must be set.");
        }
        
    }
}

