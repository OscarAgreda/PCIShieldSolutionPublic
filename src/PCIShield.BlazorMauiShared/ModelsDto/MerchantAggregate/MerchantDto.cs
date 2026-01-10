using System.ComponentModel.DataAnnotations.Schema;
using System.Reactive.Linq;
using System.Reactive.Subjects;
using System.Text.Json;
using System.Text.Json.Serialization;

using Ardalis.GuardClauses;

using LanguageExt;
using LanguageExt.Common;

using PCIShieldLib.SharedKernel.Interfaces;

using static LanguageExt.Prelude;

using Unit = LanguageExt.Unit;

namespace PCIShield.Domain.ModelsDto
{
    public static class MerchantFactory
    {
        public static MerchantDto CreateNewFromTemplate(MerchantDto template)
        {
            var now = DateTime.UtcNow;

            var newMerchant = new MerchantDto
            {
                MerchantId = Guid.Empty,
                CreatedBy = template.CreatedBy,
                TenantId = Guid.Empty,
                AnnualCardVolume = 0,
                IsDeleted = false,
                Assessments = new(),
                Assets = new(),
                CompensatingControls = new(),
                ComplianceOfficers = new(),
                CryptographicInventories = new(),
                Evidences = new(),
                NetworkSegmentations = new(),
                PaymentChannels = new(),
                ServiceProviders = new(),
            };

            return newMerchant;
        }

        public static MerchantDto CreateNewEmpty()
        {
            var now = DateTime.UtcNow;
            return new MerchantDto
            {
                MerchantId = Guid.Empty,
                TenantId = Guid.Empty,
                CreatedBy = Guid.Empty,
                UpdatedBy = Guid.Empty,
                IsDeleted = false,
            };
        }
    }
    public static class MerchantExtensions
    {
        public static MerchantDto CloneAsNew(this MerchantDto template)
        {
            return MerchantFactory.CreateNewFromTemplate(template);
        }
    }

    public sealed class MerchantDto : ErrorIdentifiableDtoBase, IModelDto, ITrackableEntity<MerchantDto>, ISnapshotable<MerchantDto>, IChangeObservable
    {
        public Guid MerchantId { get; set; }

        public Guid TenantId { get; set; }

        public string MerchantCode { get; set; }

        public string MerchantName { get; set; }

        public int MerchantLevel { get; set; }

        public string AcquirerName { get; set; }

        public string ProcessorMID { get; set; }

        public decimal AnnualCardVolume { get; set; }

        public DateTime? LastAssessmentDate { get; set; }

        public DateTime NextAssessmentDue { get; set; }

        public int ComplianceRank { get; set; }

        public DateTime CreatedAt { get; set; }

        public Guid CreatedBy { get; set; }

        public DateTime? UpdatedAt { get; set; }

        public Guid? UpdatedBy { get; set; }

        public bool IsDeleted { get; set; }
        public int ActiveAssessmentCount { get; set; }

