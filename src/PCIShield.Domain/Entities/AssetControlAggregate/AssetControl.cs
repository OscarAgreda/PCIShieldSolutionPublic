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
    
    public class AssetControl : BaseEntityEv<int>, IAggregateRoot, ITrackedEntity, ITenantEntity
    {
        [Key]
        public int RowId { get; private set; }
        
        public Guid TenantId { get; private set; }
        
        public bool IsApplicable { get; private set; }
        
        public string? CustomizedApproach { get; private set; }
        
        public DateTime CreatedAt { get; private set; }
        
        public Guid CreatedBy { get; private set; }
        
        public DateTime? UpdatedAt { get; private set; }
        
        public Guid? UpdatedBy { get; private set; }
        
        public bool IsDeleted { get; private set; }
        
        public void SetRowId(int rowId)
        {
            RowId = Guard.Against.Negative(rowId, nameof(rowId));
        }
        public void SetAssetId(Guid assetId)
        {
            AssetId = Guard.Against.Default(assetId, nameof(assetId));
        }
        public void SetControlId(Guid controlId)
        {
            ControlId = Guard.Against.Default(controlId, nameof(controlId));
        }
        public void SetTenantId(Guid tenantId)
        {
            TenantId = Guard.Against.Default(tenantId, nameof(tenantId));
        }
        public void SetIsApplicable(bool isApplicable)
        {
            IsApplicable = Guard.Against.Null(isApplicable, nameof(isApplicable));
        }
        public void SetCustomizedApproach(string customizedApproach)
        {
            CustomizedApproach = customizedApproach;
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
        public virtual Asset Asset { get; private set; }
        
        public Guid AssetId { get; private set; }
        
        public virtual Control Control { get; private set; }
        
        public Guid ControlId { get; private set; }
        
        private AssetControl() { }
        
        public AssetControl(Guid assetId, Guid controlId, Guid tenantId, bool isApplicable, DateTime createdAt, Guid createdBy, bool isDeleted)
        {
            this.AssetId = Guard.Against.Default(assetId, nameof(assetId));
            this.ControlId = Guard.Against.Default(controlId, nameof(controlId));
            this.TenantId = Guard.Against.Default(tenantId, nameof(tenantId));
            this.IsApplicable = Guard.Against.Null(isApplicable, nameof(isApplicable));
            this.CreatedAt = Guard.Against.OutOfSQLDateRange(createdAt, nameof(createdAt));
            this.CreatedBy = Guard.Against.Default(createdBy, nameof(createdBy));
            this.IsDeleted = Guard.Against.Null(isDeleted, nameof(isDeleted));
            
        }
        public override bool Equals(object? obj) =>
        obj is AssetControl assetControl && Equals(assetControl);
        
        public bool Equals(AssetControl other)
        {
            if (ReferenceEquals(null, other))
            {
                return false;
            }
            
            if (ReferenceEquals(this, other))
            {
                return true;
            }
            
            return RowId.Equals(other.RowId);
        }
        
        public override int GetHashCode() => RowId.GetHashCode();
        
        public static bool operator !=(AssetControl left, AssetControl right) => !(left == right);
        
        public static bool operator ==(AssetControl left, AssetControl right) => left?.Equals(right) ?? right is null;
        
        private void ValidateInvariants()
        {
            if (CustomizedApproach?.Length > -1)
            throw new InvalidOperationException("CustomizedApproach cannot exceed -1 characters.");
            if (UpdatedAt == default)
            throw new InvalidOperationException("UpdatedAt must be set.");
        }
        
    }
}

