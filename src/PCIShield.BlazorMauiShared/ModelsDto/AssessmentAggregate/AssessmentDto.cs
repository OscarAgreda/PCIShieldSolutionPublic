using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Reactive;
using System.Reactive.Linq;
using Ardalis.GuardClauses;
using System.Text.Json.Serialization;
using PCIShield.BlazorMauiShared;
using PCIShieldLib.SharedKernel;
using PCIShieldLib.SharedKernel.Interfaces;
using LanguageExt;
using LanguageExt.Common;
using ReactiveUI;
using static LanguageExt.Prelude;
using System.Reactive.Subjects;
using System.Text.Json;

using Unit = LanguageExt.Unit;
using System.ComponentModel.DataAnnotations.Schema;
using System.Collections.Concurrent;
namespace PCIShield.Domain.ModelsDto
{
    public static class AssessmentFactory
    {
        public static AssessmentDto CreateNewFromTemplate(AssessmentDto template)
        {
            var now = DateTime.UtcNow;

            var newAssessment = new AssessmentDto
            {
                AssessmentId = Guid.Empty,
                CreatedBy = template.CreatedBy,
                TenantId =  Guid.Empty,
                ComplianceScore = 0,
                QSAReviewRequired = false,
                IsDeleted = false,
                AssessmentControls = new(),
                ControlEvidences = new(),
                ROCPackages = new(),
            };

            return newAssessment;
        }

        public static AssessmentDto CreateNewEmpty()
        {
            var now = DateTime.UtcNow;
            return new AssessmentDto
            {
                AssessmentId = Guid.Empty,
                TenantId = Guid.Empty,
                MerchantId = Guid.Empty,
                CreatedBy = Guid.Empty,
                UpdatedBy = Guid.Empty,
                IsDeleted = false,
            };
        }
    }
    public static class AssessmentExtensions
    {
        public static AssessmentDto CloneAsNew(this AssessmentDto template)
        {
            return AssessmentFactory.CreateNewFromTemplate(template);
        }
    }

    public sealed class AssessmentDto : ErrorIdentifiableDtoBase, IModelDto,ITrackableEntity<AssessmentDto>,ISnapshotable<AssessmentDto>,IChangeObservable
    {
        public Guid  AssessmentId { get;  set; }

        public Guid TenantId { get;  set; }

        public string AssessmentCode { get;  set; }

        public int AssessmentType { get;  set; }

        public string AssessmentPeriod { get;  set; }

        public DateTime StartDate { get;  set; }

        public DateTime EndDate { get;  set; }

        public DateTime? CompletionDate { get;  set; }

        public int Rank { get;  set; }

        public decimal? ComplianceScore { get;  set; }

        public bool QSAReviewRequired { get;  set; }

        public DateTime CreatedAt { get;  set; }

        public Guid CreatedBy { get;  set; }

        public DateTime? UpdatedAt { get;  set; }

        public Guid? UpdatedBy { get;  set; }

        public bool IsDeleted { get;  set; }
        public int ActiveAssessmentControlCount { get; set; }
        public int ActiveControlEvidenceCount { get; set; }
        public int ActiveROCPackageCount { get; set; }
        public int AverageControlFrequencyDays { get; set; }
        public int ControlFrequencyDays { get; set; }
        public int CriticalROCPackageCount { get; set; }
        public int DaysSinceStartDate { get; set; }
        public bool HasControlFrequencyDaysOverdue { get; set; }
        public bool IsAssessmentTypeCritical { get; set; }
        public bool IsDeletedFlag { get; set; }
        public bool IsRankCritical { get; set; }
        public DateTime? LatestAssessmentControlTestDate { get; set; }
        public DateTime? LatestControlEvidenceUpdatedAt { get; set; }
        public DateTime? LatestROCPackageGeneratedDate { get; set; }
        public int TotalAssessmentControlCount { get; set; }
        public int TotalControlEvidenceCount { get; set; }
        public int TotalROCPackageCount { get; set; }
        public Guid ScanScheduleId { get; set; }
        public ScanScheduleDto? ScanSchedule { get; set; }
        public Guid VulnerabilityId { get; set; }
        public VulnerabilityDto? Vulnerability { get; set; }
        public Guid PaymentPageId { get; set; }
        public PaymentPageDto? PaymentPage { get; set; }
        public Guid ScriptId { get; set; }
        public ScriptDto? Script { get; set; }

