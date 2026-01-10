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
    public static class EvidenceFactory
    {
        public static EvidenceDto CreateNewFromTemplate(EvidenceDto template)
        {
            var now = DateTime.UtcNow;

            var newEvidence = new EvidenceDto
            {
                EvidenceId = Guid.Empty,
                CreatedBy = template.CreatedBy,
                TenantId =  Guid.Empty,
                IsValid = false,
                IsDeleted = false,
                ControlEvidences = new(),
            };

            return newEvidence;
        }

        public static EvidenceDto CreateNewEmpty()
        {
            var now = DateTime.UtcNow;
            return new EvidenceDto
            {
                EvidenceId = Guid.Empty,
                TenantId = Guid.Empty,
                MerchantId = Guid.Empty,
                CreatedBy = Guid.Empty,
                UpdatedBy = Guid.Empty,
                IsDeleted = false,
            };
        }
    }
    public static class EvidenceExtensions
    {
        public static EvidenceDto CloneAsNew(this EvidenceDto template)
        {
            return EvidenceFactory.CreateNewFromTemplate(template);
        }
    }

    public sealed class EvidenceDto : ErrorIdentifiableDtoBase, IModelDto,ITrackableEntity<EvidenceDto>,ISnapshotable<EvidenceDto>,IChangeObservable
    {
        public Guid  EvidenceId { get;  set; }

        public Guid TenantId { get;  set; }

        public string EvidenceCode { get;  set; }

        public string EvidenceTitle { get;  set; }

        public int EvidenceType { get;  set; }

        public DateTime CollectedDate { get;  set; }

        public string? FileHash { get;  set; }

        public string? StorageUri { get;  set; }

        public bool IsValid { get;  set; }

        public DateTime CreatedAt { get;  set; }

        public Guid CreatedBy { get;  set; }

        public DateTime? UpdatedAt { get;  set; }

        public Guid? UpdatedBy { get;  set; }

        public bool IsDeleted { get;  set; }
        public int ActiveControlEvidenceCount { get; set; }
        public int DaysSinceCollectedDate { get; set; }
        public bool IsEvidenceTypeCritical { get; set; }
        public bool IsValidFlag { get; set; }
        public DateTime? LatestControlEvidenceUpdatedAt { get; set; }
        public int TotalControlEvidenceCount { get; set; }
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
        public EvidenceDto Clone()
        {
            return new EvidenceDto
            {
                EvidenceId = this.EvidenceId,
                TenantId = this.TenantId,
                EvidenceCode = this.EvidenceCode,
                EvidenceTitle = this.EvidenceTitle,
                EvidenceType = this.EvidenceType,
                CollectedDate = this.CollectedDate,
                FileHash = this.FileHash,
                StorageUri = this.StorageUri,
                IsValid = this.IsValid,
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
        public List<ControlEvidenceDto> ControlEvidences { get; set; } = new();
        public EvidenceDto() { }
        public EvidenceDto(Guid evidenceId, Guid merchantId, Guid tenantId, string evidenceCode, string evidenceTitle, int evidenceType, DateTime collectedDate, bool isValid, DateTime createdAt, Guid createdBy, bool isDeleted)
        {
                AuditEntityId = evidenceId.ToString();
                AuditId = Guid.CreateVersion7().ToString();
                AuditEntityType = GetType().Name;
                this.EvidenceId = Guard.Against.Default(evidenceId, nameof(evidenceId));
                this.MerchantId = Guard.Against.Default(merchantId, nameof(merchantId));
                this.TenantId = Guard.Against.Default(tenantId, nameof(tenantId));
                this.EvidenceCode = Guard.Against.NullOrEmpty(evidenceCode, nameof(evidenceCode));
                this.EvidenceTitle = Guard.Against.NullOrEmpty(evidenceTitle, nameof(evidenceTitle));
                this.EvidenceType = Guard.Against.Negative(evidenceType, nameof(evidenceType));
                this.CollectedDate = Guard.Against.OutOfSQLDateRange(collectedDate, nameof(collectedDate));
                this.IsValid = Guard.Against.Null(isValid, nameof(isValid));
                this.CreatedAt = Guard.Against.OutOfSQLDateRange(createdAt, nameof(createdAt));
                this.CreatedBy = Guard.Against.Default(createdBy, nameof(createdBy));
                this.IsDeleted = Guard.Against.Null(isDeleted, nameof(isDeleted));

         this.ControlEvidences = new();
        }
        [JsonIgnore]
        private Option<EvidenceDto> _snapshot = None;

        public Option<EvidenceDto> CurrentSnapshot => _snapshot;

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

        private void CopyFrom(EvidenceDto source)
        {
        EvidenceId = source.EvidenceId;
        TenantId = source.TenantId;
        EvidenceCode = source.EvidenceCode;
        EvidenceTitle = source.EvidenceTitle;
        EvidenceType = source.EvidenceType;
        CollectedDate = source.CollectedDate;
        FileHash = source.FileHash;
        StorageUri = source.StorageUri;
        IsValid = source.IsValid;
        CreatedAt = source.CreatedAt;
        CreatedBy = source.CreatedBy;
        UpdatedAt = source.UpdatedAt;
        UpdatedBy = source.UpdatedBy;
        IsDeleted = source.IsDeleted;
        MerchantId = source.MerchantId ;
            Merchant = source.Merchant?.Clone();
            ControlEvidences = new List<ControlEvidenceDto>(source.ControlEvidences);
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
        public Either<Error, bool> Compare(EvidenceDto other)
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

        private Either<Error, bool> CompareBasicFields(EvidenceDto other)
        {
            bool same =
                (EvidenceId == other.EvidenceId) &&
                (TenantId == other.TenantId) &&
                (MerchantId == other.MerchantId) &&
                StringComparer.OrdinalIgnoreCase.Equals(EvidenceCode, other.EvidenceCode) &&
                StringComparer.OrdinalIgnoreCase.Equals(EvidenceTitle, other.EvidenceTitle) &&
                (EvidenceType == other.EvidenceType) &&
                (CollectedDate == other.CollectedDate) &&
                StringComparer.OrdinalIgnoreCase.Equals(FileHash, other.FileHash) &&
                StringComparer.OrdinalIgnoreCase.Equals(StorageUri, other.StorageUri) &&
                (IsValid == other.IsValid) &&
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