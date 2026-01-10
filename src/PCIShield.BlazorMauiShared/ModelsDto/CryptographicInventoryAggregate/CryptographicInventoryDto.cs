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
    public static class CryptographicInventoryFactory
    {
        public static CryptographicInventoryDto CreateNewFromTemplate(CryptographicInventoryDto template)
        {
            var now = DateTime.UtcNow;

            var newCryptographicInventory = new CryptographicInventoryDto
            {
                CryptographicInventoryId = Guid.Empty,
                CreatedBy = template.CreatedBy,
                TenantId =  Guid.Empty,
                IsDeleted = false,
            };

            return newCryptographicInventory;
        }

        public static CryptographicInventoryDto CreateNewEmpty()
        {
            var now = DateTime.UtcNow;
            return new CryptographicInventoryDto
            {
                CryptographicInventoryId = Guid.Empty,
                TenantId = Guid.Empty,
                MerchantId = Guid.Empty,
                CreatedBy = Guid.Empty,
                UpdatedBy = Guid.Empty,
                IsDeleted = false,
            };
        }
    }
    public static class CryptographicInventoryExtensions
    {
        public static CryptographicInventoryDto CloneAsNew(this CryptographicInventoryDto template)
        {
            return CryptographicInventoryFactory.CreateNewFromTemplate(template);
        }
    }

    public sealed class CryptographicInventoryDto : ErrorIdentifiableDtoBase, IModelDto,ITrackableEntity<CryptographicInventoryDto>,ISnapshotable<CryptographicInventoryDto>,IChangeObservable
    {
        public Guid  CryptographicInventoryId { get;  set; }

        public Guid TenantId { get;  set; }

        public string KeyName { get;  set; }

        public string KeyType { get;  set; }

        public string Algorithm { get;  set; }

        public int KeyLength { get;  set; }

        public string KeyLocation { get;  set; }

        public DateTime CreationDate { get;  set; }

        public DateTime? LastRotationDate { get;  set; }

        public DateTime NextRotationDue { get;  set; }

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
        public CryptographicInventoryDto Clone()
        {
            return new CryptographicInventoryDto
            {
                CryptographicInventoryId = this.CryptographicInventoryId,
                TenantId = this.TenantId,
                KeyName = this.KeyName,
                KeyType = this.KeyType,
                Algorithm = this.Algorithm,
                KeyLength = this.KeyLength,
                KeyLocation = this.KeyLocation,
                CreationDate = this.CreationDate,
                LastRotationDate = this.LastRotationDate,
                NextRotationDue = this.NextRotationDue,
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
        public CryptographicInventoryDto() { }
        public CryptographicInventoryDto(Guid cryptographicInventoryId, Guid merchantId, Guid tenantId, string keyName, string keyType, string algorithm, int keyLength, string keyLocation, DateTime creationDate, DateTime nextRotationDue, DateTime createdAt, Guid createdBy, bool isDeleted)
        {
                AuditEntityId = cryptographicInventoryId.ToString();
                AuditId = Guid.CreateVersion7().ToString();
                AuditEntityType = GetType().Name;
                this.CryptographicInventoryId = Guard.Against.Default(cryptographicInventoryId, nameof(cryptographicInventoryId));
                this.MerchantId = Guard.Against.Default(merchantId, nameof(merchantId));
                this.TenantId = Guard.Against.Default(tenantId, nameof(tenantId));
                this.KeyName = Guard.Against.NullOrEmpty(keyName, nameof(keyName));
                this.KeyType = Guard.Against.NullOrEmpty(keyType, nameof(keyType));
                this.Algorithm = Guard.Against.NullOrEmpty(algorithm, nameof(algorithm));
                this.KeyLength = Guard.Against.Negative(keyLength, nameof(keyLength));
                this.KeyLocation = Guard.Against.NullOrEmpty(keyLocation, nameof(keyLocation));
                this.CreationDate = Guard.Against.OutOfSQLDateRange(creationDate, nameof(creationDate));
                this.NextRotationDue = Guard.Against.OutOfSQLDateRange(nextRotationDue, nameof(nextRotationDue));
                this.CreatedAt = Guard.Against.OutOfSQLDateRange(createdAt, nameof(createdAt));
                this.CreatedBy = Guard.Against.Default(createdBy, nameof(createdBy));
                this.IsDeleted = Guard.Against.Null(isDeleted, nameof(isDeleted));
        }
        [JsonIgnore]
        private Option<CryptographicInventoryDto> _snapshot = None;

        public Option<CryptographicInventoryDto> CurrentSnapshot => _snapshot;

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

        private void CopyFrom(CryptographicInventoryDto source)
        {
        CryptographicInventoryId = source.CryptographicInventoryId;
        TenantId = source.TenantId;
        KeyName = source.KeyName;
        KeyType = source.KeyType;
        Algorithm = source.Algorithm;
        KeyLength = source.KeyLength;
        KeyLocation = source.KeyLocation;
        CreationDate = source.CreationDate;
        LastRotationDate = source.LastRotationDate;
        NextRotationDue = source.NextRotationDue;
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
        public Either<Error, bool> Compare(CryptographicInventoryDto other)
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

        private Either<Error, bool> CompareBasicFields(CryptographicInventoryDto other)
        {
            bool same =
                (CryptographicInventoryId == other.CryptographicInventoryId) &&
                (TenantId == other.TenantId) &&
                (MerchantId == other.MerchantId) &&
                StringComparer.OrdinalIgnoreCase.Equals(KeyName, other.KeyName) &&
                StringComparer.OrdinalIgnoreCase.Equals(KeyType, other.KeyType) &&
                StringComparer.OrdinalIgnoreCase.Equals(Algorithm, other.Algorithm) &&
                (KeyLength == other.KeyLength) &&
                StringComparer.OrdinalIgnoreCase.Equals(KeyLocation, other.KeyLocation) &&
                (CreationDate == other.CreationDate) &&
                (LastRotationDate == other.LastRotationDate) &&
                (NextRotationDue == other.NextRotationDue) &&
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