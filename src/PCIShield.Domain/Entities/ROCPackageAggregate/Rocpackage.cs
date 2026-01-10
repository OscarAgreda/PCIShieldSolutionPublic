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
    
    public class ROCPackage : BaseEntityEv<Guid>, IAggregateRoot, ITrackedEntity, ITenantEntity
    {
        [Key]
        public Guid ROCPackageId { get; private set; }
        
        public Guid TenantId { get; private set; }
        
        public string PackageVersion { get; private set; }
        
        public DateTime GeneratedDate { get; private set; }
        
        public string? QSAName { get; private set; }
        
        public string? QSACompany { get; private set; }
        
        public DateTime? SignatureDate { get; private set; }
        
        public string? AOCNumber { get; private set; }
        
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
        public void SetAssessmentId(Guid assessmentId)
        {
            AssessmentId = Guard.Against.Default(assessmentId, nameof(assessmentId));
        }
        public void SetPackageVersion(string packageVersion)
        {
            PackageVersion = Guard.Against.NullOrEmpty(packageVersion, nameof(packageVersion));
        }
        public void SetGeneratedDate(DateTime generatedDate)
        {
            GeneratedDate = Guard.Against.OutOfSQLDateRange(generatedDate, nameof(generatedDate));
        }
        public void SetQSAName(string qsaname)
        {
            QSAName = qsaname;
        }
        public void SetQSACompany(string qsacompany)
        {
            QSACompany = qsacompany;
        }
        public void SetSignatureDate(DateTime? signatureDate)
        {
            SignatureDate = signatureDate;
        }
        public void SetAOCNumber(string aocnumber)
        {
            AOCNumber = aocnumber;
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
        
        public void UpdateAssessmentForROCPackage(Guid newAssessmentId)
        {
            Guard.Against.Default(newAssessmentId, nameof(newAssessmentId));
            if (newAssessmentId == AssessmentId)
            {
                return;
            }
            
            AssessmentId = newAssessmentId;
            var rocpackageUpdatedEvent = new ROCPackageUpdatedEvent(this, "UpdatedEvent assessment");
            Events.Add(rocpackageUpdatedEvent);
        }
        
        public virtual Assessment Assessment { get; private set; }
        
        public Guid AssessmentId { get; private set; }
        
        private ROCPackage() { }
        
        public ROCPackage(Guid rocpackageId, Guid assessmentId, Guid tenantId, string packageVersion, DateTime generatedDate, int rank, DateTime createdAt, Guid createdBy, bool isDeleted)
        {
            this.ROCPackageId = Guard.Against.Default(rocpackageId, nameof(rocpackageId));
            this.AssessmentId = Guard.Against.Default(assessmentId, nameof(assessmentId));
            this.TenantId = Guard.Against.Default(tenantId, nameof(tenantId));
            this.PackageVersion = Guard.Against.NullOrEmpty(packageVersion, nameof(packageVersion));
            this.GeneratedDate = Guard.Against.OutOfSQLDateRange(generatedDate, nameof(generatedDate));
            this.Rank = Guard.Against.Negative(rank, nameof(rank));
            this.CreatedAt = Guard.Against.OutOfSQLDateRange(createdAt, nameof(createdAt));
            this.CreatedBy = Guard.Against.Default(createdBy, nameof(createdBy));
            this.IsDeleted = Guard.Against.Null(isDeleted, nameof(isDeleted));
            
        }
        public override bool Equals(object? obj) =>
        obj is ROCPackage rocpackage && Equals(rocpackage);
        
        public bool Equals(ROCPackage other)
        {
            if (ReferenceEquals(null, other))
            {
                return false;
            }
            
            if (ReferenceEquals(this, other))
            {
                return true;
            }
            
            return ROCPackageId.Equals(other.ROCPackageId);
        }
        
        public override int GetHashCode() => ROCPackageId.GetHashCode();
        
        public static bool operator !=(ROCPackage left, ROCPackage right) => !(left == right);
        
        public static bool operator ==(ROCPackage left, ROCPackage right) => left?.Equals(right) ?? right is null;
        
        private void ValidateInvariants()
        {
            if (string.IsNullOrWhiteSpace(PackageVersion))
            throw new InvalidOperationException("PackageVersion cannot be null or whitespace.");
            if (PackageVersion?.Length > 20)
            throw new InvalidOperationException("PackageVersion cannot exceed 20 characters.");
            if (GeneratedDate == default)
            throw new InvalidOperationException("GeneratedDate must be set.");
            if (QSAName?.Length > 200)
            throw new InvalidOperationException("QSAName cannot exceed 200 characters.");
            if (QSACompany?.Length > 200)
            throw new InvalidOperationException("QSACompany cannot exceed 200 characters.");
            if (SignatureDate == default)
            throw new InvalidOperationException("SignatureDate must be set.");
            if (AOCNumber?.Length > 50)
            throw new InvalidOperationException("AOCNumber cannot exceed 50 characters.");
            if (UpdatedAt == default)
            throw new InvalidOperationException("UpdatedAt must be set.");
        }
        
        private void RaiseDomainEvent(BaseDomainEvent domainEvent)
        {
            Events.Add(domainEvent);
        }
        public static Eff<Validation<string, ROCPackage>> Create(
        Guid rocpackageId,
        Guid assessmentId,
        Guid tenantId,
        string packageVersion,
        DateTime generatedDate,
        int rank,
        DateTime createdAt,
        Guid createdBy,
        bool isDeleted
        )
        {
            return Eff(() => CreateROCPackage(
            rocpackageId,
            assessmentId,
            tenantId,
            packageVersion,
            generatedDate,
            rank,
            createdAt,
            createdBy,
            isDeleted
            ));
        }
        
        private static Validation<string, ROCPackage> CreateROCPackage(
        Guid rocpackageId,
        Guid assessmentId,
        Guid tenantId,
        string packageVersion,
        DateTime generatedDate,
        int rank,
        DateTime createdAt,
        Guid createdBy,
        bool isDeleted
        )
        {
            try
            {
                var now = DateTime.UtcNow;
                isDeleted = false;
                
                var rocpackage = new ROCPackage
                ( rocpackageId,
                assessmentId,
                tenantId,
                packageVersion,
                generatedDate,
                rank,
                createdAt,
                createdBy,
                isDeleted)
                ;
                
                return Validation<string, ROCPackage>.Success(rocpackage);
            }
            catch (Exception ex)
            {
                return Validation<string, ROCPackage>.Fail(new Seq<string> { ex.Message });
            }
        }
        public static Eff<Validation<string, ROCPackage>> Update(
        ROCPackage existingROCPackage,
        Guid rocpackageId,
        Guid tenantId,
        Guid assessmentId,
        string packageVersion,
        DateTime generatedDate,
        string qsaname,
        string qsacompany,
        DateTime? signatureDate,
        string aocnumber,
        int rank,
        DateTime createdAt,
        Guid createdBy,
        DateTime? updatedAt,
        Guid? updatedBy,
        bool isDeleted
        )
        {
            return Eff(() => UpdateROCPackage(
            existingROCPackage,
            rocpackageId,
            tenantId,
            assessmentId,
            packageVersion,
            generatedDate,
            qsaname,
            qsacompany,
            signatureDate.ToOption().ToNullable(),
            aocnumber,
            rank,
            createdAt,
            createdBy,
            updatedAt.ToOption().ToNullable(),
            updatedBy.ToOption().ToNullable(),
            isDeleted
            ));
        }
        
        private static Validation<string, ROCPackage> UpdateROCPackage(
        ROCPackage rocpackage,
        Guid rocpackageId,
        Guid tenantId,
        Guid assessmentId,
        string packageVersion,
        DateTime generatedDate,
        string qsaname,
        string qsacompany,
        DateTime? signatureDate,
        string aocnumber,
        int rank,
        DateTime createdAt,
        Guid createdBy,
        DateTime? updatedAt,
        Guid? updatedBy,
        bool isDeleted
        )
        {
            try
            {
                
                rocpackage.ROCPackageId = rocpackageId;
                rocpackage.TenantId = tenantId;
                rocpackage.AssessmentId = assessmentId;
                rocpackage.PackageVersion = packageVersion;
                rocpackage.GeneratedDate = generatedDate;
                rocpackage.QSAName = qsaname;
                rocpackage.QSACompany = qsacompany;
                rocpackage.SignatureDate = signatureDate;
                rocpackage.AOCNumber = aocnumber;
                rocpackage.Rank = rank;
                rocpackage.CreatedAt = createdAt;
                rocpackage.CreatedBy = createdBy;
                rocpackage.UpdatedAt = updatedAt;
                rocpackage.UpdatedBy = updatedBy;
                rocpackage.IsDeleted = isDeleted;
                rocpackage.RaiseDomainEvent(new ROCPackageUpdatedEvent(rocpackage, "Updated"));
                
                return Success<string, ROCPackage>(rocpackage);
            }
            catch (Exception ex)
            {
                return Validation<string, ROCPackage>.Fail(new Seq<string> { ex.Message });
            }
        }
        public static class ROCPackageCalculations
        {
            public static decimal CalculateTotal(int rank)
            {
                return rank;
            }
            
            public static int CalculateDaysBetween(DateTime generateddate, DateTime signaturedate)
            {
                return (signaturedate - generateddate).Days;
            }
            
        }
    }
}

