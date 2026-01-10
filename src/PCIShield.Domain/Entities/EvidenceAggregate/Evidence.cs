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
    
    public class Evidence : BaseEntityEv<Guid>, IAggregateRoot, ITrackedEntity, ITenantEntity
    {
        [Key]
        public Guid EvidenceId { get; private set; }
        
        public Guid TenantId { get; private set; }
        
        public string EvidenceCode { get; private set; }
        
        public string EvidenceTitle { get; private set; }
        
        public int EvidenceType { get; private set; }
        
        public DateTime CollectedDate { get; private set; }
        
        public string? FileHash { get; private set; }
        
        public string? StorageUri { get; private set; }
        
        public bool IsValid { get; private set; }
        
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
        public void SetEvidenceCode(string evidenceCode)
        {
            EvidenceCode = Guard.Against.NullOrEmpty(evidenceCode, nameof(evidenceCode));
        }
        public void SetEvidenceTitle(string evidenceTitle)
        {
            EvidenceTitle = Guard.Against.NullOrEmpty(evidenceTitle, nameof(evidenceTitle));
        }
        public void SetEvidenceType(int evidenceType)
        {
            EvidenceType = Guard.Against.Negative(evidenceType, nameof(evidenceType));
        }
        public void SetCollectedDate(DateTime collectedDate)
        {
            CollectedDate = Guard.Against.OutOfSQLDateRange(collectedDate, nameof(collectedDate));
        }
        public void SetFileHash(string fileHash)
        {
            FileHash = fileHash;
        }
        public void SetStorageUri(string storageUri)
        {
            StorageUri = storageUri;
        }
        public void SetIsValid(bool isValid)
        {
            IsValid = Guard.Against.Null(isValid, nameof(isValid));
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
        
        public void UpdateMerchantForEvidence(Guid newMerchantId)
        {
            Guard.Against.Default(newMerchantId, nameof(newMerchantId));
            if (newMerchantId == MerchantId)
            {
                return;
            }
            
            MerchantId = newMerchantId;
            var evidenceUpdatedEvent = new EvidenceUpdatedEvent(this, "UpdatedEvent merchant");
            Events.Add(evidenceUpdatedEvent);
        }
        
        public virtual Merchant Merchant { get; private set; }
        
        public Guid MerchantId { get; private set; }
        
        private Evidence() { }
        
        public Evidence(Guid evidenceId, Guid merchantId, Guid tenantId, string evidenceCode, string evidenceTitle, int evidenceType, DateTime collectedDate, bool isValid, DateTime createdAt, Guid createdBy, bool isDeleted)
        {
            this.EvidenceId = Guard.Against.Default(evidenceId, nameof(evidenceId));
            this.MerchantId = Guard.Against.Default(merchantId, nameof(merchantId));
            this.TenantId = Guard.Against.Default(tenantId, nameof(tenantId));
            this.EvidenceCode = Guard.Against.NullOrEmpty(evidenceCode, nameof(evidenceCode));
            this.EvidenceTitle = Guard.Against.NullOrEmpty(evidenceTitle, nameof(evidenceTitle));
            this.EvidenceType = Guard.Against.Negative(evidenceType, nameof(evidenceType));
            this.CollectedDate = Guard.Against.OutOfSQLDateRange(collectedDate, nameof(collectedDate));
            this.IsValid = Guard.Against.Null(isValid, nameof(isValid));
            this.CreatedAt = Guard.Against.OutOfSQLDateRange(createdAt, nameof(createdAt));
            this.CreatedBy = Guard.Against.Default(createdBy, nameof(createdBy));
            this.IsDeleted = Guard.Against.Null(isDeleted, nameof(isDeleted));
            
            _controlEvidences = new();
            
        }
        private readonly List<ControlEvidence> _controlEvidences = new();
        public IEnumerable<ControlEvidence> ControlEvidences => _controlEvidences.AsReadOnly();
        
        public override bool Equals(object? obj) =>
        obj is Evidence evidence && Equals(evidence);
        
        public bool Equals(Evidence other)
        {
            if (ReferenceEquals(null, other))
            {
                return false;
            }
            
            if (ReferenceEquals(this, other))
            {
                return true;
            }
            
            return EvidenceId.Equals(other.EvidenceId);
        }
        
        public override int GetHashCode() => EvidenceId.GetHashCode();
        
        public static bool operator !=(Evidence left, Evidence right) => !(left == right);
        
        public static bool operator ==(Evidence left, Evidence right) => left?.Equals(right) ?? right is null;
        
        private void ValidateInvariants()
        {
            if (string.IsNullOrWhiteSpace(EvidenceCode))
            throw new InvalidOperationException("EvidenceCode cannot be null or whitespace.");
            if (EvidenceCode?.Length > 50)
            throw new InvalidOperationException("EvidenceCode cannot exceed 50 characters.");
            if (string.IsNullOrWhiteSpace(EvidenceTitle))
            throw new InvalidOperationException("EvidenceTitle cannot be null or whitespace.");
            if (EvidenceTitle?.Length > 200)
            throw new InvalidOperationException("EvidenceTitle cannot exceed 200 characters.");
            if (CollectedDate == default)
            throw new InvalidOperationException("CollectedDate must be set.");
            if (FileHash?.Length > 128)
            throw new InvalidOperationException("FileHash cannot exceed 128 characters.");
            if (StorageUri?.Length > 500)
            throw new InvalidOperationException("StorageUri cannot exceed 500 characters.");
            if (UpdatedAt == default)
            throw new InvalidOperationException("UpdatedAt must be set.");
        }
        
        private void RaiseDomainEvent(BaseDomainEvent domainEvent)
        {
            Events.Add(domainEvent);
        }
        public static Eff<Validation<string, Evidence>> Create(
        Guid evidenceId,
        Guid merchantId,
        Guid tenantId,
        string evidenceCode,
        string evidenceTitle,
        int evidenceType,
        DateTime collectedDate,
        bool isValid,
        DateTime createdAt,
        Guid createdBy,
        bool isDeleted
        )
        {
            return Eff(() => CreateEvidence(
            evidenceId,
            merchantId,
            tenantId,
            evidenceCode,
            evidenceTitle,
            evidenceType,
            collectedDate,
            isValid,
            createdAt,
            createdBy,
            isDeleted
            ));
        }
        
        private static Validation<string, Evidence> CreateEvidence(
        Guid evidenceId,
        Guid merchantId,
        Guid tenantId,
        string evidenceCode,
        string evidenceTitle,
        int evidenceType,
        DateTime collectedDate,
        bool isValid,
        DateTime createdAt,
        Guid createdBy,
        bool isDeleted
        )
        {
            try
            {
                var now = DateTime.UtcNow;
                isDeleted = false;
                
                var evidence = new Evidence
                ( evidenceId,
                merchantId,
                tenantId,
                evidenceCode,
                evidenceTitle,
                evidenceType,
                collectedDate,
                isValid,
                createdAt,
                createdBy,
                isDeleted)
                ;
                
                return Validation<string, Evidence>.Success(evidence);
            }
            catch (Exception ex)
            {
                return Validation<string, Evidence>.Fail(new Seq<string> { ex.Message });
            }
        }
        public static Eff<Validation<string, Evidence>> Update(
        Evidence existingEvidence,
        Guid evidenceId,
        Guid tenantId,
        Guid merchantId,
        string evidenceCode,
        string evidenceTitle,
        int evidenceType,
        DateTime collectedDate,
        string fileHash,
        string storageUri,
        bool isValid,
        DateTime createdAt,
        Guid createdBy,
        DateTime? updatedAt,
        Guid? updatedBy,
        bool isDeleted
        )
        {
            return Eff(() => UpdateEvidence(
            existingEvidence,
            evidenceId,
            tenantId,
            merchantId,
            evidenceCode,
            evidenceTitle,
            evidenceType,
            collectedDate,
            fileHash,
            storageUri,
            isValid,
            createdAt,
            createdBy,
            updatedAt.ToOption().ToNullable(),
            updatedBy.ToOption().ToNullable(),
            isDeleted
            ));
        }
        
        private static Validation<string, Evidence> UpdateEvidence(
        Evidence evidence,
        Guid evidenceId,
        Guid tenantId,
        Guid merchantId,
        string evidenceCode,
        string evidenceTitle,
        int evidenceType,
        DateTime collectedDate,
        string fileHash,
        string storageUri,
        bool isValid,
        DateTime createdAt,
        Guid createdBy,
        DateTime? updatedAt,
        Guid? updatedBy,
        bool isDeleted
        )
        {
            try
            {
                
                evidence.EvidenceId = evidenceId;
                evidence.TenantId = tenantId;
                evidence.MerchantId = merchantId;
                evidence.EvidenceCode = evidenceCode;
                evidence.EvidenceTitle = evidenceTitle;
                evidence.EvidenceType = evidenceType;
                evidence.CollectedDate = collectedDate;
                evidence.FileHash = fileHash;
                evidence.StorageUri = storageUri;
                evidence.IsValid = isValid;
                evidence.CreatedAt = createdAt;
                evidence.CreatedBy = createdBy;
                evidence.UpdatedAt = updatedAt;
                evidence.UpdatedBy = updatedBy;
                evidence.IsDeleted = isDeleted;
                evidence.RaiseDomainEvent(new EvidenceUpdatedEvent(evidence, "Updated"));
                
                return Success<string, Evidence>(evidence);
            }
            catch (Exception ex)
            {
                return Validation<string, Evidence>.Fail(new Seq<string> { ex.Message });
            }
        }
        
        public Eff<Option<string>> AddControlEvidence(ControlEvidence controlEvidence)
        {
            return Eff(() =>
            {
                if (controlEvidence is null)
                return Option<string>.Some("ControlEvidence cannot be null.");
                if (controlEvidence.CreatedAt == default)
                return Option<string>.Some("Created At must be set.");
                if (controlEvidence.CreatedAt < new DateTime(2024, 1, 1) || controlEvidence.CreatedAt > DateTime.UtcNow.AddYears(100))
                return Option<string>.Some("Created At is out of valid range.");
                _controlEvidences.Add(controlEvidence);
                RaiseDomainEvent(new ControlEvidenceCreatedEvent(controlEvidence, "Created"));
                return Option<string>.None;
            });
        }
        
        public static class EvidenceCalculations
        {
            public static decimal CalculateTotal(int evidencetype)
            {
                return evidencetype;
            }
            
            public static int CalculateDaysBetween(DateTime collecteddate, DateTime createdat)
            {
                return (createdat - collecteddate).Days;
            }
            
        }
    }
}

