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
    public static class AssetFactory
    {
        public static AssetDto CreateNewFromTemplate(AssetDto template)
        {
            var now = DateTime.UtcNow;

            var newAsset = new AssetDto
            {
                AssetId = Guid.Empty,
                CreatedBy = template.CreatedBy,
                TenantId =  Guid.Empty,
                IsInCDE = false,
                IsDeleted = false,
                AssetControls = new(),
                ScanSchedules = new(),
                Vulnerabilities = new(),
            };

            return newAsset;
        }

        public static AssetDto CreateNewEmpty()
        {
            var now = DateTime.UtcNow;
            return new AssetDto
            {
                AssetId = Guid.Empty,
                TenantId = Guid.Empty,
                MerchantId = Guid.Empty,
                CreatedBy = Guid.Empty,
                UpdatedBy = Guid.Empty,
                IsDeleted = false,
            };
        }
    }
    public static class AssetExtensions
    {
        public static AssetDto CloneAsNew(this AssetDto template)
        {
            return AssetFactory.CreateNewFromTemplate(template);
        }
    }

    public sealed class AssetDto : ErrorIdentifiableDtoBase, IModelDto,ITrackableEntity<AssetDto>,ISnapshotable<AssetDto>,IChangeObservable
    {
        public Guid  AssetId { get;  set; }

        public Guid TenantId { get;  set; }

        public string AssetCode { get;  set; }

        public string AssetName { get;  set; }

        public int AssetType { get;  set; }

        public string? IPAddress { get;  set; }

        public string? Hostname { get;  set; }

        public bool IsInCDE { get;  set; }

        public string? NetworkZone { get;  set; }

        public DateTime? LastScanDate { get;  set; }

        public DateTime CreatedAt { get;  set; }

        public Guid CreatedBy { get;  set; }

        public DateTime? UpdatedAt { get;  set; }

        public Guid? UpdatedBy { get;  set; }

        public bool IsDeleted { get;  set; }
        public int ActiveAssetControlCount { get; set; }
        public int ActiveScanScheduleCount { get; set; }
        public int ActiveVulnerabilityCount { get; set; }
        public int CriticalScanScheduleCount { get; set; }
        public int CriticalVulnerabilityCount { get; set; }
        public int DaysSinceLastScanDate { get; set; }
        public bool IsAssetTypeCritical { get; set; }
        public bool IsInCDEFlag { get; set; }
        public DateTime? LatestAssetControlUpdatedAt { get; set; }
        public DateTime? LatestScanScheduleNextScanDate { get; set; }
        public DateTime? LatestVulnerabilityDetectedDate { get; set; }
        public int TotalAssetControlCount { get; set; }
        public int TotalScanScheduleCount { get; set; }
        public int TotalVulnerabilityCount { get; set; }
        public Guid ROCPackageId { get; set; }
        public ROCPackageDto? ROCPackage { get; set; }
        public Guid PaymentPageId { get; set; }
        public PaymentPageDto? PaymentPage { get; set; }
        public Guid ScriptId { get; set; }
        public ScriptDto? Script { get; set; }

        public MerchantDto Merchant { get;  set; }

        public Guid MerchantId { get;  set; }
        public AssetDto Clone()
        {
            return new AssetDto
            {
                AssetId = this.AssetId,
                TenantId = this.TenantId,
                AssetCode = this.AssetCode,
                AssetName = this.AssetName,
                AssetType = this.AssetType,
                IPAddress = this.IPAddress,
                Hostname = this.Hostname,
                IsInCDE = this.IsInCDE,
                NetworkZone = this.NetworkZone,
                LastScanDate = this.LastScanDate,
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
        public List<AssetControlDto> AssetControls { get; set; } = new();
        public List<ScanScheduleDto> ScanSchedules { get; set; } = new();
        public List<VulnerabilityDto> Vulnerabilities { get; set; } = new();
        public AssetDto() { }
        public AssetDto(Guid assetId, Guid merchantId, Guid tenantId, string assetCode, string assetName, int assetType, bool isInCDE, DateTime createdAt, Guid createdBy, bool isDeleted)
        {
                AuditEntityId = assetId.ToString();
                AuditId = Guid.CreateVersion7().ToString();
                AuditEntityType = GetType().Name;
                this.AssetId = Guard.Against.Default(assetId, nameof(assetId));
                this.MerchantId = Guard.Against.Default(merchantId, nameof(merchantId));
                this.TenantId = Guard.Against.Default(tenantId, nameof(tenantId));
                this.AssetCode = Guard.Against.NullOrEmpty(assetCode, nameof(assetCode));
                this.AssetName = Guard.Against.NullOrEmpty(assetName, nameof(assetName));
                this.AssetType = Guard.Against.Negative(assetType, nameof(assetType));
                this.IsInCDE = Guard.Against.Null(isInCDE, nameof(isInCDE));
                this.CreatedAt = Guard.Against.OutOfSQLDateRange(createdAt, nameof(createdAt));
                this.CreatedBy = Guard.Against.Default(createdBy, nameof(createdBy));
                this.IsDeleted = Guard.Against.Null(isDeleted, nameof(isDeleted));

         this.AssetControls = new();

         this.ScanSchedules = new();

         this.Vulnerabilities = new();
        }
        [JsonIgnore]
        private Option<AssetDto> _snapshot = None;

        public Option<AssetDto> CurrentSnapshot => _snapshot;

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

        private void CopyFrom(AssetDto source)
        {
        AssetId = source.AssetId;
        TenantId = source.TenantId;
        AssetCode = source.AssetCode;
        AssetName = source.AssetName;
        AssetType = source.AssetType;
        IPAddress = source.IPAddress;
        Hostname = source.Hostname;
        IsInCDE = source.IsInCDE;
        NetworkZone = source.NetworkZone;
        LastScanDate = source.LastScanDate;
        CreatedAt = source.CreatedAt;
        CreatedBy = source.CreatedBy;
        UpdatedAt = source.UpdatedAt;
        UpdatedBy = source.UpdatedBy;
        IsDeleted = source.IsDeleted;
        MerchantId = source.MerchantId ;
            Merchant = source.Merchant?.Clone();
            AssetControls = new List<AssetControlDto>(source.AssetControls);
            ScanSchedules = new List<ScanScheduleDto>(source.ScanSchedules);
            Vulnerabilities = new List<VulnerabilityDto>(source.Vulnerabilities);
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
        public Either<Error, bool> Compare(AssetDto other)
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

        private Either<Error, bool> CompareBasicFields(AssetDto other)
        {
            bool same =
                (AssetId == other.AssetId) &&
                (TenantId == other.TenantId) &&
                (MerchantId == other.MerchantId) &&
                StringComparer.OrdinalIgnoreCase.Equals(AssetCode, other.AssetCode) &&
                StringComparer.OrdinalIgnoreCase.Equals(AssetName, other.AssetName) &&
                (AssetType == other.AssetType) &&
                StringComparer.OrdinalIgnoreCase.Equals(IPAddress, other.IPAddress) &&
                StringComparer.OrdinalIgnoreCase.Equals(Hostname, other.Hostname) &&
                (IsInCDE == other.IsInCDE) &&
                StringComparer.OrdinalIgnoreCase.Equals(NetworkZone, other.NetworkZone) &&
                (LastScanDate == other.LastScanDate) &&
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