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
    
    public class ControlCategory : BaseEntityEv<Guid>, IAggregateRoot, ITrackedEntity
    {
        [Key]
        public Guid ControlCategoryId { get; private set; }
        
        public string ControlCategoryCode { get; private set; }
        
        public string ControlCategoryName { get; private set; }
        
        public string RequirementSection { get; private set; }
        
        public bool IsActive { get; private set; }
        
        public DateTime CreatedAt { get; private set; }
        
        public Guid CreatedBy { get; private set; }
        
        public DateTime? UpdatedAt { get; private set; }
        
        public Guid? UpdatedBy { get; private set; }
        
        public void SetControlCategoryCode(string controlCategoryCode)
        {
            ControlCategoryCode = Guard.Against.NullOrEmpty(controlCategoryCode, nameof(controlCategoryCode));
        }
        public void SetControlCategoryName(string controlCategoryName)
        {
            ControlCategoryName = Guard.Against.NullOrEmpty(controlCategoryName, nameof(controlCategoryName));
        }
        public void SetRequirementSection(string requirementSection)
        {
            RequirementSection = Guard.Against.NullOrEmpty(requirementSection, nameof(requirementSection));
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
        private ControlCategory() { }
        
        public ControlCategory(Guid controlCategoryId, string controlCategoryCode, string controlCategoryName, string requirementSection, bool isActive, DateTime createdAt, Guid createdBy)
        {
            this.ControlCategoryId = Guard.Against.Default(controlCategoryId, nameof(controlCategoryId));
            this.ControlCategoryCode = Guard.Against.NullOrEmpty(controlCategoryCode, nameof(controlCategoryCode));
            this.ControlCategoryName = Guard.Against.NullOrEmpty(controlCategoryName, nameof(controlCategoryName));
            this.RequirementSection = Guard.Against.NullOrEmpty(requirementSection, nameof(requirementSection));
            this.IsActive = Guard.Against.Null(isActive, nameof(isActive));
            this.CreatedAt = Guard.Against.OutOfSQLDateRange(createdAt, nameof(createdAt));
            this.CreatedBy = Guard.Against.Default(createdBy, nameof(createdBy));
            
        }
        public override bool Equals(object? obj) =>
        obj is ControlCategory controlCategory && Equals(controlCategory);
        
        public bool Equals(ControlCategory other)
        {
            if (ReferenceEquals(null, other))
            {
                return false;
            }
            
            if (ReferenceEquals(this, other))
            {
                return true;
            }
            
            return ControlCategoryId.Equals(other.ControlCategoryId);
        }
        
        public override int GetHashCode() => ControlCategoryId.GetHashCode();
        
        public static bool operator !=(ControlCategory left, ControlCategory right) => !(left == right);
        
        public static bool operator ==(ControlCategory left, ControlCategory right) => left?.Equals(right) ?? right is null;
        
        private void ValidateInvariants()
        {
            if (string.IsNullOrWhiteSpace(ControlCategoryCode))
            throw new InvalidOperationException("ControlCategoryCode cannot be null or whitespace.");
            if (ControlCategoryCode?.Length > 30)
            throw new InvalidOperationException("ControlCategoryCode cannot exceed 30 characters.");
            if (string.IsNullOrWhiteSpace(ControlCategoryName))
            throw new InvalidOperationException("ControlCategoryName cannot be null or whitespace.");
            if (ControlCategoryName?.Length > 100)
            throw new InvalidOperationException("ControlCategoryName cannot exceed 100 characters.");
            if (string.IsNullOrWhiteSpace(RequirementSection))
            throw new InvalidOperationException("RequirementSection cannot be null or whitespace.");
            if (RequirementSection?.Length > 20)
            throw new InvalidOperationException("RequirementSection cannot exceed 20 characters.");
            if (UpdatedAt == default)
            throw new InvalidOperationException("UpdatedAt must be set.");
        }
        
    }
}

