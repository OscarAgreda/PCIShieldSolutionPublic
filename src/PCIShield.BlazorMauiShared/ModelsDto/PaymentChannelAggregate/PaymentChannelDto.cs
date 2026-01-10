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
    public static class PaymentChannelFactory
    {
        public static PaymentChannelDto CreateNewFromTemplate(PaymentChannelDto template)
        {
            var now = DateTime.UtcNow;

            var newPaymentChannel = new PaymentChannelDto
            {
                PaymentChannelId = Guid.Empty,
                CreatedBy = template.CreatedBy,
                TenantId =  Guid.Empty,
                ProcessingVolume = 0,
                IsInScope = false,
                TokenizationEnabled = false,
                IsDeleted = false,
                PaymentPages = new(),
            };

            return newPaymentChannel;
        }

        public static PaymentChannelDto CreateNewEmpty()
        {
            var now = DateTime.UtcNow;
            return new PaymentChannelDto
            {
                PaymentChannelId = Guid.Empty,
                TenantId = Guid.Empty,
                MerchantId = Guid.Empty,
                CreatedBy = Guid.Empty,
                UpdatedBy = Guid.Empty,
                IsDeleted = false,
            };
        }
    }
    public static class PaymentChannelExtensions
    {
        public static PaymentChannelDto CloneAsNew(this PaymentChannelDto template)
        {
            return PaymentChannelFactory.CreateNewFromTemplate(template);
        }
    }

    public sealed class PaymentChannelDto : ErrorIdentifiableDtoBase, IModelDto,ITrackableEntity<PaymentChannelDto>,ISnapshotable<PaymentChannelDto>,IChangeObservable
    {
        public Guid  PaymentChannelId { get;  set; }

        public Guid TenantId { get;  set; }

        public string ChannelCode { get;  set; }

        public string ChannelName { get;  set; }

        public int ChannelType { get;  set; }

        public decimal ProcessingVolume { get;  set; }

        public bool IsInScope { get;  set; }

        public bool TokenizationEnabled { get;  set; }

        public DateTime CreatedAt { get;  set; }

        public Guid CreatedBy { get;  set; }

        public DateTime? UpdatedAt { get;  set; }

        public Guid? UpdatedBy { get;  set; }

        public bool IsDeleted { get;  set; }
        public int ActivePaymentPageCount { get; set; }
        public int DaysSinceUpdatedAt { get; set; }
        public bool IsChannelTypeCritical { get; set; }
        public bool IsInScopeFlag { get; set; }
        public DateTime? LatestPaymentPageUpdatedAt { get; set; }
        public List<ScriptDto> Scripts { get; set; }
        public int TotalPaymentPageCount { get; set; }
        public Guid ROCPackageId { get; set; }
        public ROCPackageDto? ROCPackage { get; set; }
        public Guid ScanScheduleId { get; set; }
        public ScanScheduleDto? ScanSchedule { get; set; }
        public Guid VulnerabilityId { get; set; }
        public VulnerabilityDto? Vulnerability { get; set; }

        public MerchantDto Merchant { get;  set; }

        public Guid MerchantId { get;  set; }
        public PaymentChannelDto Clone()
        {
            return new PaymentChannelDto
            {
                PaymentChannelId = this.PaymentChannelId,
                TenantId = this.TenantId,
                ChannelCode = this.ChannelCode,
                ChannelName = this.ChannelName,
                ChannelType = this.ChannelType,
                ProcessingVolume = this.ProcessingVolume,
                IsInScope = this.IsInScope,
                TokenizationEnabled = this.TokenizationEnabled,
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
        public List<PaymentPageDto> PaymentPages { get; set; } = new();
        public PaymentChannelDto() { }
        public PaymentChannelDto(Guid paymentChannelId, Guid merchantId, Guid tenantId, string channelCode, string channelName, int channelType, decimal processingVolume, bool isInScope, bool tokenizationEnabled, DateTime createdAt, Guid createdBy, bool isDeleted)
        {
                AuditEntityId = paymentChannelId.ToString();
                AuditId = Guid.CreateVersion7().ToString();
                AuditEntityType = GetType().Name;
                this.PaymentChannelId = Guard.Against.Default(paymentChannelId, nameof(paymentChannelId));
                this.MerchantId = Guard.Against.Default(merchantId, nameof(merchantId));
                this.TenantId = Guard.Against.Default(tenantId, nameof(tenantId));
                this.ChannelCode = Guard.Against.NullOrEmpty(channelCode, nameof(channelCode));
                this.ChannelName = Guard.Against.NullOrEmpty(channelName, nameof(channelName));
                this.ChannelType = Guard.Against.Negative(channelType, nameof(channelType));
                this.ProcessingVolume = Guard.Against.Negative(processingVolume, nameof(processingVolume));
                this.IsInScope = Guard.Against.Null(isInScope, nameof(isInScope));
                this.TokenizationEnabled = Guard.Against.Null(tokenizationEnabled, nameof(tokenizationEnabled));
                this.CreatedAt = Guard.Against.OutOfSQLDateRange(createdAt, nameof(createdAt));
                this.CreatedBy = Guard.Against.Default(createdBy, nameof(createdBy));
                this.IsDeleted = Guard.Against.Null(isDeleted, nameof(isDeleted));

         this.PaymentPages = new();
        }
        [JsonIgnore]
        private Option<PaymentChannelDto> _snapshot = None;

        public Option<PaymentChannelDto> CurrentSnapshot => _snapshot;

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

        private void CopyFrom(PaymentChannelDto source)
        {
        PaymentChannelId = source.PaymentChannelId;
        TenantId = source.TenantId;
        ChannelCode = source.ChannelCode;
        ChannelName = source.ChannelName;
        ChannelType = source.ChannelType;
        ProcessingVolume = source.ProcessingVolume;
        IsInScope = source.IsInScope;
        TokenizationEnabled = source.TokenizationEnabled;
        CreatedAt = source.CreatedAt;
        CreatedBy = source.CreatedBy;
        UpdatedAt = source.UpdatedAt;
        UpdatedBy = source.UpdatedBy;
        IsDeleted = source.IsDeleted;
        MerchantId = source.MerchantId ;
            Merchant = source.Merchant?.Clone();
            PaymentPages = new List<PaymentPageDto>(source.PaymentPages);
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
        public Either<Error, bool> Compare(PaymentChannelDto other)
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

        private Either<Error, bool> CompareBasicFields(PaymentChannelDto other)
        {
            bool same =
                (PaymentChannelId == other.PaymentChannelId) &&
                (TenantId == other.TenantId) &&
                (MerchantId == other.MerchantId) &&
                StringComparer.OrdinalIgnoreCase.Equals(ChannelCode, other.ChannelCode) &&
                StringComparer.OrdinalIgnoreCase.Equals(ChannelName, other.ChannelName) &&
                (ChannelType == other.ChannelType) &&
                (ProcessingVolume == other.ProcessingVolume) &&
                (IsInScope == other.IsInScope) &&
                (TokenizationEnabled == other.TokenizationEnabled) &&
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