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
    
    public class Assessment : BaseEntityEv<Guid>, IAggregateRoot, ITrackedEntity, ITenantEntity
    {
        [Key]
        public Guid AssessmentId { get; private set; }
        
        public Guid TenantId { get; private set; }
        
        public string AssessmentCode { get; private set; }
        
        public int AssessmentType { get; private set; }
        
        public string AssessmentPeriod { get; private set; }
        
        public DateTime StartDate { get; private set; }
        
        public DateTime EndDate { get; private set; }
        
        public DateTime? CompletionDate { get; private set; }
        
        public int Rank { get; private set; }
        
        public decimal? ComplianceScore { get; private set; }
        
        public bool QSAReviewRequired { get; private set; }
        
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
        public void SetAssessmentCode(string assessmentCode)
        {
            AssessmentCode = Guard.Against.NullOrEmpty(assessmentCode, nameof(assessmentCode));
        }
        public void SetAssessmentType(int assessmentType)
        {
            AssessmentType = Guard.Against.Negative(assessmentType, nameof(assessmentType));
        }
        public void SetAssessmentPeriod(string assessmentPeriod)
        {
            AssessmentPeriod = Guard.Against.NullOrEmpty(assessmentPeriod, nameof(assessmentPeriod));
        }
        public void SetStartDate(DateTime startDate)
        {
            StartDate = Guard.Against.OutOfSQLDateRange(startDate, nameof(startDate));
        }
        public void SetEndDate(DateTime endDate)
        {
            EndDate = Guard.Against.OutOfSQLDateRange(endDate, nameof(endDate));
        }
        public void SetCompletionDate(DateTime? completionDate)
        {
            CompletionDate = completionDate;
        }
        public void SetRank(int rank)
        {
            Rank = Guard.Against.Negative(rank, nameof(rank));
        }
        public void SetComplianceScore(decimal? complianceScore)
        {
            ComplianceScore = complianceScore;
        }
        public void SetQSAReviewRequired(bool qsareviewRequired)
        {
            QSAReviewRequired = Guard.Against.Null(qsareviewRequired, nameof(qsareviewRequired));
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
        
        public void UpdateMerchantForAssessment(Guid newMerchantId)
        {
            Guard.Against.Default(newMerchantId, nameof(newMerchantId));
            if (newMerchantId == MerchantId)
            {
                return;
            }
            
            MerchantId = newMerchantId;
            var assessmentUpdatedEvent = new AssessmentUpdatedEvent(this, "UpdatedEvent merchant");
            Events.Add(assessmentUpdatedEvent);
        }
        
        public virtual Merchant Merchant { get; private set; }
        
        public Guid MerchantId { get; private set; }
        
        private Assessment() { }
        
        public Assessment(Guid assessmentId, Guid merchantId, Guid tenantId, string assessmentCode, int assessmentType, string assessmentPeriod, DateTime startDate, DateTime endDate, int rank, bool qsareviewRequired, DateTime createdAt, Guid createdBy, bool isDeleted)
        {
            this.AssessmentId = Guard.Against.Default(assessmentId, nameof(assessmentId));
            this.MerchantId = Guard.Against.Default(merchantId, nameof(merchantId));
            this.TenantId = Guard.Against.Default(tenantId, nameof(tenantId));
            this.AssessmentCode = Guard.Against.NullOrEmpty(assessmentCode, nameof(assessmentCode));
            this.AssessmentType = Guard.Against.Negative(assessmentType, nameof(assessmentType));
            this.AssessmentPeriod = Guard.Against.NullOrEmpty(assessmentPeriod, nameof(assessmentPeriod));
            this.StartDate = Guard.Against.OutOfSQLDateRange(startDate, nameof(startDate));
            this.EndDate = Guard.Against.OutOfSQLDateRange(endDate, nameof(endDate));
            this.Rank = Guard.Against.Negative(rank, nameof(rank));
            this.QSAReviewRequired = Guard.Against.Null(qsareviewRequired, nameof(qsareviewRequired));
            this.CreatedAt = Guard.Against.OutOfSQLDateRange(createdAt, nameof(createdAt));
            this.CreatedBy = Guard.Against.Default(createdBy, nameof(createdBy));
            this.IsDeleted = Guard.Against.Null(isDeleted, nameof(isDeleted));
            
            _assessmentControls = new();
            
            _controlEvidences = new();
            
            _rocpackages = new();
            
        }
        private readonly List<AssessmentControl> _assessmentControls = new();
        public IEnumerable<AssessmentControl> AssessmentControls => _assessmentControls.AsReadOnly();
        
        private readonly List<ControlEvidence> _controlEvidences = new();
        public IEnumerable<ControlEvidence> ControlEvidences => _controlEvidences.AsReadOnly();
        
        private readonly List<ROCPackage> _rocpackages = new();
        public IEnumerable<ROCPackage> ROCPackages => _rocpackages.AsReadOnly();
        
        public override bool Equals(object? obj) =>
        obj is Assessment assessment && Equals(assessment);
        
        public bool Equals(Assessment other)
        {
            if (ReferenceEquals(null, other))
            {
                return false;
            }
            
            if (ReferenceEquals(this, other))
            {
                return true;
            }
            
            return AssessmentId.Equals(other.AssessmentId);
        }
        
        public override int GetHashCode() => AssessmentId.GetHashCode();
        
        public static bool operator !=(Assessment left, Assessment right) => !(left == right);
        
        public static bool operator ==(Assessment left, Assessment right) => left?.Equals(right) ?? right is null;
        
        private void ValidateInvariants()
        {
            if (string.IsNullOrWhiteSpace(AssessmentCode))
            throw new InvalidOperationException("AssessmentCode cannot be null or whitespace.");
            if (AssessmentCode?.Length > 30)
            throw new InvalidOperationException("AssessmentCode cannot exceed 30 characters.");
            if (string.IsNullOrWhiteSpace(AssessmentPeriod))
            throw new InvalidOperationException("AssessmentPeriod cannot be null or whitespace.");
            if (AssessmentPeriod?.Length > 20)
            throw new InvalidOperationException("AssessmentPeriod cannot exceed 20 characters.");
            if (StartDate == default)
            throw new InvalidOperationException("StartDate must be set.");
            if (EndDate == default)
            throw new InvalidOperationException("EndDate must be set.");
            if (CompletionDate == default)
            throw new InvalidOperationException("CompletionDate must be set.");
            if (UpdatedAt == default)
            throw new InvalidOperationException("UpdatedAt must be set.");
        }
        
        private void RaiseDomainEvent(BaseDomainEvent domainEvent)
        {
            Events.Add(domainEvent);
        }
        public static Eff<Validation<string, Assessment>> Create(
        Guid assessmentId,
        Guid merchantId,
        Guid tenantId,
        string assessmentCode,
        int assessmentType,
        string assessmentPeriod,
        DateTime startDate,
        DateTime endDate,
        int rank,
        bool qsareviewRequired,
        DateTime createdAt,
        Guid createdBy,
        bool isDeleted
        )
        {
            return Eff(() => CreateAssessment(
            assessmentId,
            merchantId,
            tenantId,
            assessmentCode,
            assessmentType,
            assessmentPeriod,
            startDate,
            endDate,
            rank,
            qsareviewRequired,
            createdAt,
            createdBy,
            isDeleted
            ));
        }
        
        private static Validation<string, Assessment> CreateAssessment(
        Guid assessmentId,
        Guid merchantId,
        Guid tenantId,
        string assessmentCode,
        int assessmentType,
        string assessmentPeriod,
        DateTime startDate,
        DateTime endDate,
        int rank,
        bool qsareviewRequired,
        DateTime createdAt,
        Guid createdBy,
        bool isDeleted
        )
        {
            try
            {
                var now = DateTime.UtcNow;
                isDeleted = false;
                
                var assessment = new Assessment
                ( assessmentId,
                merchantId,
                tenantId,
                assessmentCode,
                assessmentType,
                assessmentPeriod,
                startDate,
                endDate,
                rank,
                qsareviewRequired,
                createdAt,
                createdBy,
                isDeleted)
                ;
                
                return Validation<string, Assessment>.Success(assessment);
            }
            catch (Exception ex)
            {
                return Validation<string, Assessment>.Fail(new Seq<string> { ex.Message });
            }
        }
        public static Eff<Validation<string, Assessment>> Update(
        Assessment existingAssessment,
        Guid assessmentId,
        Guid tenantId,
        Guid merchantId,
        string assessmentCode,
        int assessmentType,
        string assessmentPeriod,
        DateTime startDate,
        DateTime endDate,
        DateTime? completionDate,
        int rank,
        decimal? complianceScore,
        bool qsareviewRequired,
        DateTime createdAt,
        Guid createdBy,
        DateTime? updatedAt,
        Guid? updatedBy,
        bool isDeleted
        )
        {
            return Eff(() => UpdateAssessment(
            existingAssessment,
            assessmentId,
            tenantId,
            merchantId,
            assessmentCode,
            assessmentType,
            assessmentPeriod,
            startDate,
            endDate,
            completionDate.ToOption().ToNullable(),
            rank,
            complianceScore,
            qsareviewRequired,
            createdAt,
            createdBy,
            updatedAt.ToOption().ToNullable(),
            updatedBy.ToOption().ToNullable(),
            isDeleted
            ));
        }
        
        private static Validation<string, Assessment> UpdateAssessment(
        Assessment assessment,
        Guid assessmentId,
        Guid tenantId,
        Guid merchantId,
        string assessmentCode,
        int assessmentType,
        string assessmentPeriod,
        DateTime startDate,
        DateTime endDate,
        DateTime? completionDate,
        int rank,
        decimal? complianceScore,
        bool qsareviewRequired,
        DateTime createdAt,
        Guid createdBy,
        DateTime? updatedAt,
        Guid? updatedBy,
        bool isDeleted
        )
        {
            try
            {
                
                assessment.AssessmentId = assessmentId;
                assessment.TenantId = tenantId;
                assessment.MerchantId = merchantId;
                assessment.AssessmentCode = assessmentCode;
                assessment.AssessmentType = assessmentType;
                assessment.AssessmentPeriod = assessmentPeriod;
                assessment.StartDate = startDate;
                assessment.EndDate = endDate;
                assessment.CompletionDate = completionDate;
                assessment.Rank = rank;
                assessment.ComplianceScore = complianceScore;
                assessment.QSAReviewRequired = qsareviewRequired;
                assessment.CreatedAt = createdAt;
                assessment.CreatedBy = createdBy;
                assessment.UpdatedAt = updatedAt;
                assessment.UpdatedBy = updatedBy;
                assessment.IsDeleted = isDeleted;
                assessment.RaiseDomainEvent(new AssessmentUpdatedEvent(assessment, "Updated"));
                
                return Success<string, Assessment>(assessment);
            }
            catch (Exception ex)
            {
                return Validation<string, Assessment>.Fail(new Seq<string> { ex.Message });
            }
        }
        
        public Eff<Option<string>> AddAssessmentControl(AssessmentControl assessmentControl)
        {
            return Eff(() =>
            {
                if (assessmentControl is null)
                return Option<string>.Some("AssessmentControl cannot be null.");
                if (assessmentControl.TestResult < 0)
                return Option<string>.Some("Test Result is out of valid range.");
                if (assessmentControl.CreatedAt == default)
                return Option<string>.Some("Created At must be set.");
                if (assessmentControl.CreatedAt < new DateTime(2024, 1, 1) || assessmentControl.CreatedAt > DateTime.UtcNow.AddYears(100))
                return Option<string>.Some("Created At is out of valid range.");
                _assessmentControls.Add(assessmentControl);
                RaiseDomainEvent(new AssessmentControlCreatedEvent(assessmentControl, "Created"));
                return Option<string>.None;
            });
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
        public Eff<Option<string>> AddROCPackage(ROCPackage rocpackage)
        {
            return Eff(() =>
            {
                if (rocpackage is null)
                return Option<string>.Some("ROCPackage cannot be null.");
                if (string.IsNullOrWhiteSpace(rocpackage.PackageVersion))
                return Option<string>.Some("Package Version cannot be empty.");
                if (rocpackage.PackageVersion.Length > 20)
                return Option<string>.Some("Package Version cannot exceed 20 characters.");
                if (rocpackage.GeneratedDate == default)
                return Option<string>.Some("Generated Date must be set.");
                if (rocpackage.GeneratedDate < new DateTime(2024, 1, 1) || rocpackage.GeneratedDate > DateTime.UtcNow.AddYears(100))
                return Option<string>.Some("Generated Date is out of valid range.");
                if (rocpackage.CreatedAt == default)
                return Option<string>.Some("Created At must be set.");
                if (rocpackage.CreatedAt < new DateTime(2024, 1, 1) || rocpackage.CreatedAt > DateTime.UtcNow.AddYears(100))
                return Option<string>.Some("Created At is out of valid range.");
                _rocpackages.Add(rocpackage);
                RaiseDomainEvent(new ROCPackageCreatedEvent(rocpackage, "Created"));
                return Option<string>.None;
            });
        }
        
        public static class AssessmentCalculations
        {
            public static decimal CalculateTotal(int assessmenttype, int rank, decimal compliancescore)
            {
                return assessmenttype + rank + compliancescore;
            }
            
            public static decimal CalculateAverage(int assessmenttype, int rank, decimal compliancescore)
            {
                return (assessmenttype + rank + compliancescore) / 3;
            }
            
            public static decimal CalculateAssessmentTypePerRank(int assessmenttype, int rank)
            {
                if (rank == 0)
                throw new DivideByZeroException("Cannot calculate rate: Rank is zero.");
                return (decimal)assessmenttype / rank;
            }
            
            public static decimal CalculateAssessmentTypePerComplianceScore(int assessmenttype, decimal compliancescore)
            {
                if (compliancescore == 0)
                throw new DivideByZeroException("Cannot calculate rate: ComplianceScore is zero.");
                return (decimal)assessmenttype / compliancescore;
            }
            
            public static decimal CalculateRankPerComplianceScore(int rank, decimal compliancescore)
            {
                if (compliancescore == 0)
                throw new DivideByZeroException("Cannot calculate rate: ComplianceScore is zero.");
                return (decimal)rank / compliancescore;
            }
            
            public static int CalculateDaysBetween(DateTime startdate, DateTime enddate)
            {
                return (enddate - startdate).Days;
            }
            
        }
    }
}

