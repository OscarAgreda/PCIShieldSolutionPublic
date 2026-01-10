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
    public static class ControlCategoryFactory
    {
        public static ControlCategoryDto CreateNewFromTemplate(ControlCategoryDto template)
        {
            var now = DateTime.UtcNow;

            var newControlCategory = new ControlCategoryDto
            {
                ControlCategoryId = Guid.Empty,
                CreatedBy = template.CreatedBy,
                IsActive = false,
            };

            return newControlCategory;
        }

        public static ControlCategoryDto CreateNewEmpty()
        {
            var now = DateTime.UtcNow;
            return new ControlCategoryDto
            {
                ControlCategoryId = Guid.Empty,
                CreatedBy = Guid.Empty,
                UpdatedBy = Guid.Empty,
                IsActive = true,
            };
        }
    }
    public static class ControlCategoryExtensions
    {
        public static ControlCategoryDto CloneAsNew(this ControlCategoryDto template)
        {
            return ControlCategoryFactory.CreateNewFromTemplate(template);
        }
    }

    public sealed class ControlCategoryDto : ErrorIdentifiableDtoBase, IModelDto,ITrackableEntity<ControlCategoryDto>,ISnapshotable<ControlCategoryDto>,IChangeObservable
    {
        public Guid  ControlCategoryId { get;  set; }

        public string ControlCategoryCode { get;  set; }

        public string ControlCategoryName { get;  set; }

        public string RequirementSection { get;  set; }

        public bool IsActive { get;  set; }

        public DateTime CreatedAt { get;  set; }

        public Guid CreatedBy { get;  set; }

        public DateTime? UpdatedAt { get;  set; }

        public Guid? UpdatedBy { get;  set; }
        public ControlCategoryDto Clone()
        {
            return new ControlCategoryDto
            {
                ControlCategoryId = this.ControlCategoryId,
                ControlCategoryCode = this.ControlCategoryCode,
                ControlCategoryName = this.ControlCategoryName,
                RequirementSection = this.RequirementSection,
                IsActive = this.IsActive,
                CreatedAt = this.CreatedAt,
                CreatedBy = this.CreatedBy,
                UpdatedAt = this.UpdatedAt,
                UpdatedBy = this.UpdatedBy,
            };
        }
        public ControlCategoryDto() { }
        public ControlCategoryDto(Guid controlCategoryId, string controlCategoryCode, string controlCategoryName, string requirementSection, bool isActive, DateTime createdAt, Guid createdBy)
        {
                AuditEntityId = controlCategoryId.ToString();
                AuditId = Guid.CreateVersion7().ToString();
                AuditEntityType = GetType().Name;
                this.ControlCategoryId = Guard.Against.Default(controlCategoryId, nameof(controlCategoryId));
                this.ControlCategoryCode = Guard.Against.NullOrEmpty(controlCategoryCode, nameof(controlCategoryCode));
                this.ControlCategoryName = Guard.Against.NullOrEmpty(controlCategoryName, nameof(controlCategoryName));
                this.RequirementSection = Guard.Against.NullOrEmpty(requirementSection, nameof(requirementSection));
                this.IsActive = Guard.Against.Null(isActive, nameof(isActive));
                this.CreatedAt = Guard.Against.OutOfSQLDateRange(createdAt, nameof(createdAt));
                this.CreatedBy = Guard.Against.Default(createdBy, nameof(createdBy));
        }
        [JsonIgnore]
        private Option<ControlCategoryDto> _snapshot = None;

        public Option<ControlCategoryDto> CurrentSnapshot => _snapshot;

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

        private void CopyFrom(ControlCategoryDto source)
        {
        ControlCategoryId = source.ControlCategoryId;
        ControlCategoryCode = source.ControlCategoryCode;
        ControlCategoryName = source.ControlCategoryName;
        RequirementSection = source.RequirementSection;
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
        public Either<Error, bool> Compare(ControlCategoryDto other)
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

        private Either<Error, bool> CompareBasicFields(ControlCategoryDto other)
        {
            bool same =
                (ControlCategoryId == other.ControlCategoryId) &&
                StringComparer.OrdinalIgnoreCase.Equals(ControlCategoryCode, other.ControlCategoryCode) &&
                StringComparer.OrdinalIgnoreCase.Equals(ControlCategoryName, other.ControlCategoryName) &&
                StringComparer.OrdinalIgnoreCase.Equals(RequirementSection, other.RequirementSection) &&
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