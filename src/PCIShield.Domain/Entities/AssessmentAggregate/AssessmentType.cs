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
    
    public class AssessmentType : BaseEntityEv<Guid>, IAggregateRoot, ITrackedEntity
    {
        [Key]
        public Guid AssessmentTypeId { get; private set; }
        
        public string AssessmentTypeCode { get; private set; }
        
        public string AssessmentTypeName { get; private set; }
        
        public string? Description { get; private set; }
        
        public bool IsActive { get; private set; }
        
        public DateTime CreatedAt { get; private set; }
        
        public Guid CreatedBy { get; private set; }
        
        public DateTime? UpdatedAt { get; private set; }
        
        public Guid? UpdatedBy { get; private set; }
        
        public void SetAssessmentTypeCode(string assessmentTypeCode)
        {
            AssessmentTypeCode = Guard.Against.NullOrEmpty(assessmentTypeCode, nameof(assessmentTypeCode));
        }
        public void SetAssessmentTypeName(string assessmentTypeName)
        {
            AssessmentTypeName = Guard.Against.NullOrEmpty(assessmentTypeName, nameof(assessmentTypeName));
        }
        public void SetDescription(string description)
        {
            Description = description;
        }
        public void SetIsActive(bool isActive)
        {
            IsActive = Guard.Against.Null(isActive, nameof(isActive));
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
        private AssessmentType() { }
        
        public AssessmentType(Guid assessmentTypeId, string assessmentTypeCode, string assessmentTypeName, bool isActive, DateTime createdAt, Guid createdBy)
        {
            this.AssessmentTypeId = Guard.Against.Default(assessmentTypeId, nameof(assessmentTypeId));
            this.AssessmentTypeCode = Guard.Against.NullOrEmpty(assessmentTypeCode, nameof(assessmentTypeCode));
            this.AssessmentTypeName = Guard.Against.NullOrEmpty(assessmentTypeName, nameof(assessmentTypeName));
            this.IsActive = Guard.Against.Null(isActive, nameof(isActive));
            this.CreatedAt = Guard.Against.OutOfSQLDateRange(createdAt, nameof(createdAt));
            this.CreatedBy = Guard.Against.Default(createdBy, nameof(createdBy));
            
        }
        public override bool Equals(object? obj) =>
        obj is AssessmentType assessmentType && Equals(assessmentType);
        
        public bool Equals(AssessmentType other)
        {
            if (ReferenceEquals(null, other))
            {
                return false;
            }
            
            if (ReferenceEquals(this, other))
            {
                return true;
            }
            
            return AssessmentTypeId.Equals(other.AssessmentTypeId);
        }
        
        public override int GetHashCode() => AssessmentTypeId.GetHashCode();
        
        public static bool operator !=(AssessmentType left, AssessmentType right) => !(left == right);
        
        public static bool operator ==(AssessmentType left, AssessmentType right) => left?.Equals(right) ?? right is null;
        
        private void ValidateInvariants()
        {
            if (string.IsNullOrWhiteSpace(AssessmentTypeCode))
            throw new InvalidOperationException("AssessmentTypeCode cannot be null or whitespace.");
            if (AssessmentTypeCode?.Length > 30)
            throw new InvalidOperationException("AssessmentTypeCode cannot exceed 30 characters.");
            if (string.IsNullOrWhiteSpace(AssessmentTypeName))
            throw new InvalidOperationException("AssessmentTypeName cannot be null or whitespace.");
            if (AssessmentTypeName?.Length > 100)
            throw new InvalidOperationException("AssessmentTypeName cannot exceed 100 characters.");
            if (Description?.Length > 500)
            throw new InvalidOperationException("Description cannot exceed 500 characters.");
            if (UpdatedAt == default)
            throw new InvalidOperationException("UpdatedAt must be set.");
        }
        
    }
}

