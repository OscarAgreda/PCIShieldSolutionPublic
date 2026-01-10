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
    public static class AssessmentTypeFactory
    {
        public static AssessmentTypeDto CreateNewFromTemplate(AssessmentTypeDto template)
        {
            var now = DateTime.UtcNow;

            var newAssessmentType = new AssessmentTypeDto
            {
                AssessmentTypeId = Guid.Empty,
                CreatedBy = template.CreatedBy,
                IsActive = false,
            };

            return newAssessmentType;
        }

        public static AssessmentTypeDto CreateNewEmpty()
        {
            var now = DateTime.UtcNow;
            return new AssessmentTypeDto
            {
                AssessmentTypeId = Guid.Empty,
                CreatedBy = Guid.Empty,
                UpdatedBy = Guid.Empty,
                IsActive = true,
            };
        }
    }
    public static class AssessmentTypeExtensions
    {
        public static AssessmentTypeDto CloneAsNew(this AssessmentTypeDto template)
        {
            return AssessmentTypeFactory.CreateNewFromTemplate(template);
        }
    }

    public sealed class AssessmentTypeDto : ErrorIdentifiableDtoBase, IModelDto,ITrackableEntity<AssessmentTypeDto>,ISnapshotable<AssessmentTypeDto>,IChangeObservable
    {
        public Guid  AssessmentTypeId { get;  set; }

        public string AssessmentTypeCode { get;  set; }

        public string AssessmentTypeName { get;  set; }

        public string? Description { get;  set; }

        public bool IsActive { get;  set; }

        public DateTime CreatedAt { get;  set; }

        public Guid CreatedBy { get;  set; }

        public DateTime? UpdatedAt { get;  set; }

        public Guid? UpdatedBy { get;  set; }
        public AssessmentTypeDto Clone()
        {
            return new AssessmentTypeDto
            {
                AssessmentTypeId = this.AssessmentTypeId,
                AssessmentTypeCode = this.AssessmentTypeCode,
                AssessmentTypeName = this.AssessmentTypeName,
                Description = this.Description,
                IsActive = this.IsActive,
                CreatedAt = this.CreatedAt,
                CreatedBy = this.CreatedBy,
                UpdatedAt = this.UpdatedAt,
                UpdatedBy = this.UpdatedBy,
            };
        }
        public AssessmentTypeDto() { }
        public AssessmentTypeDto(Guid assessmentTypeId, string assessmentTypeCode, string assessmentTypeName, bool isActive, DateTime createdAt, Guid createdBy)
        {
                AuditEntityId = assessmentTypeId.ToString();
                AuditId = Guid.CreateVersion7().ToString();
                AuditEntityType = GetType().Name;
                this.AssessmentTypeId = Guard.Against.Default(assessmentTypeId, nameof(assessmentTypeId));
                this.AssessmentTypeCode = Guard.Against.NullOrEmpty(assessmentTypeCode, nameof(assessmentTypeCode));
                this.AssessmentTypeName = Guard.Against.NullOrEmpty(assessmentTypeName, nameof(assessmentTypeName));
                this.IsActive = Guard.Against.Null(isActive, nameof(isActive));
                this.CreatedAt = Guard.Against.OutOfSQLDateRange(createdAt, nameof(createdAt));
                this.CreatedBy = Guard.Against.Default(createdBy, nameof(createdBy));
        }
        [JsonIgnore]
        private Option<AssessmentTypeDto> _snapshot = None;

        public Option<AssessmentTypeDto> CurrentSnapshot => _snapshot;

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

        private void CopyFrom(AssessmentTypeDto source)
        {
        AssessmentTypeId = source.AssessmentTypeId;
        AssessmentTypeCode = source.AssessmentTypeCode;
        AssessmentTypeName = source.AssessmentTypeName;
        Description = source.Description;
        IsActive = source.IsActive;
        CreatedAt = source.CreatedAt;
        CreatedBy = source.CreatedBy;
        UpdatedAt = source.UpdatedAt;
        UpdatedBy = source.UpdatedBy;
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
        public Either<Error, bool> Compare(AssessmentTypeDto other)
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

        private Either<Error, bool> CompareBasicFields(AssessmentTypeDto other)
        {
            bool same =
                (AssessmentTypeId == other.AssessmentTypeId) &&
                StringComparer.OrdinalIgnoreCase.Equals(AssessmentTypeCode, other.AssessmentTypeCode) &&
                StringComparer.OrdinalIgnoreCase.Equals(AssessmentTypeName, other.AssessmentTypeName) &&
                StringComparer.OrdinalIgnoreCase.Equals(Description, other.Description) &&
                (IsActive == other.IsActive) &&
                (CreatedAt == other.CreatedAt) &&
                (CreatedBy == other.CreatedBy) &&
                (UpdatedAt == other.UpdatedAt) &&
                (UpdatedBy == other.UpdatedBy) ;
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