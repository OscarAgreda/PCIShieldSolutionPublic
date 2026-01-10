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
    public static class ControlEvidenceFactory
    {
        public static ControlEvidenceDto CreateNewFromTemplate(ControlEvidenceDto template)
        {
            var now = DateTime.UtcNow;

            var newControlEvidence = new ControlEvidenceDto
            {
                CreatedBy = template.CreatedBy,
                TenantId =  Guid.Empty,
                IsPrimary = false,
                IsDeleted = false,
            };

            return newControlEvidence;
        }

        public static ControlEvidenceDto CreateNewEmpty()
        {
            var now = DateTime.UtcNow;
            return new ControlEvidenceDto
            {
                ControlId = Guid.Empty,
                EvidenceId = Guid.Empty,
                AssessmentId = Guid.Empty,
                TenantId = Guid.Empty,
                CreatedBy = Guid.Empty,
                UpdatedBy = Guid.Empty,
                IsDeleted = false,
                IsPrimary = false,
            };
        }
    }
    public static class ControlEvidenceExtensions
    {
        public static ControlEvidenceDto CloneAsNew(this ControlEvidenceDto template)
        {
            return ControlEvidenceFactory.CreateNewFromTemplate(template);
        }
    }

    public sealed class ControlEvidenceDto : ErrorIdentifiableDtoBase, IModelDto,ITrackableEntity<ControlEvidenceDto>,ISnapshotable<ControlEvidenceDto>,IChangeObservable
    {
        public int  RowId { get;  set; }

        public Guid TenantId { get;  set; }

        public bool IsPrimary { get;  set; }

        public DateTime CreatedAt { get;  set; }

        public Guid CreatedBy { get;  set; }

        public DateTime? UpdatedAt { get;  set; }

        public Guid? UpdatedBy { get;  set; }

        public bool IsDeleted { get;  set; }

        public AssessmentDto Assessment { get;  set; }

        public Guid AssessmentId { get;  set; }

        public ControlDto Control { get;  set; }

        public Guid ControlId { get;  set; }

        public EvidenceDto Evidence { get;  set; }

        public Guid EvidenceId { get;  set; }
        public ControlEvidenceDto Clone()
        {
            return new ControlEvidenceDto
            {
                TenantId = this.TenantId,
                IsPrimary = this.IsPrimary,
                CreatedAt = this.CreatedAt,
                CreatedBy = this.CreatedBy,
                UpdatedAt = this.UpdatedAt,
                UpdatedBy = this.UpdatedBy,
                IsDeleted = this.IsDeleted,
                AssessmentId = this.AssessmentId ,
                 Assessment = (this.Assessment!= null ? new  AssessmentDto
                {
                    AssessmentId = this.AssessmentId,
                    TenantId = this.Assessment.TenantId,
                    AssessmentCode = this.Assessment.AssessmentCode,
                    AssessmentType = this.Assessment.AssessmentType,
                    AssessmentPeriod = this.Assessment.AssessmentPeriod,
                    StartDate = this.Assessment.StartDate,
                    EndDate = this.Assessment.EndDate,
                    CompletionDate = this.Assessment.CompletionDate,
                    Rank = this.Assessment.Rank,
                    ComplianceScore = this.Assessment.ComplianceScore,
                    QSAReviewRequired = this.Assessment.QSAReviewRequired,
                    CreatedAt = this.Assessment.CreatedAt,
                    CreatedBy = this.Assessment.CreatedBy,
                    UpdatedAt = this.Assessment.UpdatedAt,
                    UpdatedBy = this.Assessment.UpdatedBy,
                    IsDeleted = this.Assessment.IsDeleted,
                }:null) ?? new AssessmentDto(),
                ControlId = this.ControlId ,
                 Control = (this.Control!= null ? new  ControlDto
                {
                    ControlId = this.ControlId,
                    TenantId = this.Control.TenantId,
                    ControlCode = this.Control.ControlCode,
                    RequirementNumber = this.Control.RequirementNumber,
                    ControlTitle = this.Control.ControlTitle,
                    ControlDescription = this.Control.ControlDescription,
                    TestingGuidance = this.Control.TestingGuidance,
                    FrequencyDays = this.Control.FrequencyDays,
                    IsMandatory = this.Control.IsMandatory,
                    EffectiveDate = this.Control.EffectiveDate,
                    CreatedAt = this.Control.CreatedAt,
                    CreatedBy = this.Control.CreatedBy,
                    UpdatedAt = this.Control.UpdatedAt,
                    UpdatedBy = this.Control.UpdatedBy,
                    IsDeleted = this.Control.IsDeleted,
                }:null) ?? new ControlDto(),
                EvidenceId = this.EvidenceId ,
                 Evidence = (this.Evidence!= null ? new  EvidenceDto
                {
                    EvidenceId = this.EvidenceId,
                    TenantId = this.Evidence.TenantId,
                    EvidenceCode = this.Evidence.EvidenceCode,
                    EvidenceTitle = this.Evidence.EvidenceTitle,
                    EvidenceType = this.Evidence.EvidenceType,
                    CollectedDate = this.Evidence.CollectedDate,
                    FileHash = this.Evidence.FileHash,
                    StorageUri = this.Evidence.StorageUri,
                    IsValid = this.Evidence.IsValid,
                    CreatedAt = this.Evidence.CreatedAt,
                    CreatedBy = this.Evidence.CreatedBy,
                    UpdatedAt = this.Evidence.UpdatedAt,
                    UpdatedBy = this.Evidence.UpdatedBy,
                    IsDeleted = this.Evidence.IsDeleted,
                }:null) ?? new EvidenceDto(),
            };
        }
        public ControlEvidenceDto() { }
        public ControlEvidenceDto(Guid assessmentId, Guid controlId, Guid evidenceId, Guid tenantId, bool isPrimary, DateTime createdAt, Guid createdBy, bool isDeleted)
        {
                this.AssessmentId = Guard.Against.Default(assessmentId, nameof(assessmentId));
                this.ControlId = Guard.Against.Default(controlId, nameof(controlId));
                this.EvidenceId = Guard.Against.Default(evidenceId, nameof(evidenceId));
                this.TenantId = Guard.Against.Default(tenantId, nameof(tenantId));
                this.IsPrimary = Guard.Against.Null(isPrimary, nameof(isPrimary));
                this.CreatedAt = Guard.Against.OutOfSQLDateRange(createdAt, nameof(createdAt));
                this.CreatedBy = Guard.Against.Default(createdBy, nameof(createdBy));
                this.IsDeleted = Guard.Against.Null(isDeleted, nameof(isDeleted));
        }
        [JsonIgnore]
        private Option<ControlEvidenceDto> _snapshot = None;

        public Option<ControlEvidenceDto> CurrentSnapshot => _snapshot;

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
                    NotifyChange();
                    return Right<Error, Unit>(LanguageExt.Unit.Default);
                },
                None: () => Left<Error, Unit>(Error.New("No snapshot exists to restore"))
            );

        private void CopyFrom(ControlEvidenceDto source)
        {
        TenantId = source.TenantId;
        IsPrimary = source.IsPrimary;
        CreatedAt = source.CreatedAt;
        CreatedBy = source.CreatedBy;
        UpdatedAt = source.UpdatedAt;
        UpdatedBy = source.UpdatedBy;
        IsDeleted = source.IsDeleted;
        AssessmentId = source.AssessmentId ;
            Assessment = source.Assessment?.Clone();
        ControlId = source.ControlId ;
            Control = source.Control?.Clone();
        EvidenceId = source.EvidenceId ;
            Evidence = source.Evidence?.Clone();
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
        public Either<Error, bool> Compare(ControlEvidenceDto other)
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

        private Either<Error, bool> CompareBasicFields(ControlEvidenceDto other)
        {
            bool same =
                (RowId == other.RowId) &&
                (ControlId == other.ControlId) &&
                (EvidenceId == other.EvidenceId) &&
                (AssessmentId == other.AssessmentId) &&
                (TenantId == other.TenantId) &&
                (IsPrimary == other.IsPrimary) &&
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