        public int ActiveAssetCount { get; set; }
        public int ActiveCompensatingControlCount { get; set; }
        public int ActiveComplianceOfficerCount { get; set; }
        public int ActiveCryptographicInventoryCount { get; set; }
        public int ActiveEvidenceCount { get; set; }
        public int ActiveNetworkSegmentationCount { get; set; }
        public int ActivePaymentChannelCount { get; set; }
        public int ActiveServiceProviderCount { get; set; }
        public int AffectedEntitiesCount { get; set; }
        public int ArticulationRank { get; set; }
        public decimal AssessmentComplianceScore { get; set; }
        public bool AssessmentInfoExceedsRiskThreshold { get; set; }
        public double AuthorityScore { get; set; }
        public decimal AverageAssessmentComplianceScore { get; set; }
        public int AverageControlFrequencyDays { get; set; }
        public int AverageVulnerabilitySeverity { get; set; }
        public decimal CachedAssessmentAssessmentTypeSum { get; set; }
        public int CachedAssessmentCount { get; set; }
        public decimal CachedAssetAssetTypeSum { get; set; }
        public int CachedAssetCount { get; set; }
        public int CachedCompensatingControlCount { get; set; }
        public decimal CachedCompensatingControlRankSum { get; set; }
        public int CachedCryptographicInventoryCount { get; set; }
        public decimal CachedCryptographicInventoryKeyLengthSum { get; set; }
        public int CachedEvidenceCount { get; set; }
        public decimal CachedEvidenceEvidenceTypeSum { get; set; }
        public int CascadeDepth { get; set; }
        public double CascadeImpactScore { get; set; }
        public int ControlFrequencyDays { get; set; }
        public int CriticalAssessmentCount { get; set; }
        public int CriticalAssetCount { get; set; }
        public int CriticalCompensatingControlCount { get; set; }
        public int CriticalEvidenceCount { get; set; }
        public int CriticalityRank { get; set; }
        public int CriticalPaymentChannelCount { get; set; }
        public int CryptographicInventoryKeyLength { get; set; }
        public double DataStabilityScore { get; set; }
        public int DaysSinceLastAssessment { get; set; }
        public int DaysSinceLastAssessmentDate { get; set; }
        public double DeletionImpactScore { get; set; }
        public int DrillDownPathCount { get; set; }
        public double EntityInfluenceRank { get; set; }
        public double EntityInfluenceScore { get; set; }
        public double EvolutionPressureScore { get; set; }
        public bool HasAssessmentComplianceScoreBelowThreshold { get; set; }
        public bool HasComplexDrillDowns { get; set; }
        public bool HasControlFrequencyDaysOverdue { get; set; }
        public bool HasRepeatingChildPatternBundle { get; set; }
        public bool HasUpdatedBy { get; set; }
        public double HubScore { get; set; }
        public bool IsArticulationPoint { get; set; }
        public bool IsDeletedFlag { get; set; }
        public bool IsHighComplexityEntity { get; set; }
        public bool IsHubNode { get; set; }
        public bool IsMerchantLevelCritical { get; set; }
        public bool IsSystemCriticalNode { get; set; }
        public bool IsVolatileEntity { get; set; }
        public DateTime? LatestAssessmentStartDate { get; set; }
        public DateTime? LatestAssetLastScanDate { get; set; }
        public DateTime? LatestCompensatingControlApprovalDate { get; set; }
        public DateTime? LatestComplianceOfficerUpdatedAt { get; set; }
        public DateTime? LatestCryptographicInventoryCreationDate { get; set; }
        public DateTime? LatestEvidenceCollectedDate { get; set; }
        public DateTime? LatestNetworkSegmentationLastValidated { get; set; }
        public DateTime? LatestPaymentChannelUpdatedAt { get; set; }
        public DateTime? LatestServiceProviderAOCExpiryDate { get; set; }
        public int MaxCryptographicInventoryKeyLength { get; set; }
        public decimal MaxVulnerabilityCVSS { get; set; }
        public double NetworkCentrality { get; set; }
        public int NodeDegree { get; set; }
        public decimal PaymentChannelProcessingVolume { get; set; }
        public int RelationshipCommunities { get; set; }
        public double RepeatingChildPatternCompleteness { get; set; }
        public bool RequiresDeletionPreview { get; set; }
        public bool RequiresSafetyChecks { get; set; }
        public int TotalAssessmentCount { get; set; }
        public int TotalAssetCount { get; set; }
        public int TotalCompensatingControlCount { get; set; }
        public int TotalComplianceOfficerCount { get; set; }
        public int TotalCryptographicInventoryCount { get; set; }
        public int TotalEvidenceCount { get; set; }
        public int TotalNetworkSegmentationCount { get; set; }
        public int TotalPaymentChannelCount { get; set; }
        public decimal TotalPaymentChannelProcessingVolume { get; set; }
        public int TotalServiceProviderCount { get; set; }
        public decimal VulnerabilityCVSS { get; set; }
        public int VulnerabilitySeverity { get; set; }
        public bool VulnerabilityInfoExceedsRiskThreshold { get; set; }
        public AssessmentControlDto? AssessmentControl { get; set; }
        public List<AssetControlDto> AssetControls { get; set; } = new();
        public List<ControlDto> Controls { get; set; } = new();
        public ControlEvidenceDto? ControlEvidence { get; set; }
        public PaymentPageDto? PaymentPage { get; set; }
        public List<ROCPackageDto> ROCPackages { get; set; } = new();
        public List<ScanScheduleDto> ScanSchedules { get; set; } = new();
        public List<VulnerabilityDto> Vulnerabilities { get; set; } = new();

        public Guid ControlId { get; set; }
        public ControlDto? Control { get; set; }
        public MerchantDto Clone()
        {
            return new MerchantDto
            {
                MerchantId = this.MerchantId,
                TenantId = this.TenantId,
                MerchantCode = this.MerchantCode,
                MerchantName = this.MerchantName,
                MerchantLevel = this.MerchantLevel,
                AcquirerName = this.AcquirerName,
                ProcessorMID = this.ProcessorMID,
                AnnualCardVolume = this.AnnualCardVolume,
                LastAssessmentDate = this.LastAssessmentDate,
                NextAssessmentDue = this.NextAssessmentDue,
                ComplianceRank = this.ComplianceRank,
                CreatedAt = this.CreatedAt,
                CreatedBy = this.CreatedBy,
                UpdatedAt = this.UpdatedAt,
                UpdatedBy = this.UpdatedBy,
                IsDeleted = this.IsDeleted,
            };
        }
        public List<AssessmentDto> Assessments { get; set; } = new();

