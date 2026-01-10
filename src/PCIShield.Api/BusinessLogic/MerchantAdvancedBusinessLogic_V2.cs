using System;
using System.Collections.Generic;
using System.Linq;
using System.Reactive.Linq;
using System.Threading.Tasks;
using Ardalis.GuardClauses;
using PCIShield.Domain.Entities;
using PCIShield.Infrastructure.Services;
using LanguageExt;
using LanguageExt.Common;
using static LanguageExt.Prelude;
using Unit = LanguageExt.Unit;
using PCIShield.Domain.ModelEntityDto;
using PCIShield.Domain.ModelsDto;

namespace PCIShield.Api.BusinessLogic
{
    public class MerchantAdvancedBusinessLogicService
    {
        private readonly MerchantDto _merchant;
        private readonly IAppLoggerService<MerchantAdvancedBusinessLogicService> _logger;

        public MerchantAdvancedBusinessLogicService(
            MerchantDto merchant,
            IAppLoggerService<MerchantAdvancedBusinessLogicService> logger)
        {
            _merchant = Guard.Against.Null(merchant, nameof(merchant));
            _logger = Guard.Against.Null(logger, nameof(logger));
        }

        public string DisplayIdentifier => $"{_merchant.MerchantCode} - {_merchant.MerchantName}";

        public TimeSpan AgeOfRecord => DateTime.UtcNow - _merchant.CreatedAt;
        public TimeSpan TimeSinceLastUpdate => _merchant.UpdatedAt.HasValue ? DateTime.UtcNow - _merchant.UpdatedAt.Value : TimeSpan.Zero;
        public bool IsRecentlyModified(int hours = 24) => TimeSinceLastUpdate.TotalHours < hours;

        public Either<string, Unit> ValidateTenantScope(Guid currentTenantId) =>
            _merchant.TenantId == currentTenantId
                ? Right(unit)
                : Left("Tenant mismatch: unauthorized access");
        public int CountAssessments => _merchant.Assessments?.Count(x => !x.IsDeleted) ?? 0;
        public bool HasAssessments => CountAssessments > 0;
        public int CountAssets => _merchant.Assets?.Count(x => !x.IsDeleted) ?? 0;
        public bool HasAssets => CountAssets > 0;
        public int CountCompensatingControls => _merchant.CompensatingControls?.Count(x => !x.IsDeleted) ?? 0;
        public bool HasCompensatingControls => CountCompensatingControls > 0;
        public int CountComplianceOfficers => _merchant.ComplianceOfficers?.Count(x => !x.IsDeleted) ?? 0;
        public bool HasComplianceOfficers => CountComplianceOfficers > 0;
        public int CountCryptographicInventories => _merchant.CryptographicInventories?.Count(x => !x.IsDeleted) ?? 0;
        public bool HasCryptographicInventories => CountCryptographicInventories > 0;
        public decimal TotalCryptographicInventoryKeyLength => _merchant.CryptographicInventories?.Where(x => !x.IsDeleted).Sum(x => x.KeyLength) ?? 0;
        public int CountEvidences => _merchant.Evidences?.Count(x => !x.IsDeleted) ?? 0;
        public bool HasEvidences => CountEvidences > 0;
        public int CountNetworkSegmentations => _merchant.NetworkSegmentations?.Count(x => !x.IsDeleted) ?? 0;
        public bool HasNetworkSegmentations => CountNetworkSegmentations > 0;
        public int CountPaymentChannels => _merchant.PaymentChannels?.Count(x => !x.IsDeleted) ?? 0;
        public bool HasPaymentChannels => CountPaymentChannels > 0;
        public decimal TotalPaymentChannelProcessingVolume => _merchant.PaymentChannels?.Where(x => !x.IsDeleted).Sum(x => x.ProcessingVolume) ?? 0;
        public int CountServiceProviders => _merchant.ServiceProviders?.Count(x => !x.IsDeleted) ?? 0;
        public bool HasServiceProviders => CountServiceProviders > 0;
        public IEnumerable<AssessmentControlDto> GetRelatedAssessmentControlsViaPath() =>
            _merchant.Assessments?
                .SelectMany(x => x.AssessmentControls)
                .Distinct() ?? Enumerable.Empty<AssessmentControlDto>();
        public IEnumerable<ControlEvidenceDto> GetRelatedControlEvidencesViaPath() =>
            _merchant.Assessments?
                .SelectMany(x => x.ControlEvidences)
                .Distinct() ?? Enumerable.Empty<ControlEvidenceDto>();
        public IEnumerable<ROCPackageDto> GetRelatedROCPackagesViaPath() =>
            _merchant.Assessments?
                .SelectMany(x => x.ROCPackages)
                .Distinct() ?? Enumerable.Empty<ROCPackageDto>();
        public IEnumerable<AssetControlDto> GetRelatedAssetControlsViaPath() =>
            _merchant.Assets?
                .SelectMany(x => x.AssetControls)
                .Distinct() ?? Enumerable.Empty<AssetControlDto>();
        public IEnumerable<ScanScheduleDto> GetRelatedScanSchedulesViaPath() =>
            _merchant.Assets?
                .SelectMany(x => x.ScanSchedules)
                .Distinct() ?? Enumerable.Empty<ScanScheduleDto>();

        public decimal GetAnnualCardVolumeWithMarkup(decimal markupPercent) => _merchant.AnnualCardVolume * (1 + markupPercent / 100);

        public int DaysSinceLastAssessmentDate => _merchant.LastAssessmentDate.HasValue ? (DateTime.UtcNow - _merchant.LastAssessmentDate.Value).Days : 0;
        public int DaysSinceUpdatedAt => _merchant.UpdatedAt.HasValue ? (DateTime.UtcNow - _merchant.UpdatedAt.Value).Days : 0;
        public AuditInfo GetAuditTrail() => new(
            _merchant.CreatedBy,
            _merchant.UpdatedBy,
            _merchant.CreatedAt,
            _merchant.UpdatedAt);

        private IEnumerable<T> FilterDeleted<T>(IEnumerable<T> collection) where T : class
        {
            var isDeletedProp = typeof(T).GetProperty("IsDeleted");
            return isDeletedProp != null
                ? collection.Where(x => !(bool)isDeletedProp.GetValue(x))
                : collection;
        }

        public record AuditInfo(Guid CreatedBy, Guid? UpdatedBy, DateTime CreatedAt, DateTime? UpdatedAt);
        public record StatusSummary(bool IsActive, bool IsDeleted, bool IsValid);
        public record AggregationResult(int Count, decimal Total, decimal Average, Option<DateTime> LastActivity);
    }
}