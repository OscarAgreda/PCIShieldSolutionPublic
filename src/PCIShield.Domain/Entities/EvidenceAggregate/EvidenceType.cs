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
    
    public class EvidenceType : BaseEntityEv<Guid>, IAggregateRoot, ITrackedEntity
    {
        [Key]
        public Guid EvidenceTypeId { get; private set; }
        
        public string EvidenceTypeCode { get; private set; }
        
        public string EvidenceTypeName { get; private set; }
        
        public string? FileExtensions { get; private set; }
        
        public int? MaxSizeMB { get; private set; }
        
        public bool IsActive { get; private set; }
        
        public DateTime CreatedAt { get; private set; }
        
        public Guid CreatedBy { get; private set; }
        
        public DateTime? UpdatedAt { get; private set; }
        
        public Guid? UpdatedBy { get; private set; }
        
        public void SetEvidenceTypeCode(string evidenceTypeCode)
        {
            EvidenceTypeCode = Guard.Against.NullOrEmpty(evidenceTypeCode, nameof(evidenceTypeCode));
        }
        public void SetEvidenceTypeName(string evidenceTypeName)
        {
            EvidenceTypeName = Guard.Against.NullOrEmpty(evidenceTypeName, nameof(evidenceTypeName));
        }
        public void SetFileExtensions(string fileExtensions)
        {
            FileExtensions = fileExtensions;
        }
        public void SetMaxSizeMB(int? maxSizeMB)
        {
            MaxSizeMB = maxSizeMB;
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
        private EvidenceType() { }
        
        public EvidenceType(Guid evidenceTypeId, string evidenceTypeCode, string evidenceTypeName, bool isActive, DateTime createdAt, Guid createdBy)
        {
            this.EvidenceTypeId = Guard.Against.Default(evidenceTypeId, nameof(evidenceTypeId));
            this.EvidenceTypeCode = Guard.Against.NullOrEmpty(evidenceTypeCode, nameof(evidenceTypeCode));
            this.EvidenceTypeName = Guard.Against.NullOrEmpty(evidenceTypeName, nameof(evidenceTypeName));
            this.IsActive = Guard.Against.Null(isActive, nameof(isActive));
            this.CreatedAt = Guard.Against.OutOfSQLDateRange(createdAt, nameof(createdAt));
            this.CreatedBy = Guard.Against.Default(createdBy, nameof(createdBy));
            
        }
        public override bool Equals(object? obj) =>
        obj is EvidenceType evidenceType && Equals(evidenceType);
        
        public bool Equals(EvidenceType other)
        {
            if (ReferenceEquals(null, other))
            {
                return false;
            }
            
            if (ReferenceEquals(this, other))
            {
                return true;
            }
            
            return EvidenceTypeId.Equals(other.EvidenceTypeId);
        }
        
        public override int GetHashCode() => EvidenceTypeId.GetHashCode();
        
        public static bool operator !=(EvidenceType left, EvidenceType right) => !(left == right);
        
        public static bool operator ==(EvidenceType left, EvidenceType right) => left?.Equals(right) ?? right is null;
        
        private void ValidateInvariants()
        {
            if (string.IsNullOrWhiteSpace(EvidenceTypeCode))
            throw new InvalidOperationException("EvidenceTypeCode cannot be null or whitespace.");
            if (EvidenceTypeCode?.Length > 30)
            throw new InvalidOperationException("EvidenceTypeCode cannot exceed 30 characters.");
            if (string.IsNullOrWhiteSpace(EvidenceTypeName))
            throw new InvalidOperationException("EvidenceTypeName cannot be null or whitespace.");
            if (EvidenceTypeName?.Length > 100)
            throw new InvalidOperationException("EvidenceTypeName cannot exceed 100 characters.");
            if (FileExtensions?.Length > 200)
            throw new InvalidOperationException("FileExtensions cannot exceed 200 characters.");
            if (UpdatedAt == default)
            throw new InvalidOperationException("UpdatedAt must be set.");
        }
        
    }
}

