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
    public static class PaymentPageFactory
    {
        public static PaymentPageDto CreateNewFromTemplate(PaymentPageDto template)
        {
            var now = DateTime.UtcNow;

            var newPaymentPage = new PaymentPageDto
            {
                PaymentPageId = Guid.Empty,
                CreatedBy = template.CreatedBy,
                TenantId =  Guid.Empty,
                IsActive = false,
                IsDeleted = false,
                Scripts = new(),
            };

            return newPaymentPage;
        }

        public static PaymentPageDto CreateNewEmpty()
        {
            var now = DateTime.UtcNow;
            return new PaymentPageDto
            {
                PaymentPageId = Guid.Empty,
                TenantId = Guid.Empty,
                PaymentChannelId = Guid.Empty,
                CreatedBy = Guid.Empty,
                UpdatedBy = Guid.Empty,
                IsActive = true,
                IsDeleted = false,
            };
        }
    }
    public static class PaymentPageExtensions
    {
        public static PaymentPageDto CloneAsNew(this PaymentPageDto template)
        {
            return PaymentPageFactory.CreateNewFromTemplate(template);
        }
    }

    public sealed class PaymentPageDto : ErrorIdentifiableDtoBase, IModelDto,ITrackableEntity<PaymentPageDto>,ISnapshotable<PaymentPageDto>,IChangeObservable
    {
        public Guid  PaymentPageId { get;  set; }

        public Guid TenantId { get;  set; }

        public string PageUrl { get;  set; }

        public string PageName { get;  set; }

        public bool IsActive { get;  set; }

        public DateTime? LastScriptInventory { get;  set; }

        public string? ScriptIntegrityHash { get;  set; }

        public DateTime CreatedAt { get;  set; }

        public Guid CreatedBy { get;  set; }

        public DateTime? UpdatedAt { get;  set; }

        public Guid? UpdatedBy { get;  set; }

        public bool IsDeleted { get;  set; }

        public PaymentChannelDto PaymentChannel { get;  set; }

        public Guid PaymentChannelId { get;  set; }
        public PaymentPageDto Clone()
        {
            return new PaymentPageDto
            {
                PaymentPageId = this.PaymentPageId,
                TenantId = this.TenantId,
                PageUrl = this.PageUrl,
                PageName = this.PageName,
                IsActive = this.IsActive,
                LastScriptInventory = this.LastScriptInventory,
                ScriptIntegrityHash = this.ScriptIntegrityHash,
                CreatedAt = this.CreatedAt,
                CreatedBy = this.CreatedBy,
                UpdatedAt = this.UpdatedAt,
                UpdatedBy = this.UpdatedBy,
                IsDeleted = this.IsDeleted,
                PaymentChannelId = this.PaymentChannelId ,
                 PaymentChannel = (this.PaymentChannel!= null ? new  PaymentChannelDto
                {
                    PaymentChannelId = this.PaymentChannelId,
                    TenantId = this.PaymentChannel.TenantId,
                    ChannelCode = this.PaymentChannel.ChannelCode,
                    ChannelName = this.PaymentChannel.ChannelName,
                    ChannelType = this.PaymentChannel.ChannelType,
                    ProcessingVolume = this.PaymentChannel.ProcessingVolume,
                    IsInScope = this.PaymentChannel.IsInScope,
                    TokenizationEnabled = this.PaymentChannel.TokenizationEnabled,
                    CreatedAt = this.PaymentChannel.CreatedAt,
                    CreatedBy = this.PaymentChannel.CreatedBy,
                    UpdatedAt = this.PaymentChannel.UpdatedAt,
                    UpdatedBy = this.PaymentChannel.UpdatedBy,
                    IsDeleted = this.PaymentChannel.IsDeleted,
                }:null) ?? new PaymentChannelDto(),
            };
        }
        public List<ScriptDto> Scripts { get; set; } = new();
        public PaymentPageDto() { }
        public PaymentPageDto(Guid paymentPageId, Guid paymentChannelId, Guid tenantId, string pageUrl, string pageName, bool isActive, DateTime createdAt, Guid createdBy, bool isDeleted)
        {
                AuditEntityId = paymentPageId.ToString();
                AuditId = Guid.CreateVersion7().ToString();
                AuditEntityType = GetType().Name;
                this.PaymentPageId = Guard.Against.Default(paymentPageId, nameof(paymentPageId));
                this.PaymentChannelId = Guard.Against.Default(paymentChannelId, nameof(paymentChannelId));
                this.TenantId = Guard.Against.Default(tenantId, nameof(tenantId));
                this.PageUrl = Guard.Against.NullOrEmpty(pageUrl, nameof(pageUrl));
                this.PageName = Guard.Against.NullOrEmpty(pageName, nameof(pageName));
                this.IsActive = Guard.Against.Null(isActive, nameof(isActive));
                this.CreatedAt = Guard.Against.OutOfSQLDateRange(createdAt, nameof(createdAt));
                this.CreatedBy = Guard.Against.Default(createdBy, nameof(createdBy));
                this.IsDeleted = Guard.Against.Null(isDeleted, nameof(isDeleted));

         this.Scripts = new();
        }
        [JsonIgnore]
        private Option<PaymentPageDto> _snapshot = None;

        public Option<PaymentPageDto> CurrentSnapshot => _snapshot;

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

        private void CopyFrom(PaymentPageDto source)
        {
        PaymentPageId = source.PaymentPageId;
        TenantId = source.TenantId;
        PageUrl = source.PageUrl;
        PageName = source.PageName;
        IsActive = source.IsActive;
        LastScriptInventory = source.LastScriptInventory;
        ScriptIntegrityHash = source.ScriptIntegrityHash;
        CreatedAt = source.CreatedAt;
        CreatedBy = source.CreatedBy;
        UpdatedAt = source.UpdatedAt;
        UpdatedBy = source.UpdatedBy;
        IsDeleted = source.IsDeleted;
        PaymentChannelId = source.PaymentChannelId ;
            PaymentChannel = source.PaymentChannel?.Clone();
            Scripts = new List<ScriptDto>(source.Scripts);
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
        public Either<Error, bool> Compare(PaymentPageDto other)
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

        private Either<Error, bool> CompareBasicFields(PaymentPageDto other)
        {
            bool same =
                (PaymentPageId == other.PaymentPageId) &&
                (TenantId == other.TenantId) &&
                (PaymentChannelId == other.PaymentChannelId) &&
                StringComparer.OrdinalIgnoreCase.Equals(PageUrl, other.PageUrl) &&
                StringComparer.OrdinalIgnoreCase.Equals(PageName, other.PageName) &&
                (IsActive == other.IsActive) &&
                (LastScriptInventory == other.LastScriptInventory) &&
                StringComparer.OrdinalIgnoreCase.Equals(ScriptIntegrityHash, other.ScriptIntegrityHash) &&
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