        public MerchantDto Merchant { get;  set; }

        public Guid MerchantId { get;  set; }
        public AssessmentDto Clone()
        {
            return new AssessmentDto
            {
                AssessmentId = this.AssessmentId,
                TenantId = this.TenantId,
                AssessmentCode = this.AssessmentCode,
                AssessmentType = this.AssessmentType,
                AssessmentPeriod = this.AssessmentPeriod,
                StartDate = this.StartDate,
                EndDate = this.EndDate,
                CompletionDate = this.CompletionDate,
                Rank = this.Rank,
                ComplianceScore = this.ComplianceScore,
                QSAReviewRequired = this.QSAReviewRequired,
                CreatedAt = this.CreatedAt,
                CreatedBy = this.CreatedBy,
                UpdatedAt = this.UpdatedAt,
                UpdatedBy = this.UpdatedBy,
                IsDeleted = this.IsDeleted,
                MerchantId = this.MerchantId ,
                 Merchant = (this.Merchant!= null ? new  MerchantDto
                {
                    MerchantId = this.MerchantId,
                    TenantId = this.Merchant.TenantId,
                    MerchantCode = this.Merchant.MerchantCode,
                    MerchantName = this.Merchant.MerchantName,
                    MerchantLevel = this.Merchant.MerchantLevel,
                    AcquirerName = this.Merchant.AcquirerName,
                    ProcessorMID = this.Merchant.ProcessorMID,
                    AnnualCardVolume = this.Merchant.AnnualCardVolume,
                    LastAssessmentDate = this.Merchant.LastAssessmentDate,
                    NextAssessmentDue = this.Merchant.NextAssessmentDue,
                    ComplianceRank = this.Merchant.ComplianceRank,
                    CreatedAt = this.Merchant.CreatedAt,
                    CreatedBy = this.Merchant.CreatedBy,
                    UpdatedAt = this.Merchant.UpdatedAt,
                    UpdatedBy = this.Merchant.UpdatedBy,
                    IsDeleted = this.Merchant.IsDeleted,
                }:null) ?? new MerchantDto(),
            };
        }
        public List<AssessmentControlDto> AssessmentControls { get; set; } = new();
        public List<ControlEvidenceDto> ControlEvidences { get; set; } = new();
        public List<ROCPackageDto> ROCPackages { get; set; } = new();
        public AssessmentDto() { }
        public AssessmentDto(Guid assessmentId, Guid merchantId, Guid tenantId, string assessmentCode, int assessmentType, string assessmentPeriod, DateTime startDate, DateTime endDate, int rank, bool qsareviewRequired, DateTime createdAt, Guid createdBy, bool isDeleted)
        {
                AuditEntityId = assessmentId.ToString();
                AuditId = Guid.CreateVersion7().ToString();
                AuditEntityType = GetType().Name;
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

         this.AssessmentControls = new();

         this.ControlEvidences = new();

         this.ROCPackages = new();
        }
        [JsonIgnore]
        private Option<AssessmentDto> _snapshot = None;

        public Option<AssessmentDto> CurrentSnapshot => _snapshot;

        public void TakeSnapshot()
        {
            _snapshot = Some(Clone());
        }

        [NotMapped]
        [JsonIgnore]
        public bool IsDirty =>
            _snapshot.Match(
                Some: snap =>
                    !Compare(snap).Match(
                        Right: equal => equal,
                        Left: _ => false
                    ),
                None: () => true
            );

        public Either<Error, Unit> RestoreSnapshot() =>
            _snapshot.Match(
                Some: s =>
                {
                    CopyFrom(s);
                    AuditDataBeforeChanged = JsonSerializer.Serialize(this);
                    AuditDataAfterChanged = JsonSerializer.Serialize(s);
                    AuditTimeStamp = DateTime.UtcNow;
                    AuditAction = "Restored";
                    NotifyChange();
                    return Right<Error, Unit>(LanguageExt.Unit.Default);
                },
                None: () => Left<Error, Unit>(Error.New("No snapshot exists to restore"))
            );

