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
    public static class ComplianceOfficerFactory
    {
        public static ComplianceOfficerDto CreateNewFromTemplate(ComplianceOfficerDto template)
        {
            var now = DateTime.UtcNow;

            var newComplianceOfficer = new ComplianceOfficerDto
            {
                ComplianceOfficerId = Guid.Empty,
                CreatedBy = template.CreatedBy,
                TenantId =  Guid.Empty,
                IsActive = false,
                IsDeleted = false,
            };

            return newComplianceOfficer;
        }

        public static ComplianceOfficerDto CreateNewEmpty()
        {
            var now = DateTime.UtcNow;
            return new ComplianceOfficerDto
            {
                ComplianceOfficerId = Guid.Empty,
                TenantId = Guid.Empty,
                MerchantId = Guid.Empty,
                CreatedBy = Guid.Empty,
                UpdatedBy = Guid.Empty,
                IsActive = true,
                IsDeleted = false,
            };
        }
    }
    public static class ComplianceOfficerExtensions
    {
        public static ComplianceOfficerDto CloneAsNew(this ComplianceOfficerDto template)
        {
            return ComplianceOfficerFactory.CreateNewFromTemplate(template);
        }
    }

    public sealed class ComplianceOfficerDto : ErrorIdentifiableDtoBase, IModelDto,ITrackableEntity<ComplianceOfficerDto>,ISnapshotable<ComplianceOfficerDto>,IChangeObservable
    {
        public Guid  ComplianceOfficerId { get;  set; }

        public Guid TenantId { get;  set; }

        public string OfficerCode { get;  set; }

        public string FirstName { get;  set; }

        public string LastName { get;  set; }

        public string Email { get;  set; }

        public string? Phone { get;  set; }

        public string? CertificationLevel { get;  set; }

        public bool IsActive { get;  set; }

        public DateTime CreatedAt { get;  set; }

        public Guid CreatedBy { get;  set; }

        public DateTime? UpdatedAt { get;  set; }

        public Guid? UpdatedBy { get;  set; }

        public bool IsDeleted { get;  set; }
        public int DaysSinceUpdatedAt { get; set; }
        public bool IsActiveFlag { get; set; }
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
        public ComplianceOfficerDto Clone()
        {
            return new ComplianceOfficerDto
            {
                ComplianceOfficerId = this.ComplianceOfficerId,
                TenantId = this.TenantId,
                OfficerCode = this.OfficerCode,
                FirstName = this.FirstName,
                LastName = this.LastName,
                Email = this.Email,
                Phone = this.Phone,
                CertificationLevel = this.CertificationLevel,
                IsActive = this.IsActive,
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
        public ComplianceOfficerDto() { }
        public ComplianceOfficerDto(Guid complianceOfficerId, Guid merchantId, Guid tenantId, string officerCode, string firstName, string lastName, string email, bool isActive, DateTime createdAt, Guid createdBy, bool isDeleted)
        {
                AuditEntityId = complianceOfficerId.ToString();
                AuditId = Guid.CreateVersion7().ToString();
                AuditEntityType = GetType().Name;
                this.ComplianceOfficerId = Guard.Against.Default(complianceOfficerId, nameof(complianceOfficerId));
                this.MerchantId = Guard.Against.Default(merchantId, nameof(merchantId));
                this.TenantId = Guard.Against.Default(tenantId, nameof(tenantId));
                this.OfficerCode = Guard.Against.NullOrEmpty(officerCode, nameof(officerCode));
                this.FirstName = Guard.Against.NullOrEmpty(firstName, nameof(firstName));
                this.LastName = Guard.Against.NullOrEmpty(lastName, nameof(lastName));
                this.Email = Guard.Against.NullOrEmpty(email, nameof(email));
                this.IsActive = Guard.Against.Null(isActive, nameof(isActive));
                this.CreatedAt = Guard.Against.OutOfSQLDateRange(createdAt, nameof(createdAt));
                this.CreatedBy = Guard.Against.Default(createdBy, nameof(createdBy));
                this.IsDeleted = Guard.Against.Null(isDeleted, nameof(isDeleted));
        }
        [JsonIgnore]
        private Option<ComplianceOfficerDto> _snapshot = None;

        public Option<ComplianceOfficerDto> CurrentSnapshot => _snapshot;

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

        private void CopyFrom(ComplianceOfficerDto source)
        {
        ComplianceOfficerId = source.ComplianceOfficerId;
        TenantId = source.TenantId;
        OfficerCode = source.OfficerCode;
        FirstName = source.FirstName;
        LastName = source.LastName;
        Email = source.Email;
        Phone = source.Phone;
        CertificationLevel = source.CertificationLevel;
        IsActive = source.IsActive;
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
        public Either<Error, bool> Compare(ComplianceOfficerDto other)
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

        private Either<Error, bool> CompareBasicFields(ComplianceOfficerDto other)
        {
            bool same =
                (ComplianceOfficerId == other.ComplianceOfficerId) &&
                (TenantId == other.TenantId) &&
                (MerchantId == other.MerchantId) &&
                StringComparer.OrdinalIgnoreCase.Equals(OfficerCode, other.OfficerCode) &&
                StringComparer.OrdinalIgnoreCase.Equals(FirstName, other.FirstName) &&
                StringComparer.OrdinalIgnoreCase.Equals(LastName, other.LastName) &&
                StringComparer.OrdinalIgnoreCase.Equals(Email, other.Email) &&
                StringComparer.OrdinalIgnoreCase.Equals(Phone, other.Phone) &&
                StringComparer.OrdinalIgnoreCase.Equals(CertificationLevel, other.CertificationLevel) &&
                (IsActive == other.IsActive) &&
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