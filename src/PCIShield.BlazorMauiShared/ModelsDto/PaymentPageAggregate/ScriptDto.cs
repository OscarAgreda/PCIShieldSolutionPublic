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
    public static class ScriptFactory
    {
        public static ScriptDto CreateNewFromTemplate(ScriptDto template)
        {
            var now = DateTime.UtcNow;

            var newScript = new ScriptDto
            {
                ScriptId = Guid.Empty,
                CreatedBy = template.CreatedBy,
                TenantId =  Guid.Empty,
                IsAuthorized = false,
                IsDeleted = false,
            };

            return newScript;
        }

        public static ScriptDto CreateNewEmpty()
        {
            var now = DateTime.UtcNow;
            return new ScriptDto
            {
                ScriptId = Guid.Empty,
                TenantId = Guid.Empty,
                PaymentPageId = Guid.Empty,
                CreatedBy = Guid.Empty,
                UpdatedBy = Guid.Empty,
                IsDeleted = false,
            };
        }
    }
    public static class ScriptExtensions
    {
        public static ScriptDto CloneAsNew(this ScriptDto template)
        {
            return ScriptFactory.CreateNewFromTemplate(template);
        }
    }

    public sealed class ScriptDto : ErrorIdentifiableDtoBase, IModelDto,ITrackableEntity<ScriptDto>,ISnapshotable<ScriptDto>,IChangeObservable
    {
        public Guid  ScriptId { get;  set; }

        public Guid TenantId { get;  set; }

        public string ScriptUrl { get;  set; }

        public string ScriptHash { get;  set; }

        public string ScriptType { get;  set; }

        public bool IsAuthorized { get;  set; }

        public DateTime FirstSeen { get;  set; }

        public DateTime LastSeen { get;  set; }

        public DateTime CreatedAt { get;  set; }

        public Guid CreatedBy { get;  set; }

        public DateTime? UpdatedAt { get;  set; }

        public Guid? UpdatedBy { get;  set; }

        public bool IsDeleted { get;  set; }

        public PaymentPageDto PaymentPage { get;  set; }

        public Guid PaymentPageId { get;  set; }
        public ScriptDto Clone()
        {
            return new ScriptDto
            {
                ScriptId = this.ScriptId,
                TenantId = this.TenantId,
                ScriptUrl = this.ScriptUrl,
                ScriptHash = this.ScriptHash,
                ScriptType = this.ScriptType,
                IsAuthorized = this.IsAuthorized,
                FirstSeen = this.FirstSeen,
                LastSeen = this.LastSeen,
                CreatedAt = this.CreatedAt,
                CreatedBy = this.CreatedBy,
                UpdatedAt = this.UpdatedAt,
                UpdatedBy = this.UpdatedBy,
                IsDeleted = this.IsDeleted,
                PaymentPageId = this.PaymentPageId ,
                 PaymentPage = (this.PaymentPage!= null ? new  PaymentPageDto
                {
                    PaymentPageId = this.PaymentPageId,
                    TenantId = this.PaymentPage.TenantId,
                    PageUrl = this.PaymentPage.PageUrl,
                    PageName = this.PaymentPage.PageName,
                    IsActive = this.PaymentPage.IsActive,
                    LastScriptInventory = this.PaymentPage.LastScriptInventory,
                    ScriptIntegrityHash = this.PaymentPage.ScriptIntegrityHash,
                    CreatedAt = this.PaymentPage.CreatedAt,
                    CreatedBy = this.PaymentPage.CreatedBy,
                    UpdatedAt = this.PaymentPage.UpdatedAt,
                    UpdatedBy = this.PaymentPage.UpdatedBy,
                    IsDeleted = this.PaymentPage.IsDeleted,
                }:null) ?? new PaymentPageDto(),
            };
        }
        public ScriptDto() { }
        public ScriptDto(Guid scriptId, Guid paymentPageId, Guid tenantId, string scriptUrl, string scriptHash, string scriptType, bool isAuthorized, DateTime firstSeen, DateTime lastSeen, DateTime createdAt, Guid createdBy, bool isDeleted)
        {
                AuditEntityId = scriptId.ToString();
                AuditId = Guid.CreateVersion7().ToString();
                AuditEntityType = GetType().Name;
                this.ScriptId = Guard.Against.Default(scriptId, nameof(scriptId));
                this.PaymentPageId = Guard.Against.Default(paymentPageId, nameof(paymentPageId));
                this.TenantId = Guard.Against.Default(tenantId, nameof(tenantId));
                this.ScriptUrl = Guard.Against.NullOrEmpty(scriptUrl, nameof(scriptUrl));
                this.ScriptHash = Guard.Against.NullOrEmpty(scriptHash, nameof(scriptHash));
                this.ScriptType = Guard.Against.NullOrEmpty(scriptType, nameof(scriptType));
                this.IsAuthorized = Guard.Against.Null(isAuthorized, nameof(isAuthorized));
                this.FirstSeen = Guard.Against.OutOfSQLDateRange(firstSeen, nameof(firstSeen));
                this.LastSeen = Guard.Against.OutOfSQLDateRange(lastSeen, nameof(lastSeen));
                this.CreatedAt = Guard.Against.OutOfSQLDateRange(createdAt, nameof(createdAt));
                this.CreatedBy = Guard.Against.Default(createdBy, nameof(createdBy));
                this.IsDeleted = Guard.Against.Null(isDeleted, nameof(isDeleted));
        }
        [JsonIgnore]
        private Option<ScriptDto> _snapshot = None;

        public Option<ScriptDto> CurrentSnapshot => _snapshot;

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

        private void CopyFrom(ScriptDto source)
        {
        ScriptId = source.ScriptId;
        TenantId = source.TenantId;
        ScriptUrl = source.ScriptUrl;
        ScriptHash = source.ScriptHash;
        ScriptType = source.ScriptType;
        IsAuthorized = source.IsAuthorized;
        FirstSeen = source.FirstSeen;
        LastSeen = source.LastSeen;
        CreatedAt = source.CreatedAt;
        CreatedBy = source.CreatedBy;
        UpdatedAt = source.UpdatedAt;
        UpdatedBy = source.UpdatedBy;
        IsDeleted = source.IsDeleted;
        PaymentPageId = source.PaymentPageId ;
            PaymentPage = source.PaymentPage?.Clone();
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
        public Either<Error, bool> Compare(ScriptDto other)
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

        private Either<Error, bool> CompareBasicFields(ScriptDto other)
        {
            bool same =
                (ScriptId == other.ScriptId) &&
                (TenantId == other.TenantId) &&
                (PaymentPageId == other.PaymentPageId) &&
                StringComparer.OrdinalIgnoreCase.Equals(ScriptUrl, other.ScriptUrl) &&
                StringComparer.OrdinalIgnoreCase.Equals(ScriptHash, other.ScriptHash) &&
                StringComparer.OrdinalIgnoreCase.Equals(ScriptType, other.ScriptType) &&
                (IsAuthorized == other.IsAuthorized) &&
                (FirstSeen == other.FirstSeen) &&
                (LastSeen == other.LastSeen) &&
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