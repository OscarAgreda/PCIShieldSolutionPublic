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
    
    public class ControlEvidence : BaseEntityEv<int>, IAggregateRoot, ITenantEntity
    {
        [Key]
        public int RowId { get; private set; }
        
        public Guid TenantId { get; private set; }
        
        public bool IsPrimary { get; private set; }
        
        public DateTime CreatedAt { get; private set; }
        
        public Guid CreatedBy { get; private set; }
        
        public DateTime? UpdatedAt { get; private set; }
        
        public Guid? UpdatedBy { get; private set; }
        
        public bool IsDeleted { get; private set; }
        
        public void SetRowId(int rowId)
        {
            RowId = Guard.Against.Negative(rowId, nameof(rowId));
        }
        public void SetControlId(Guid controlId)
        {
            ControlId = Guard.Against.Default(controlId, nameof(controlId));
        }
        public void SetEvidenceId(Guid evidenceId)
        {
            EvidenceId = Guard.Against.Default(evidenceId, nameof(evidenceId));
        }
        public void SetAssessmentId(Guid assessmentId)
        {
            AssessmentId = Guard.Against.Default(assessmentId, nameof(assessmentId));
        }
        public void SetTenantId(Guid tenantId)
        {
            TenantId = Guard.Against.Default(tenantId, nameof(tenantId));
        }
        public void SetIsPrimary(bool isPrimary)
        {
            IsPrimary = Guard.Against.Null(isPrimary, nameof(isPrimary));
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
        
        public virtual Assessment Assessment { get; private set; }
        
        public Guid AssessmentId { get; private set; }
        
        public virtual Control Control { get; private set; }
        
        public Guid ControlId { get; private set; }
        
        public virtual Evidence Evidence { get; private set; }
        
        public Guid EvidenceId { get; private set; }
        
        private ControlEvidence() { }
        
        public ControlEvidence(Guid assessmentId, Guid controlId, Guid evidenceId, Guid tenantId, bool isPrimary, DateTime createdAt, Guid createdBy, bool isDeleted)
        {
            this.AssessmentId = Guard.Against.Default(assessmentId, nameof(assessmentId));
            this.ControlId = Guard.Against.Default(controlId, nameof(controlId));
            this.EvidenceId = Guard.Against.Default(evidenceId, nameof(evidenceId));
            this.TenantId = Guard.Against.Default(tenantId, nameof(tenantId));
            this.IsPrimary = Guard.Against.Null(isPrimary, nameof(isPrimary));
            this.CreatedAt = Guard.Against.OutOfSQLDateRange(createdAt, nameof(createdAt));
            this.CreatedBy = Guard.Against.Default(createdBy, nameof(createdBy));
            this.IsDeleted = Guard.Against.Null(isDeleted, nameof(isDeleted));
            
        }
        public override bool Equals(object? obj) =>
        obj is ControlEvidence controlEvidence && Equals(controlEvidence);
        
        public bool Equals(ControlEvidence other)
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
        
        public static bool operator !=(ControlEvidence left, ControlEvidence right) => !(left == right);
        
        public static bool operator ==(ControlEvidence left, ControlEvidence right) => left?.Equals(right) ?? right is null;
        
        private void ValidateInvariants()
        {
            if (UpdatedAt == default)
            throw new InvalidOperationException("UpdatedAt must be set.");
        }
        
    }
}

