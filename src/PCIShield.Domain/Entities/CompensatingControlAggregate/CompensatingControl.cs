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
    
    public class CompensatingControl : BaseEntityEv<Guid>, IAggregateRoot, ITrackedEntity, ITenantEntity
    {
        [Key]
        public Guid CompensatingControlId { get; private set; }
        
        public Guid TenantId { get; private set; }
        
        public string Justification { get; private set; }
        
        public string ImplementationDetails { get; private set; }
        
        public Guid? ApprovedBy { get; private set; }
        
        public DateTime? ApprovalDate { get; private set; }
        
        public DateTime ExpiryDate { get; private set; }
        
        public int Rank { get; private set; }
        
        public DateTime CreatedAt { get; private set; }
        
        public Guid CreatedBy { get; private set; }
        
        public DateTime? UpdatedAt { get; private set; }
        
        public Guid? UpdatedBy { get; private set; }
        
        public bool IsDeleted { get; private set; }
        
        public void SetTenantId(Guid tenantId)
        {
            TenantId = Guard.Against.Default(tenantId, nameof(tenantId));
        }
        public void SetControlId(Guid controlId)
        {
            ControlId = Guard.Against.Default(controlId, nameof(controlId));
        }
        public void SetMerchantId(Guid merchantId)
        {
            MerchantId = Guard.Against.Default(merchantId, nameof(merchantId));
        }
        public void SetJustification(string justification)
        {
            Justification = Guard.Against.NullOrEmpty(justification, nameof(justification));
        }
        public void SetImplementationDetails(string implementationDetails)
        {
            ImplementationDetails = Guard.Against.NullOrEmpty(implementationDetails, nameof(implementationDetails));
        }
        public void SetApprovedBy(Guid? approvedBy)
        {
            ApprovedBy = approvedBy;
        }
        public void SetApprovalDate(DateTime? approvalDate)
        {
            ApprovalDate = approvalDate;
        }
        public void SetExpiryDate(DateTime expiryDate)
        {
            ExpiryDate = Guard.Against.OutOfSQLDateRange(expiryDate, nameof(expiryDate));
        }
        public void SetRank(int rank)
        {
            Rank = Guard.Against.Negative(rank, nameof(rank));
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
        public virtual Control Control { get; private set; }
        
        public Guid ControlId { get; private set; }
        
        public virtual Merchant Merchant { get; private set; }
        
        public Guid MerchantId { get; private set; }
        
        private CompensatingControl() { }
        
        public CompensatingControl(Guid compensatingControlId, Guid controlId, Guid merchantId, Guid tenantId, string justification, string implementationDetails, DateTime expiryDate, int rank, DateTime createdAt, Guid createdBy, bool isDeleted)
        {
            this.CompensatingControlId = Guard.Against.Default(compensatingControlId, nameof(compensatingControlId));
            this.ControlId = Guard.Against.Default(controlId, nameof(controlId));
            this.MerchantId = Guard.Against.Default(merchantId, nameof(merchantId));
            this.TenantId = Guard.Against.Default(tenantId, nameof(tenantId));
            this.Justification = Guard.Against.NullOrEmpty(justification, nameof(justification));
            this.ImplementationDetails = Guard.Against.NullOrEmpty(implementationDetails, nameof(implementationDetails));
            this.ExpiryDate = Guard.Against.OutOfSQLDateRange(expiryDate, nameof(expiryDate));
            this.Rank = Guard.Against.Negative(rank, nameof(rank));
            this.CreatedAt = Guard.Against.OutOfSQLDateRange(createdAt, nameof(createdAt));
            this.CreatedBy = Guard.Against.Default(createdBy, nameof(createdBy));
            this.IsDeleted = Guard.Against.Null(isDeleted, nameof(isDeleted));
            
        }
        public override bool Equals(object? obj) =>
        obj is CompensatingControl compensatingControl && Equals(compensatingControl);
        
        public bool Equals(CompensatingControl other)
        {
            if (ReferenceEquals(null, other))
            {
                return false;
            }
            
            if (ReferenceEquals(this, other))
            {
                return true;
            }
            
            return CompensatingControlId.Equals(other.CompensatingControlId);
        }
        
        public override int GetHashCode() => CompensatingControlId.GetHashCode();
        
        public static bool operator !=(CompensatingControl left, CompensatingControl right) => !(left == right);
        
        public static bool operator ==(CompensatingControl left, CompensatingControl right) => left?.Equals(right) ?? right is null;
        
        private void ValidateInvariants()
        {
            if (string.IsNullOrWhiteSpace(Justification))
            throw new InvalidOperationException("Justification cannot be null or whitespace.");
            if (Justification?.Length > -1)
            throw new InvalidOperationException("Justification cannot exceed -1 characters.");
            if (string.IsNullOrWhiteSpace(ImplementationDetails))
            throw new InvalidOperationException("ImplementationDetails cannot be null or whitespace.");
            if (ImplementationDetails?.Length > -1)
            throw new InvalidOperationException("ImplementationDetails cannot exceed -1 characters.");
            if (ApprovalDate == default)
            throw new InvalidOperationException("ApprovalDate must be set.");
            if (ExpiryDate == default)
            throw new InvalidOperationException("ExpiryDate must be set.");
            if (UpdatedAt == default)
            throw new InvalidOperationException("UpdatedAt must be set.");
        }
        
    }
}

