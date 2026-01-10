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
    
    public class ComplianceOfficer : BaseEntityEv<Guid>, IAggregateRoot, ITrackedEntity, ITenantEntity
    {
        [Key]
        public Guid ComplianceOfficerId { get; private set; }
        
        public Guid TenantId { get; private set; }
        
        public string OfficerCode { get; private set; }
        
        public string FirstName { get; private set; }
        
        public string LastName { get; private set; }
        
        public string Email { get; private set; }
        
        public string? Phone { get; private set; }
        
        public string? CertificationLevel { get; private set; }
        
        public bool IsActive { get; private set; }
        
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
        public void SetOfficerCode(string officerCode)
        {
            OfficerCode = Guard.Against.NullOrEmpty(officerCode, nameof(officerCode));
        }
        public void SetFirstName(string firstName)
        {
            FirstName = Guard.Against.NullOrEmpty(firstName, nameof(firstName));
        }
        public void SetLastName(string lastName)
        {
            LastName = Guard.Against.NullOrEmpty(lastName, nameof(lastName));
        }
        public void SetEmail(string email)
        {
            Email = Guard.Against.NullOrEmpty(email, nameof(email));
        }
        public void SetPhone(string phone)
        {
            Phone = phone;
        }
        public void SetCertificationLevel(string certificationLevel)
        {
            CertificationLevel = certificationLevel;
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
        public void SetIsDeleted(bool isDeleted)
        {
            IsDeleted = Guard.Against.Null(isDeleted, nameof(isDeleted));
        }
        
        public void UpdateMerchantForComplianceOfficer(Guid newMerchantId)
        {
            Guard.Against.Default(newMerchantId, nameof(newMerchantId));
            if (newMerchantId == MerchantId)
            {
                return;
            }
            
            MerchantId = newMerchantId;
            var complianceOfficerUpdatedEvent = new ComplianceOfficerUpdatedEvent(this, "UpdatedEvent merchant");
            Events.Add(complianceOfficerUpdatedEvent);
        }
        
        public virtual Merchant Merchant { get; private set; }
        
        public Guid MerchantId { get; private set; }
        
        private ComplianceOfficer() { }
        
        public ComplianceOfficer(Guid complianceOfficerId, Guid merchantId, Guid tenantId, string officerCode, string firstName, string lastName, string email, bool isActive, DateTime createdAt, Guid createdBy, bool isDeleted)
        {
            this.ComplianceOfficerId = Guard.Against.Default(complianceOfficerId, nameof(complianceOfficerId));
            this.MerchantId = Guard.Against.Default(merchantId, nameof(merchantId));
            this.TenantId = Guard.Against.Default(tenantId, nameof(tenantId));
            this.OfficerCode = Guard.Against.NullOrEmpty(officerCode, nameof(officerCode));
            this.FirstName = Guard.Against.NullOrEmpty(firstName, nameof(firstName));
            this.LastName = Guard.Against.NullOrEmpty(lastName, nameof(lastName));
            this.Email = Guard.Against.NullOrEmpty(email, nameof(email));
            this.IsActive = Guard.Against.Null(isActive, nameof(isActive));
            this.CreatedAt = Guard.Against.OutOfSQLDateRange(createdAt, nameof(createdAt));
            this.CreatedBy = Guard.Against.Default(createdBy, nameof(createdBy));
            this.IsDeleted = Guard.Against.Null(isDeleted, nameof(isDeleted));
            
        }
        public override bool Equals(object? obj) =>
        obj is ComplianceOfficer complianceOfficer && Equals(complianceOfficer);
        
        public bool Equals(ComplianceOfficer other)
        {
            if (ReferenceEquals(null, other))
            {
                return false;
            }
            
            if (ReferenceEquals(this, other))
            {
                return true;
            }
            
            return ComplianceOfficerId.Equals(other.ComplianceOfficerId);
        }
        
        public override int GetHashCode() => ComplianceOfficerId.GetHashCode();
        
        public static bool operator !=(ComplianceOfficer left, ComplianceOfficer right) => !(left == right);
        
        public static bool operator ==(ComplianceOfficer left, ComplianceOfficer right) => left?.Equals(right) ?? right is null;
        
        private void ValidateInvariants()
        {
            if (string.IsNullOrWhiteSpace(OfficerCode))
            throw new InvalidOperationException("OfficerCode cannot be null or whitespace.");
            if (OfficerCode?.Length > 30)
            throw new InvalidOperationException("OfficerCode cannot exceed 30 characters.");
            if (string.IsNullOrWhiteSpace(FirstName))
            throw new InvalidOperationException("FirstName cannot be null or whitespace.");
            if (FirstName?.Length > 100)
            throw new InvalidOperationException("FirstName cannot exceed 100 characters.");
            if (string.IsNullOrWhiteSpace(LastName))
            throw new InvalidOperationException("LastName cannot be null or whitespace.");
            if (LastName?.Length > 100)
            throw new InvalidOperationException("LastName cannot exceed 100 characters.");
            if (string.IsNullOrWhiteSpace(Email))
            throw new InvalidOperationException("Email cannot be null or whitespace.");
            if (Email?.Length > 320)
            throw new InvalidOperationException("Email cannot exceed 320 characters.");
            if (Phone?.Length > 32)
            throw new InvalidOperationException("Phone cannot exceed 32 characters.");
            if (CertificationLevel?.Length > 50)
            throw new InvalidOperationException("CertificationLevel cannot exceed 50 characters.");
            if (UpdatedAt == default)
            throw new InvalidOperationException("UpdatedAt must be set.");
        }
        
        private void RaiseDomainEvent(BaseDomainEvent domainEvent)
        {
            Events.Add(domainEvent);
        }
        public static Eff<Validation<string, ComplianceOfficer>> Create(
        Guid complianceOfficerId,
        Guid merchantId,
        Guid tenantId,
        string officerCode,
        string firstName,
        string lastName,
        string email,
        bool isActive,
        DateTime createdAt,
        Guid createdBy,
        bool isDeleted
        )
        {
            return Eff(() => CreateComplianceOfficer(
            complianceOfficerId,
            merchantId,
            tenantId,
            officerCode,
            firstName,
            lastName,
            email,
            isActive,
            createdAt,
            createdBy,
            isDeleted
            ));
        }
        
        private static Validation<string, ComplianceOfficer> CreateComplianceOfficer(
        Guid complianceOfficerId,
        Guid merchantId,
        Guid tenantId,
        string officerCode,
        string firstName,
        string lastName,
        string email,
        bool isActive,
        DateTime createdAt,
        Guid createdBy,
        bool isDeleted
        )
        {
            try
            {
                var now = DateTime.UtcNow;
                isDeleted = false;
                
                var complianceOfficer = new ComplianceOfficer
                ( complianceOfficerId,
                merchantId,
                tenantId,
                officerCode,
                firstName,
                lastName,
                email,
                isActive,
                createdAt,
                createdBy,
                isDeleted)
                ;
                
                return Validation<string, ComplianceOfficer>.Success(complianceOfficer);
            }
            catch (Exception ex)
            {
                return Validation<string, ComplianceOfficer>.Fail(new Seq<string> { ex.Message });
            }
        }
        public static Eff<Validation<string, ComplianceOfficer>> Update(
        ComplianceOfficer existingComplianceOfficer,
        Guid complianceOfficerId,
        Guid tenantId,
        Guid merchantId,
        string officerCode,
        string firstName,
        string lastName,
        string email,
        string phone,
        string certificationLevel,
        bool isActive,
        DateTime createdAt,
        Guid createdBy,
        DateTime? updatedAt,
        Guid? updatedBy,
        bool isDeleted
        )
        {
            return Eff(() => UpdateComplianceOfficer(
            existingComplianceOfficer,
            complianceOfficerId,
            tenantId,
            merchantId,
            officerCode,
            firstName,
            lastName,
            email,
            phone,
            certificationLevel,
            isActive,
            createdAt,
            createdBy,
            updatedAt.ToOption().ToNullable(),
            updatedBy.ToOption().ToNullable(),
            isDeleted
            ));
        }
        
        private static Validation<string, ComplianceOfficer> UpdateComplianceOfficer(
        ComplianceOfficer complianceOfficer,
        Guid complianceOfficerId,
        Guid tenantId,
        Guid merchantId,
        string officerCode,
        string firstName,
        string lastName,
        string email,
        string phone,
        string certificationLevel,
        bool isActive,
        DateTime createdAt,
        Guid createdBy,
        DateTime? updatedAt,
        Guid? updatedBy,
        bool isDeleted
        )
        {
            try
            {
                
                complianceOfficer.ComplianceOfficerId = complianceOfficerId;
                complianceOfficer.TenantId = tenantId;
                complianceOfficer.MerchantId = merchantId;
                complianceOfficer.OfficerCode = officerCode;
                complianceOfficer.FirstName = firstName;
                complianceOfficer.LastName = lastName;
                complianceOfficer.Email = email;
                complianceOfficer.Phone = phone;
                complianceOfficer.CertificationLevel = certificationLevel;
                complianceOfficer.IsActive = isActive;
                complianceOfficer.CreatedAt = createdAt;
                complianceOfficer.CreatedBy = createdBy;
                complianceOfficer.UpdatedAt = updatedAt;
                complianceOfficer.UpdatedBy = updatedBy;
                complianceOfficer.IsDeleted = isDeleted;
                complianceOfficer.RaiseDomainEvent(new ComplianceOfficerUpdatedEvent(complianceOfficer, "Updated"));
                
                return Success<string, ComplianceOfficer>(complianceOfficer);
            }
            catch (Exception ex)
            {
                return Validation<string, ComplianceOfficer>.Fail(new Seq<string> { ex.Message });
            }
        }
        public static class ComplianceOfficerCalculations
        {
            public static int CalculateDaysBetween(DateTime createdat, DateTime updatedat)
            {
                return (updatedat - createdat).Days;
            }
            
        }
    }
}

