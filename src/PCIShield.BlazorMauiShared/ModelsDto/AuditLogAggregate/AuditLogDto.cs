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
    public static class AuditLogFactory
    {
        public static AuditLogDto CreateNewFromTemplate(AuditLogDto template)
        {
            var now = DateTime.UtcNow;

            var newAuditLog = new AuditLogDto
            {
                AuditLogId = Guid.Empty,
                TenantId =  Guid.Empty,
                EntityId =  Guid.Empty,
                UserId =  Guid.Empty,
            };

            return newAuditLog;
        }

        public static AuditLogDto CreateNewEmpty()
        {
            var now = DateTime.UtcNow;
            return new AuditLogDto
            {
                AuditLogId = Guid.Empty,
                TenantId = Guid.Empty,
                EntityId = Guid.Empty,
                UserId = Guid.Empty,
            };
        }
    }
    public static class AuditLogExtensions
    {
        public static AuditLogDto CloneAsNew(this AuditLogDto template)
        {
            return AuditLogFactory.CreateNewFromTemplate(template);
        }
    }

    public sealed class AuditLogDto : ErrorIdentifiableDtoBase, IModelDto,ITrackableEntity<AuditLogDto>,ISnapshotable<AuditLogDto>,IChangeObservable
    {
        public Guid  AuditLogId { get;  set; }

        public Guid TenantId { get;  set; }

        public string EntityType { get;  set; }

        public Guid EntityId { get;  set; }

        public string Action { get;  set; }

        public string? OldValues { get;  set; }

        public string? NewValues { get;  set; }

        public Guid UserId { get;  set; }

        public string? IPAddress { get;  set; }
        public AuditLogDto Clone()
        {
            return new AuditLogDto
            {
                AuditLogId = this.AuditLogId,
                TenantId = this.TenantId,
                EntityType = this.EntityType,
                EntityId = this.EntityId,
                Action = this.Action,
                OldValues = this.OldValues,
                NewValues = this.NewValues,
                UserId = this.UserId,
                IPAddress = this.IPAddress,
            };
        }
        public AuditLogDto() { }
        public AuditLogDto(Guid auditLogId, Guid tenantId, string entityType, Guid entityId, string action, Guid userId)
        {
                AuditEntityId = auditLogId.ToString();
                AuditId = Guid.CreateVersion7().ToString();
                AuditEntityType = GetType().Name;
                this.AuditLogId = Guard.Against.Default(auditLogId, nameof(auditLogId));
                this.TenantId = Guard.Against.Default(tenantId, nameof(tenantId));
                this.EntityType = Guard.Against.NullOrEmpty(entityType, nameof(entityType));
                this.EntityId = Guard.Against.Default(entityId, nameof(entityId));
                this.Action = Guard.Against.NullOrEmpty(action, nameof(action));
                this.UserId = Guard.Against.Default(userId, nameof(userId));
        }
        [JsonIgnore]
        private Option<AuditLogDto> _snapshot = None;

        public Option<AuditLogDto> CurrentSnapshot => _snapshot;

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

        private void CopyFrom(AuditLogDto source)
        {
        AuditLogId = source.AuditLogId;
        TenantId = source.TenantId;
        EntityType = source.EntityType;
        EntityId = source.EntityId;
        Action = source.Action;
        OldValues = source.OldValues;
        NewValues = source.NewValues;
        UserId = source.UserId;
        IPAddress = source.IPAddress;
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
        public Either<Error, bool> Compare(AuditLogDto other)
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

        private Either<Error, bool> CompareBasicFields(AuditLogDto other)
        {
            bool same =
                (AuditLogId == other.AuditLogId) &&
                (TenantId == other.TenantId) &&
                StringComparer.OrdinalIgnoreCase.Equals(EntityType, other.EntityType) &&
                (EntityId == other.EntityId) &&
                StringComparer.OrdinalIgnoreCase.Equals(Action, other.Action) &&
                StringComparer.OrdinalIgnoreCase.Equals(OldValues, other.OldValues) &&
                StringComparer.OrdinalIgnoreCase.Equals(NewValues, other.NewValues) &&
                (UserId == other.UserId) &&
                StringComparer.OrdinalIgnoreCase.Equals(IPAddress, other.IPAddress) ;
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