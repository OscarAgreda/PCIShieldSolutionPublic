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
    public static class CompensatingControlFactory
    {
        public static CompensatingControlDto CreateNewFromTemplate(CompensatingControlDto template)
        {
            var now = DateTime.UtcNow;

            var newCompensatingControl = new CompensatingControlDto
            {
                CompensatingControlId = Guid.Empty,
                CreatedBy = template.CreatedBy,
                TenantId =  Guid.Empty,
                IsDeleted = false,
            };

            return newCompensatingControl;
        }

        public static CompensatingControlDto CreateNewEmpty()
        {
            var now = DateTime.UtcNow;
            return new CompensatingControlDto
            {
                CompensatingControlId = Guid.Empty,
                TenantId = Guid.Empty,
                ControlId = Guid.Empty,
                MerchantId = Guid.Empty,
                ApprovedBy = Guid.Empty,
                CreatedBy = Guid.Empty,
                UpdatedBy = Guid.Empty,
                IsDeleted = false,
            };
        }
    }
    public static class CompensatingControlExtensions
    {
        public static CompensatingControlDto CloneAsNew(this CompensatingControlDto template)
        {
            return CompensatingControlFactory.CreateNewFromTemplate(template);
        }
    }

    public sealed class CompensatingControlDto : ErrorIdentifiableDtoBase, IModelDto,ITrackableEntity<CompensatingControlDto>,ISnapshotable<CompensatingControlDto>,IChangeObservable
    {
        public Guid  CompensatingControlId { get;  set; }

        public Guid TenantId { get;  set; }

        public string Justification { get;  set; }

        public string ImplementationDetails { get;  set; }

        public Guid? ApprovedBy { get;  set; }

        public DateTime? ApprovalDate { get;  set; }

        public DateTime ExpiryDate { get;  set; }

        public int Rank { get;  set; }

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

        public ControlDto Control { get;  set; }

        public Guid ControlId { get;  set; }

        public MerchantDto Merchant { get;  set; }

        public Guid MerchantId { get;  set; }
        public CompensatingControlDto Clone()
        {
            return new CompensatingControlDto
            {
                CompensatingControlId = this.CompensatingControlId,
                TenantId = this.TenantId,
                Justification = this.Justification,
                ImplementationDetails = this.ImplementationDetails,
                ApprovedBy = this.ApprovedBy,
                ApprovalDate = this.ApprovalDate,
                ExpiryDate = this.ExpiryDate,
                Rank = this.Rank,
                CreatedAt = this.CreatedAt,
                CreatedBy = this.CreatedBy,
                UpdatedAt = this.UpdatedAt,
                UpdatedBy = this.UpdatedBy,
                IsDeleted = this.IsDeleted,
                ControlId = this.ControlId ,
                 Control = (this.Control!= null ? new  ControlDto
                {
                    ControlId = this.ControlId,
                    TenantId = this.Control.TenantId,
                    ControlCode = this.Control.ControlCode,
                    RequirementNumber = this.Control.RequirementNumber,
                    ControlTitle = this.Control.ControlTitle,
                    ControlDescription = this.Control.ControlDescription,
                    TestingGuidance = this.Control.TestingGuidance,
                    FrequencyDays = this.Control.FrequencyDays,
                    IsMandatory = this.Control.IsMandatory,
                    EffectiveDate = this.Control.EffectiveDate,
                    CreatedAt = this.Control.CreatedAt,
                    CreatedBy = this.Control.CreatedBy,
                    UpdatedAt = this.Control.UpdatedAt,
                    UpdatedBy = this.Control.UpdatedBy,
                    IsDeleted = this.Control.IsDeleted,
                }:null) ?? new ControlDto(),
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
        public CompensatingControlDto() { }
        public CompensatingControlDto(Guid compensatingControlId, Guid controlId, Guid merchantId, Guid tenantId, string justification, string implementationDetails, DateTime expiryDate, int rank, DateTime createdAt, Guid createdBy, bool isDeleted)
        {
                AuditEntityId = compensatingControlId.ToString();
                AuditId = Guid.CreateVersion7().ToString();
                AuditEntityType = GetType().Name;
                this.CompensatingControlId = Guard.Against.Default(compensatingControlId, nameof(compensatingControlId));
                this.ControlId = Guard.Against.Default(controlId, nameof(controlId));
                this.MerchantId = Guard.Against.Default(merchantId, nameof(merchantId));
                this.TenantId = Guard.Against.Default(tenantId, nameof(tenantId));
                this.Justification = Guard.Against.NullOrEmpty(justification, nameof(justification));
                this.ImplementationDetails = Guard.Against.NullOrEmpty(implementationDetails, nameof(implementationDetails));
                this.ExpiryDate = Guard.Against.OutOfSQLDateRange(expiryDate, nameof(expiryDate));
                this.Rank = Guard.Against.Negative(rank, nameof(rank));
                this.CreatedAt = Guard.Against.OutOfSQLDateRange(createdAt, nameof(createdAt));
                this.CreatedBy = Guard.Against.Default(createdBy, nameof(createdBy));
                this.IsDeleted = Guard.Against.Null(isDeleted, nameof(isDeleted));
        }
        [JsonIgnore]
        private Option<CompensatingControlDto> _snapshot = None;

        public Option<CompensatingControlDto> CurrentSnapshot => _snapshot;

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

        private void CopyFrom(CompensatingControlDto source)
        {
        CompensatingControlId = source.CompensatingControlId;
        TenantId = source.TenantId;
        Justification = source.Justification;
        ImplementationDetails = source.ImplementationDetails;
        ApprovedBy = source.ApprovedBy;
        ApprovalDate = source.ApprovalDate;
        ExpiryDate = source.ExpiryDate;
        Rank = source.Rank;
        CreatedAt = source.CreatedAt;
        CreatedBy = source.CreatedBy;
        UpdatedAt = source.UpdatedAt;
        UpdatedBy = source.UpdatedBy;
        IsDeleted = source.IsDeleted;
        ControlId = source.ControlId ;
            Control = source.Control?.Clone();
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
        public Either<Error, bool> Compare(CompensatingControlDto other)
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

        private Either<Error, bool> CompareBasicFields(CompensatingControlDto other)
        {
            bool same =
                (CompensatingControlId == other.CompensatingControlId) &&
                (TenantId == other.TenantId) &&
                (ControlId == other.ControlId) &&
                (MerchantId == other.MerchantId) &&
                StringComparer.OrdinalIgnoreCase.Equals(Justification, other.Justification) &&
                StringComparer.OrdinalIgnoreCase.Equals(ImplementationDetails, other.ImplementationDetails) &&
                (ApprovedBy == other.ApprovedBy) &&
                (ApprovalDate == other.ApprovalDate) &&
                (ExpiryDate == other.ExpiryDate) &&
                (Rank == other.Rank) &&
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