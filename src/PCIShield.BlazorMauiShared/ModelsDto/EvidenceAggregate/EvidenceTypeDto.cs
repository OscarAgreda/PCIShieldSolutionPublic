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
    public static class EvidenceTypeFactory
    {
        public static EvidenceTypeDto CreateNewFromTemplate(EvidenceTypeDto template)
        {
            var now = DateTime.UtcNow;

            var newEvidenceType = new EvidenceTypeDto
            {
                EvidenceTypeId = Guid.Empty,
                CreatedBy = template.CreatedBy,
                IsActive = false,
            };

            return newEvidenceType;
        }

        public static EvidenceTypeDto CreateNewEmpty()
        {
            var now = DateTime.UtcNow;
            return new EvidenceTypeDto
            {
                EvidenceTypeId = Guid.Empty,
                CreatedBy = Guid.Empty,
                UpdatedBy = Guid.Empty,
                IsActive = true,
            };
        }
    }
    public static class EvidenceTypeExtensions
    {
        public static EvidenceTypeDto CloneAsNew(this EvidenceTypeDto template)
        {
            return EvidenceTypeFactory.CreateNewFromTemplate(template);
        }
    }

    public sealed class EvidenceTypeDto : ErrorIdentifiableDtoBase, IModelDto,ITrackableEntity<EvidenceTypeDto>,ISnapshotable<EvidenceTypeDto>,IChangeObservable
    {
        public Guid  EvidenceTypeId { get;  set; }

        public string EvidenceTypeCode { get;  set; }

        public string EvidenceTypeName { get;  set; }

        public string? FileExtensions { get;  set; }

        public int? MaxSizeMB { get;  set; }

        public bool IsActive { get;  set; }

        public DateTime CreatedAt { get;  set; }

        public Guid CreatedBy { get;  set; }

        public DateTime? UpdatedAt { get;  set; }

        public Guid? UpdatedBy { get;  set; }
        public EvidenceTypeDto Clone()
        {
            return new EvidenceTypeDto
            {
                EvidenceTypeId = this.EvidenceTypeId,
                EvidenceTypeCode = this.EvidenceTypeCode,
                EvidenceTypeName = this.EvidenceTypeName,
                FileExtensions = this.FileExtensions,
                MaxSizeMB = this.MaxSizeMB,
                IsActive = this.IsActive,
                CreatedAt = this.CreatedAt,
                CreatedBy = this.CreatedBy,
                UpdatedAt = this.UpdatedAt,
                UpdatedBy = this.UpdatedBy,
            };
        }
        public EvidenceTypeDto() { }
        public EvidenceTypeDto(Guid evidenceTypeId, string evidenceTypeCode, string evidenceTypeName, bool isActive, DateTime createdAt, Guid createdBy)
        {
                AuditEntityId = evidenceTypeId.ToString();
                AuditId = Guid.CreateVersion7().ToString();
                AuditEntityType = GetType().Name;
                this.EvidenceTypeId = Guard.Against.Default(evidenceTypeId, nameof(evidenceTypeId));
                this.EvidenceTypeCode = Guard.Against.NullOrEmpty(evidenceTypeCode, nameof(evidenceTypeCode));
                this.EvidenceTypeName = Guard.Against.NullOrEmpty(evidenceTypeName, nameof(evidenceTypeName));
                this.IsActive = Guard.Against.Null(isActive, nameof(isActive));
                this.CreatedAt = Guard.Against.OutOfSQLDateRange(createdAt, nameof(createdAt));
                this.CreatedBy = Guard.Against.Default(createdBy, nameof(createdBy));
        }
        [JsonIgnore]
        private Option<EvidenceTypeDto> _snapshot = None;

        public Option<EvidenceTypeDto> CurrentSnapshot => _snapshot;

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

        private void CopyFrom(EvidenceTypeDto source)
        {
        EvidenceTypeId = source.EvidenceTypeId;
        EvidenceTypeCode = source.EvidenceTypeCode;
        EvidenceTypeName = source.EvidenceTypeName;
        FileExtensions = source.FileExtensions;
        MaxSizeMB = source.MaxSizeMB;
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
        public Either<Error, bool> Compare(EvidenceTypeDto other)
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

        private Either<Error, bool> CompareBasicFields(EvidenceTypeDto other)
        {
            bool same =
                (EvidenceTypeId == other.EvidenceTypeId) &&
                StringComparer.OrdinalIgnoreCase.Equals(EvidenceTypeCode, other.EvidenceTypeCode) &&
                StringComparer.OrdinalIgnoreCase.Equals(EvidenceTypeName, other.EvidenceTypeName) &&
                StringComparer.OrdinalIgnoreCase.Equals(FileExtensions, other.FileExtensions) &&
                (MaxSizeMB == other.MaxSizeMB) &&
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