        private void CopyFrom(AssessmentDto source)
        {
        AssessmentId = source.AssessmentId;
        TenantId = source.TenantId;
        AssessmentCode = source.AssessmentCode;
        AssessmentType = source.AssessmentType;
        AssessmentPeriod = source.AssessmentPeriod;
        StartDate = source.StartDate;
        EndDate = source.EndDate;
        CompletionDate = source.CompletionDate;
        Rank = source.Rank;
        ComplianceScore = source.ComplianceScore;
        QSAReviewRequired = source.QSAReviewRequired;
        CreatedAt = source.CreatedAt;
        CreatedBy = source.CreatedBy;
        UpdatedAt = source.UpdatedAt;
        UpdatedBy = source.UpdatedBy;
        IsDeleted = source.IsDeleted;
        MerchantId = source.MerchantId ;
            Merchant = source.Merchant?.Clone();
            AssessmentControls = new List<AssessmentControlDto>(source.AssessmentControls);
            ControlEvidences = new List<ControlEvidenceDto>(source.ControlEvidences);
            ROCPackages = new List<ROCPackageDto>(source.ROCPackages);
        }
        [JsonIgnore]
        private readonly BehaviorSubject<DtoState> _stateSubject
            = new(new DtoState(false, false, null));

        [JsonIgnore]
        private readonly Subject<Unit> _changes = new();

        [JsonIgnore]
        public IObservable<Unit> Changes => _changes.AsObservable();

        [JsonIgnore]
        public IObservable<DtoState> StateChanges => _stateSubject.AsObservable();

        public void NotifyChange()
        {
            _changes.OnNext(LanguageExt.Unit.Default);
            UpdateState();
        }

        private void UpdateState()
        {
            GetState().Match(
                Right: newState => _stateSubject.OnNext(newState),
                Left: err => _stateSubject.OnError(new Exception(err.Message))
            );
        }
        public Either<Error, bool> Compare(AssessmentDto other)
        {
            return CompareBasicFields(other).Bind(basicSame =>
            {
                if (!basicSame)
                {
                    return Right<Error, bool>(false);
                }
                else
                {
                    return Right<Error, bool>(true);
                }
            });
        }

        private Either<Error, bool> CompareBasicFields(AssessmentDto other)
        {
            bool same =
                (AssessmentId == other.AssessmentId) &&
                (TenantId == other.TenantId) &&
                (MerchantId == other.MerchantId) &&
                StringComparer.OrdinalIgnoreCase.Equals(AssessmentCode, other.AssessmentCode) &&
                (AssessmentType == other.AssessmentType) &&
                StringComparer.OrdinalIgnoreCase.Equals(AssessmentPeriod, other.AssessmentPeriod) &&
                (StartDate == other.StartDate) &&
                (EndDate == other.EndDate) &&
                (CompletionDate == other.CompletionDate) &&
                (Rank == other.Rank) &&
                (ComplianceScore == other.ComplianceScore) &&
                (QSAReviewRequired == other.QSAReviewRequired) &&
                (CreatedAt == other.CreatedAt) &&
                (CreatedBy == other.CreatedBy) &&
                (UpdatedAt == other.UpdatedAt) &&
                (UpdatedBy == other.UpdatedBy) &&
                (IsDeleted == other.IsDeleted) ;
            return Right<Error, bool>(same);
        }
        private bool CompareList<T>(List<T> current, List<T> other)
        {
            if (ReferenceEquals(current, other))
            {
                return true;
            }
            if (current.Count != other.Count)
            {
                return false;
            }
            for (int i = 0; i < current.Count; i++)
            {
                if (!Equals(current[i], other[i]))
                {
                    return false;
                }
            }
            return true;
        }

        public Either<Error, DtoState> GetState()
        {
            var state = new DtoState(
                IsDirty: IsDirty,
                HasChanges: _changes.HasObservers,
                LastModified: DateTime.UtcNow
            );
            return Right<Error, DtoState>(state);
        }
    }

    }