        public List<AssetDto> Assets { get; set; } = new();
        public List<CompensatingControlDto> CompensatingControls { get; set; } = new();
        public List<ComplianceOfficerDto> ComplianceOfficers { get; set; } = new();
        public List<CryptographicInventoryDto> CryptographicInventories { get; set; } = new();
        public List<EvidenceDto> Evidences { get; set; } = new();
        public List<NetworkSegmentationDto> NetworkSegmentations { get; set; } = new();
        public List<PaymentChannelDto> PaymentChannels { get; set; } = new();
        public List<ServiceProviderDto> ServiceProviders { get; set; } = new();
        public MerchantDto()
        { }
        public MerchantDto(Guid merchantId, Guid tenantId, string merchantCode, string merchantName, int merchantLevel, string acquirerName, string processorMID, decimal annualCardVolume, DateTime nextAssessmentDue, int complianceRank, DateTime createdAt, Guid createdBy, bool isDeleted)
        {
            AuditEntityId = merchantId.ToString();
            AuditId = Guid.CreateVersion7().ToString();
            AuditEntityType = GetType().Name;
            this.MerchantId = Guard.Against.Default(merchantId, nameof(merchantId));
            this.TenantId = Guard.Against.Default(tenantId, nameof(tenantId));
            this.MerchantCode = Guard.Against.NullOrEmpty(merchantCode, nameof(merchantCode));
            this.MerchantName = Guard.Against.NullOrEmpty(merchantName, nameof(merchantName));
            this.MerchantLevel = Guard.Against.Negative(merchantLevel, nameof(merchantLevel));
            this.AcquirerName = Guard.Against.NullOrEmpty(acquirerName, nameof(acquirerName));
            this.ProcessorMID = Guard.Against.NullOrEmpty(processorMID, nameof(processorMID));
            this.AnnualCardVolume = Guard.Against.Negative(annualCardVolume, nameof(annualCardVolume));
            this.NextAssessmentDue = Guard.Against.OutOfSQLDateRange(nextAssessmentDue, nameof(nextAssessmentDue));
            this.ComplianceRank = Guard.Against.Negative(complianceRank, nameof(complianceRank));
            this.CreatedAt = Guard.Against.OutOfSQLDateRange(createdAt, nameof(createdAt));
            this.CreatedBy = Guard.Against.Default(createdBy, nameof(createdBy));
            this.IsDeleted = Guard.Against.Null(isDeleted, nameof(isDeleted));

            this.Assessments = new();

            this.Assets = new();

            this.CompensatingControls = new();

            this.ComplianceOfficers = new();

            this.CryptographicInventories = new();

            this.Evidences = new();

            this.NetworkSegmentations = new();

            this.PaymentChannels = new();

            this.ServiceProviders = new();
        }
        [JsonIgnore]
        private Option<MerchantDto> _snapshot = None;

        public Option<MerchantDto> CurrentSnapshot => _snapshot;

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

        private void CopyFrom(MerchantDto source)
        {
            MerchantId = source.MerchantId;
            TenantId = source.TenantId;
            MerchantCode = source.MerchantCode;
            MerchantName = source.MerchantName;
            MerchantLevel = source.MerchantLevel;
            AcquirerName = source.AcquirerName;
            ProcessorMID = source.ProcessorMID;
            AnnualCardVolume = source.AnnualCardVolume;
            LastAssessmentDate = source.LastAssessmentDate;
            NextAssessmentDue = source.NextAssessmentDue;
            ComplianceRank = source.ComplianceRank;
            CreatedAt = source.CreatedAt;
            CreatedBy = source.CreatedBy;
            UpdatedAt = source.UpdatedAt;
            UpdatedBy = source.UpdatedBy;
            IsDeleted = source.IsDeleted;
            Assessments = new List<AssessmentDto>(source.Assessments);
            Assets = new List<AssetDto>(source.Assets);
            CompensatingControls = new List<CompensatingControlDto>(source.CompensatingControls);
            ComplianceOfficers = new List<ComplianceOfficerDto>(source.ComplianceOfficers);
            CryptographicInventories = new List<CryptographicInventoryDto>(source.CryptographicInventories);
            Evidences = new List<EvidenceDto>(source.Evidences);
            NetworkSegmentations = new List<NetworkSegmentationDto>(source.NetworkSegmentations);
            PaymentChannels = new List<PaymentChannelDto>(source.PaymentChannels);
            ServiceProviders = new List<ServiceProviderDto>(source.ServiceProviders);
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
        public Either<Error, bool> Compare(MerchantDto other)
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

        private Either<Error, bool> CompareBasicFields(MerchantDto other)
        {
            bool same =
                (MerchantId == other.MerchantId) &&
                (TenantId == other.TenantId) &&
                StringComparer.OrdinalIgnoreCase.Equals(MerchantCode, other.MerchantCode) &&
                StringComparer.OrdinalIgnoreCase.Equals(MerchantName, other.MerchantName) &&
                (MerchantLevel == other.MerchantLevel) &&
                StringComparer.OrdinalIgnoreCase.Equals(AcquirerName, other.AcquirerName) &&
                StringComparer.OrdinalIgnoreCase.Equals(ProcessorMID, other.ProcessorMID) &&
                (AnnualCardVolume == other.AnnualCardVolume) &&
                (LastAssessmentDate == other.LastAssessmentDate) &&
                (NextAssessmentDue == other.NextAssessmentDue) &&
                (ComplianceRank == other.ComplianceRank) &&
                (CreatedAt == other.CreatedAt) &&
                (CreatedBy == other.CreatedBy) &&
                (UpdatedAt == other.UpdatedAt) &&
                (UpdatedBy == other.UpdatedBy) &&
                (IsDeleted == other.IsDeleted);
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