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
    public static class ServiceProviderFactory
    {
        public static ServiceProviderDto CreateNewFromTemplate(ServiceProviderDto template)
        {
            var now = DateTime.UtcNow;

            var newServiceProvider = new ServiceProviderDto
            {
                ServiceProviderId = Guid.Empty,
                CreatedBy = template.CreatedBy,
                TenantId =  Guid.Empty,
                IsPCICompliant = false,
                IsDeleted = false,
            };

            return newServiceProvider;
        }

        public static ServiceProviderDto CreateNewEmpty()
        {
            var now = DateTime.UtcNow;
            return new ServiceProviderDto
            {
                ServiceProviderId = Guid.Empty,
                TenantId = Guid.Empty,
                MerchantId = Guid.Empty,
                CreatedBy = Guid.Empty,
                UpdatedBy = Guid.Empty,
                IsDeleted = false,
            };
        }
    }
    public static class ServiceProviderExtensions
    {
        public static ServiceProviderDto CloneAsNew(this ServiceProviderDto template)
        {
            return ServiceProviderFactory.CreateNewFromTemplate(template);
        }
    }

    public sealed class ServiceProviderDto : ErrorIdentifiableDtoBase, IModelDto,ITrackableEntity<ServiceProviderDto>,ISnapshotable<ServiceProviderDto>,IChangeObservable
    {
        public Guid  ServiceProviderId { get;  set; }

        public Guid TenantId { get;  set; }

        public string ProviderName { get;  set; }

        public string ServiceType { get;  set; }

        public bool IsPCICompliant { get;  set; }

        public DateTime? AOCExpiryDate { get;  set; }

        public string? ResponsibilityMatrix { get;  set; }

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
        public ServiceProviderDto Clone()
        {
            return new ServiceProviderDto
            {
                ServiceProviderId = this.ServiceProviderId,
                TenantId = this.TenantId,
                ProviderName = this.ProviderName,
                ServiceType = this.ServiceType,
                IsPCICompliant = this.IsPCICompliant,
                AOCExpiryDate = this.AOCExpiryDate,
                ResponsibilityMatrix = this.ResponsibilityMatrix,
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
        public ServiceProviderDto() { }
        public ServiceProviderDto(Guid serviceProviderId, Guid merchantId, Guid tenantId, string providerName, string serviceType, bool isPcicompliant, DateTime createdAt, Guid createdBy, bool isDeleted)
        {
                AuditEntityId = serviceProviderId.ToString();
                AuditId = Guid.CreateVersion7().ToString();
                AuditEntityType = GetType().Name;
                this.ServiceProviderId = Guard.Against.Default(serviceProviderId, nameof(serviceProviderId));
                this.MerchantId = Guard.Against.Default(merchantId, nameof(merchantId));
                this.TenantId = Guard.Against.Default(tenantId, nameof(tenantId));
                this.ProviderName = Guard.Against.NullOrEmpty(providerName, nameof(providerName));
                this.ServiceType = Guard.Against.NullOrEmpty(serviceType, nameof(serviceType));
                this.IsPCICompliant = Guard.Against.Null(isPcicompliant, nameof(isPcicompliant));
                this.CreatedAt = Guard.Against.OutOfSQLDateRange(createdAt, nameof(createdAt));
                this.CreatedBy = Guard.Against.Default(createdBy, nameof(createdBy));
                this.IsDeleted = Guard.Against.Null(isDeleted, nameof(isDeleted));
        }
        [JsonIgnore]
        private Option<ServiceProviderDto> _snapshot = None;

        public Option<ServiceProviderDto> CurrentSnapshot => _snapshot;

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

        private void CopyFrom(ServiceProviderDto source)
        {
        ServiceProviderId = source.ServiceProviderId;
        TenantId = source.TenantId;
        ProviderName = source.ProviderName;
        ServiceType = source.ServiceType;
        IsPCICompliant = source.IsPCICompliant;
        AOCExpiryDate = source.AOCExpiryDate;
        ResponsibilityMatrix = source.ResponsibilityMatrix;
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
        public Either<Error, bool> Compare(ServiceProviderDto other)
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

        private Either<Error, bool> CompareBasicFields(ServiceProviderDto other)
        {
            bool same =
                (ServiceProviderId == other.ServiceProviderId) &&
                (TenantId == other.TenantId) &&
                (MerchantId == other.MerchantId) &&
                StringComparer.OrdinalIgnoreCase.Equals(ProviderName, other.ProviderName) &&
                StringComparer.OrdinalIgnoreCase.Equals(ServiceType, other.ServiceType) &&
                (IsPCICompliant == other.IsPCICompliant) &&
                (AOCExpiryDate == other.AOCExpiryDate) &&
                StringComparer.OrdinalIgnoreCase.Equals(ResponsibilityMatrix, other.ResponsibilityMatrix) &&
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