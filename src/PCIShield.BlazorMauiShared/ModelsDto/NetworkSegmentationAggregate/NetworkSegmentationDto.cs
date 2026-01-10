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
    public static class NetworkSegmentationFactory
    {
        public static NetworkSegmentationDto CreateNewFromTemplate(NetworkSegmentationDto template)
        {
            var now = DateTime.UtcNow;

            var newNetworkSegmentation = new NetworkSegmentationDto
            {
                NetworkSegmentationId = Guid.Empty,
                CreatedBy = template.CreatedBy,
                TenantId =  Guid.Empty,
                IsInCDE = false,
                IsDeleted = false,
            };

            return newNetworkSegmentation;
        }

        public static NetworkSegmentationDto CreateNewEmpty()
        {
            var now = DateTime.UtcNow;
            return new NetworkSegmentationDto
            {
                NetworkSegmentationId = Guid.Empty,
                TenantId = Guid.Empty,
                MerchantId = Guid.Empty,
                CreatedBy = Guid.Empty,
                UpdatedBy = Guid.Empty,
                IsDeleted = false,
            };
        }
    }
    public static class NetworkSegmentationExtensions
    {
        public static NetworkSegmentationDto CloneAsNew(this NetworkSegmentationDto template)
        {
            return NetworkSegmentationFactory.CreateNewFromTemplate(template);
        }
    }

    public sealed class NetworkSegmentationDto : ErrorIdentifiableDtoBase, IModelDto,ITrackableEntity<NetworkSegmentationDto>,ISnapshotable<NetworkSegmentationDto>,IChangeObservable
    {
        public Guid  NetworkSegmentationId { get;  set; }

        public Guid TenantId { get;  set; }

        public string SegmentName { get;  set; }

        public int? VLANId { get;  set; }

        public string IPRange { get;  set; }

        public string? FirewallRules { get;  set; }

        public bool IsInCDE { get;  set; }

        public DateTime? LastValidated { get;  set; }

        public DateTime CreatedAt { get;  set; }

        public Guid CreatedBy { get;  set; }

        public DateTime? UpdatedAt { get;  set; }

        public Guid? UpdatedBy { get;  set; }

        public bool IsDeleted { get;  set; }
        public Guid ROCPackageId { get; set; }
        public ROCPackageDto? ROCPackage { get; set; }
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
        public NetworkSegmentationDto Clone()
        {
            return new NetworkSegmentationDto
            {
                NetworkSegmentationId = this.NetworkSegmentationId,
                TenantId = this.TenantId,
                SegmentName = this.SegmentName,
                VLANId = this.VLANId,
                IPRange = this.IPRange,
                FirewallRules = this.FirewallRules,
                IsInCDE = this.IsInCDE,
                LastValidated = this.LastValidated,
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
        public NetworkSegmentationDto() { }
        public NetworkSegmentationDto(Guid networkSegmentationId, Guid merchantId, Guid tenantId, string segmentName, string iprange, bool isInCDE, DateTime createdAt, Guid createdBy, bool isDeleted)
        {
                AuditEntityId = networkSegmentationId.ToString();
                AuditId = Guid.CreateVersion7().ToString();
                AuditEntityType = GetType().Name;
                this.NetworkSegmentationId = Guard.Against.Default(networkSegmentationId, nameof(networkSegmentationId));
                this.MerchantId = Guard.Against.Default(merchantId, nameof(merchantId));
                this.TenantId = Guard.Against.Default(tenantId, nameof(tenantId));
                this.SegmentName = Guard.Against.NullOrEmpty(segmentName, nameof(segmentName));
                this.IPRange = Guard.Against.NullOrEmpty(iprange, nameof(iprange));
                this.IsInCDE = Guard.Against.Null(isInCDE, nameof(isInCDE));
                this.CreatedAt = Guard.Against.OutOfSQLDateRange(createdAt, nameof(createdAt));
                this.CreatedBy = Guard.Against.Default(createdBy, nameof(createdBy));
                this.IsDeleted = Guard.Against.Null(isDeleted, nameof(isDeleted));
        }
        [JsonIgnore]
        private Option<NetworkSegmentationDto> _snapshot = None;

        public Option<NetworkSegmentationDto> CurrentSnapshot => _snapshot;

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

        private void CopyFrom(NetworkSegmentationDto source)
        {
        NetworkSegmentationId = source.NetworkSegmentationId;
        TenantId = source.TenantId;
        SegmentName = source.SegmentName;
        VLANId = source.VLANId;
        IPRange = source.IPRange;
        FirewallRules = source.FirewallRules;
        IsInCDE = source.IsInCDE;
        LastValidated = source.LastValidated;
        CreatedAt = source.CreatedAt;
        CreatedBy = source.CreatedBy;
        UpdatedAt = source.UpdatedAt;
        UpdatedBy = source.UpdatedBy;
        IsDeleted = source.IsDeleted;
        MerchantId = source.MerchantId ;
            Merchant = source.Merchant?.Clone();
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
        public Either<Error, bool> Compare(NetworkSegmentationDto other)
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

        private Either<Error, bool> CompareBasicFields(NetworkSegmentationDto other)
        {
            bool same =
                (NetworkSegmentationId == other.NetworkSegmentationId) &&
                (TenantId == other.TenantId) &&
                (MerchantId == other.MerchantId) &&
                StringComparer.OrdinalIgnoreCase.Equals(SegmentName, other.SegmentName) &&
                (VLANId == other.VLANId) &&
                StringComparer.OrdinalIgnoreCase.Equals(IPRange, other.IPRange) &&
                StringComparer.OrdinalIgnoreCase.Equals(FirewallRules, other.FirewallRules) &&
                (IsInCDE == other.IsInCDE) &&
                (LastValidated == other.LastValidated) &&
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