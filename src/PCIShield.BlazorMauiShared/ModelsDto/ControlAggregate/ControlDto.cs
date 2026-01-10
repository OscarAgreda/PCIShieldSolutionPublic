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
    public static class ControlFactory
    {
        public static ControlDto CreateNewFromTemplate(ControlDto template)
        {
            var now = DateTime.UtcNow;

            var newControl = new ControlDto
            {
                ControlId = Guid.Empty,
                CreatedBy = template.CreatedBy,
                TenantId =  Guid.Empty,
                IsMandatory = false,
                IsDeleted = false,
                AssessmentControls = new(),
                AssetControls = new(),
                CompensatingControls = new(),
                ControlEvidences = new(),
            };

            return newControl;
        }

        public static ControlDto CreateNewEmpty()
        {
            var now = DateTime.UtcNow;
            return new ControlDto
            {
                ControlId = Guid.Empty,
                TenantId = Guid.Empty,
                CreatedBy = Guid.Empty,
                UpdatedBy = Guid.Empty,
                IsDeleted = false,
            };
        }
    }
    public static class ControlExtensions
    {
        public static ControlDto CloneAsNew(this ControlDto template)
        {
            return ControlFactory.CreateNewFromTemplate(template);
        }
    }

    public sealed class ControlDto : ErrorIdentifiableDtoBase, IModelDto,ITrackableEntity<ControlDto>,ISnapshotable<ControlDto>,IChangeObservable
    {
        public Guid  ControlId { get;  set; }

        public Guid TenantId { get;  set; }

        public string ControlCode { get;  set; }

        public string RequirementNumber { get;  set; }

        public string ControlTitle { get;  set; }

        public string ControlDescription { get;  set; }

        public string? TestingGuidance { get;  set; }

        public int FrequencyDays { get;  set; }

        public bool IsMandatory { get;  set; }

        public DateTime EffectiveDate { get;  set; }

        public DateTime CreatedAt { get;  set; }

        public Guid CreatedBy { get;  set; }

        public DateTime? UpdatedAt { get;  set; }

        public Guid? UpdatedBy { get;  set; }

        public bool IsDeleted { get;  set; }
        public int ActiveAssessmentControlCount { get; set; }
        public int ActiveAssetControlCount { get; set; }
        public int ActiveCompensatingControlCount { get; set; }
        public int ActiveControlEvidenceCount { get; set; }
        public decimal AssessmentComplianceScore { get; set; }
        public decimal AverageAssessmentComplianceScore { get; set; }
        public decimal AverageMerchantAnnualCardVolume { get; set; }
        public int CriticalCompensatingControlCount { get; set; }
        public int DaysSinceEffectiveDate { get; set; }
        public bool HasAssessmentComplianceScoreBelowThreshold { get; set; }
        public bool IsMandatoryFlag { get; set; }
        public DateTime? LatestAssessmentControlTestDate { get; set; }
        public DateTime? LatestAssetControlUpdatedAt { get; set; }
        public DateTime? LatestCompensatingControlApprovalDate { get; set; }
        public DateTime? LatestControlEvidenceUpdatedAt { get; set; }
        public List<MerchantDto> Merchants { get; set; }
        public int TotalAssessmentControlCount { get; set; }
        public int TotalAssetControlCount { get; set; }
        public int TotalCompensatingControlCount { get; set; }
        public int TotalControlEvidenceCount { get; set; }

        public Guid MerchantId { get; set; }
        public MerchantDto? Merchant { get; set; }
        public ControlDto Clone()
        {
            return new ControlDto
            {
                ControlId = this.ControlId,
                TenantId = this.TenantId,
                ControlCode = this.ControlCode,
                RequirementNumber = this.RequirementNumber,
                ControlTitle = this.ControlTitle,
                ControlDescription = this.ControlDescription,
                TestingGuidance = this.TestingGuidance,
                FrequencyDays = this.FrequencyDays,
                IsMandatory = this.IsMandatory,
                EffectiveDate = this.EffectiveDate,
                CreatedAt = this.CreatedAt,
                CreatedBy = this.CreatedBy,
                UpdatedAt = this.UpdatedAt,
                UpdatedBy = this.UpdatedBy,
                IsDeleted = this.IsDeleted,
            };
        }
        public List<AssessmentControlDto> AssessmentControls { get; set; } = new();
        public List<AssetControlDto> AssetControls { get; set; } = new();
        public List<CompensatingControlDto> CompensatingControls { get; set; } = new();
        public List<ControlEvidenceDto> ControlEvidences { get; set; } = new();
        public ControlDto() { }
        public ControlDto(Guid controlId, Guid tenantId, string controlCode, string requirementNumber, string controlTitle, string controlDescription, int frequencyDays, bool isMandatory, DateTime effectiveDate, DateTime createdAt, Guid createdBy, bool isDeleted)
        {
                AuditEntityId = controlId.ToString();
                AuditId = Guid.CreateVersion7().ToString();
                AuditEntityType = GetType().Name;
                this.ControlId = Guard.Against.Default(controlId, nameof(controlId));
                this.TenantId = Guard.Against.Default(tenantId, nameof(tenantId));
                this.ControlCode = Guard.Against.NullOrEmpty(controlCode, nameof(controlCode));
                this.RequirementNumber = Guard.Against.NullOrEmpty(requirementNumber, nameof(requirementNumber));
                this.ControlTitle = Guard.Against.NullOrEmpty(controlTitle, nameof(controlTitle));
                this.ControlDescription = Guard.Against.NullOrEmpty(controlDescription, nameof(controlDescription));
                this.FrequencyDays = Guard.Against.Negative(frequencyDays, nameof(frequencyDays));
                this.IsMandatory = Guard.Against.Null(isMandatory, nameof(isMandatory));
                this.EffectiveDate = Guard.Against.OutOfSQLDateRange(effectiveDate, nameof(effectiveDate));
                this.CreatedAt = Guard.Against.OutOfSQLDateRange(createdAt, nameof(createdAt));
                this.CreatedBy = Guard.Against.Default(createdBy, nameof(createdBy));
                this.IsDeleted = Guard.Against.Null(isDeleted, nameof(isDeleted));

         this.AssessmentControls = new();

         this.AssetControls = new();

         this.CompensatingControls = new();

         this.ControlEvidences = new();
        }
        [JsonIgnore]
        private Option<ControlDto> _snapshot = None;

        public Option<ControlDto> CurrentSnapshot => _snapshot;

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

        private void CopyFrom(ControlDto source)
        {
        ControlId = source.ControlId;
        TenantId = source.TenantId;
        ControlCode = source.ControlCode;
        RequirementNumber = source.RequirementNumber;
        ControlTitle = source.ControlTitle;
        ControlDescription = source.ControlDescription;
        TestingGuidance = source.TestingGuidance;
        FrequencyDays = source.FrequencyDays;
        IsMandatory = source.IsMandatory;
        EffectiveDate = source.EffectiveDate;
        CreatedAt = source.CreatedAt;
        CreatedBy = source.CreatedBy;
        UpdatedAt = source.UpdatedAt;
        UpdatedBy = source.UpdatedBy;
        IsDeleted = source.IsDeleted;
            AssessmentControls = new List<AssessmentControlDto>(source.AssessmentControls);
            AssetControls = new List<AssetControlDto>(source.AssetControls);
            CompensatingControls = new List<CompensatingControlDto>(source.CompensatingControls);
            ControlEvidences = new List<ControlEvidenceDto>(source.ControlEvidences);
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
        public Either<Error, bool> Compare(ControlDto other)
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

        private Either<Error, bool> CompareBasicFields(ControlDto other)
        {
            bool same =
                (ControlId == other.ControlId) &&
                (TenantId == other.TenantId) &&
                StringComparer.OrdinalIgnoreCase.Equals(ControlCode, other.ControlCode) &&
                StringComparer.OrdinalIgnoreCase.Equals(RequirementNumber, other.RequirementNumber) &&
                StringComparer.OrdinalIgnoreCase.Equals(ControlTitle, other.ControlTitle) &&
                StringComparer.OrdinalIgnoreCase.Equals(ControlDescription, other.ControlDescription) &&
                StringComparer.OrdinalIgnoreCase.Equals(TestingGuidance, other.TestingGuidance) &&
                (FrequencyDays == other.FrequencyDays) &&
                (IsMandatory == other.IsMandatory) &&
                (EffectiveDate == other.EffectiveDate) &&
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