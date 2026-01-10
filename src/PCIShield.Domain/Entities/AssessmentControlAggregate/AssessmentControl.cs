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
    
    public class AssessmentControl : BaseEntityEv<int>, IAggregateRoot, ITrackedEntity, ITenantEntity
    {
        [Key]
        public int RowId { get; private set; }
        
        public Guid TenantId { get; private set; }
        
        public int TestResult { get; private set; }
        
        public DateTime? TestDate { get; private set; }
        
        public Guid? TestedBy { get; private set; }
        
        public string? Notes { get; private set; }
        
        public DateTime CreatedAt { get; private set; }
        
        public Guid CreatedBy { get; private set; }
        
        public DateTime? UpdatedAt { get; private set; }
        
        public Guid? UpdatedBy { get; private set; }
        
        public bool IsDeleted { get; private set; }
        
        public void SetRowId(int rowId)
        {
            RowId = Guard.Against.Negative(rowId, nameof(rowId));
        }
        public void SetAssessmentId(Guid assessmentId)
        {
            AssessmentId = Guard.Against.Default(assessmentId, nameof(assessmentId));
        }
        public void SetControlId(Guid controlId)
        {
            ControlId = Guard.Against.Default(controlId, nameof(controlId));
        }
        public void SetTenantId(Guid tenantId)
        {
            TenantId = Guard.Against.Default(tenantId, nameof(tenantId));
        }
        public void SetTestResult(int testResult)
        {
            TestResult = Guard.Against.Negative(testResult, nameof(testResult));
        }
        public void SetTestDate(DateTime? testDate)
        {
            TestDate = testDate;
        }
        public void SetTestedBy(Guid? testedBy)
        {
            TestedBy = testedBy;
        }
        public void SetNotes(string notes)
        {
            Notes = notes;
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
        
        private AssessmentControl() { }
        
        public AssessmentControl(Guid assessmentId, Guid controlId, Guid tenantId, int testResult, DateTime createdAt, Guid createdBy, bool isDeleted)
        {
            this.AssessmentId = Guard.Against.Default(assessmentId, nameof(assessmentId));
            this.ControlId = Guard.Against.Default(controlId, nameof(controlId));
            this.TenantId = Guard.Against.Default(tenantId, nameof(tenantId));
            this.TestResult = Guard.Against.Negative(testResult, nameof(testResult));
            this.CreatedAt = Guard.Against.OutOfSQLDateRange(createdAt, nameof(createdAt));
            this.CreatedBy = Guard.Against.Default(createdBy, nameof(createdBy));
            this.IsDeleted = Guard.Against.Null(isDeleted, nameof(isDeleted));
            
        }
        public override bool Equals(object? obj) =>
        obj is AssessmentControl assessmentControl && Equals(assessmentControl);
        
        public bool Equals(AssessmentControl other)
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
        
        public static bool operator !=(AssessmentControl left, AssessmentControl right) => !(left == right);
        
        public static bool operator ==(AssessmentControl left, AssessmentControl right) => left?.Equals(right) ?? right is null;
        
        private void ValidateInvariants()
        {
            if (TestDate == default)
            throw new InvalidOperationException("TestDate must be set.");
            if (Notes?.Length > -1)
            throw new InvalidOperationException("Notes cannot exceed -1 characters.");
            if (UpdatedAt == default)
            throw new InvalidOperationException("UpdatedAt must be set.");
        }
        
    }
}

