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
using PCIShield.Domain.ModelsDto;

namespace PCIShield.Api.BusinessLogic
{
    public class MerchantAdvancedBusinessLogicServiceV3
    {
        private readonly MerchantDto _merchant;
        private readonly IAppLoggerService<MerchantAdvancedBusinessLogicServiceV3> _logger;

        public MerchantAdvancedBusinessLogicServiceV3(
            MerchantDto merchant,
            IAppLoggerService<MerchantAdvancedBusinessLogicServiceV3> logger)
        {
            _merchant = Guard.Against.Null(merchant, nameof(merchant));
            _logger = Guard.Against.Null(logger, nameof(logger));
        }
        public decimal CalculateAnnualCardVolumeWithMarkup(decimal markupPercent) =>
            _merchant.AnnualCardVolume * (1 + markupPercent / 100);
        public int DaysSinceLastAssessmentDate =>
            _merchant.LastAssessmentDate.HasValue
                ? (DateTime.UtcNow - _merchant.LastAssessmentDate.Value).Days
                : 0;
        public int DaysSinceNextAssessmentDue => (DateTime.UtcNow - _merchant.NextAssessmentDue).Days;
        public int DaysSinceCreatedAt => (DateTime.UtcNow - _merchant.CreatedAt).Days;
        public bool IsMerchantLevelAboveThreshold(decimal threshold) => _merchant.MerchantLevel > threshold;
        public bool IsComplianceRankAboveThreshold(decimal threshold) => _merchant.ComplianceRank > threshold;
        public AuditInfo GetAuditTrail() => new(
            _merchant.CreatedBy,
            _merchant.UpdatedBy,
            _merchant.CreatedAt,
            _merchant.UpdatedAt);
        public int CountAssessments => _merchant.Assessments?.Where(x => !x.IsDeleted).Count() ?? 0;
        public bool HasAssessments => CountAssessments > 0;
        public decimal TotalAssessmentRank =>
            _merchant.Assessments?.Where(x => !x.IsDeleted).Sum(x => (decimal)x.Rank) ?? 0;
        public decimal AverageAssessmentRank =>
            _merchant.Assessments?.Where(x => !x.IsDeleted).Any() == true
                ? _merchant.Assessments.Where(x => !x.IsDeleted).Average(x => (decimal)x.Rank)
                : 0m;
        public decimal AverageAssessmentComplianceScore =>
            _merchant.Assessments?.Where(x => !x.IsDeleted).Any() == true
                ? _merchant.Assessments.Where(x => !x.IsDeleted).Average(x => (decimal)x.ComplianceScore)
                : 0m;
        public IEnumerable<AssessmentDto> GetAssessmentsInRange(DateTime start, DateTime end) =>
            _merchant.Assessments?.Where(x => !x.IsDeleted)
                .Where(x => x.StartDate >= start && x.StartDate <= end)
                ?? Enumerable.Empty<AssessmentDto>();
        public int CountAssets => _merchant.Assets?.Where(x => !x.IsDeleted).Count() ?? 0;
        public bool HasAssets => CountAssets > 0;
        public IEnumerable<AssetDto> GetAssetsInRange(DateTime start, DateTime end) =>
            _merchant.Assets?.Where(x => !x.IsDeleted)
                .Where(x => x.LastScanDate >= start && x.LastScanDate <= end)
                ?? Enumerable.Empty<AssetDto>();
        public int CountCompensatingControls => _merchant.CompensatingControls?.Where(x => !x.IsDeleted).Count() ?? 0;
        public bool HasCompensatingControls => CountCompensatingControls > 0;
        public decimal TotalCompensatingControlRank =>
            _merchant.CompensatingControls?.Where(x => !x.IsDeleted).Sum(x => (decimal)x.Rank) ?? 0;
        public decimal AverageCompensatingControlRank =>
            _merchant.CompensatingControls?.Where(x => !x.IsDeleted).Any() == true
                ? _merchant.CompensatingControls.Where(x => !x.IsDeleted).Average(x => (decimal)x.Rank)
                : 0m;
        public IEnumerable<CompensatingControlDto> GetCompensatingControlsInRange(DateTime start, DateTime end) =>
            _merchant.CompensatingControls?.Where(x => !x.IsDeleted)
                .Where(x => x.ApprovalDate >= start && x.ApprovalDate <= end)
                ?? Enumerable.Empty<CompensatingControlDto>();
        public int CountComplianceOfficers => _merchant.ComplianceOfficers?.Where(x => !x.IsActive).Count() ?? 0;
        public bool HasComplianceOfficers => CountComplianceOfficers > 0;
        public IEnumerable<ComplianceOfficerDto> GetComplianceOfficersInRange(DateTime start, DateTime end) =>
            _merchant.ComplianceOfficers?.Where(x => !x.IsActive)
                .Where(x => x.CreatedAt >= start && x.CreatedAt <= end)
                ?? Enumerable.Empty<ComplianceOfficerDto>();
        public int CountCryptographicInventories => _merchant.CryptographicInventories?.Where(x => !x.IsDeleted).Count() ?? 0;
        public bool HasCryptographicInventories => CountCryptographicInventories > 0;
        public decimal TotalCryptographicInventoryKeyLength =>
            _merchant.CryptographicInventories?.Where(x => !x.IsDeleted).Sum(x => (decimal)x.KeyLength) ?? 0;
        public IEnumerable<CryptographicInventoryDto> GetCryptographicInventoriesInRange(DateTime start, DateTime end) =>
            _merchant.CryptographicInventories?.Where(x => !x.IsDeleted)
                .Where(x => x.CreationDate >= start && x.CreationDate <= end)
                ?? Enumerable.Empty<CryptographicInventoryDto>();
        public int CountEvidences => _merchant.Evidences?.Where(x => !x.IsDeleted).Count() ?? 0;
        public bool HasEvidences => CountEvidences > 0;
        public IEnumerable<EvidenceDto> GetEvidencesInRange(DateTime start, DateTime end) =>
            _merchant.Evidences?.Where(x => !x.IsDeleted)
                .Where(x => x.CollectedDate >= start && x.CollectedDate <= end)
                ?? Enumerable.Empty<EvidenceDto>();
        public int CountNetworkSegmentations => _merchant.NetworkSegmentations?.Where(x => !x.IsDeleted).Count() ?? 0;
        public bool HasNetworkSegmentations => CountNetworkSegmentations > 0;
        public IEnumerable<NetworkSegmentationDto> GetNetworkSegmentationsInRange(DateTime start, DateTime end) =>
            _merchant.NetworkSegmentations?.Where(x => !x.IsDeleted)
                .Where(x => x.LastValidated >= start && x.LastValidated <= end)
                ?? Enumerable.Empty<NetworkSegmentationDto>();
        public int CountPaymentChannels => _merchant.PaymentChannels?.Where(x => !x.IsDeleted).Count() ?? 0;
        public bool HasPaymentChannels => CountPaymentChannels > 0;
        public decimal TotalPaymentChannelProcessingVolume =>
            _merchant.PaymentChannels?.Where(x => !x.IsDeleted).Sum(x => (decimal)x.ProcessingVolume) ?? 0;
        public IEnumerable<PaymentChannelDto> GetPaymentChannelsInRange(DateTime start, DateTime end) =>
            _merchant.PaymentChannels?.Where(x => !x.IsDeleted)
                .Where(x => x.CreatedAt >= start && x.CreatedAt <= end)
                ?? Enumerable.Empty<PaymentChannelDto>();
        public int CountServiceProviders => _merchant.ServiceProviders?.Where(x => !x.IsDeleted).Count() ?? 0;
        public bool HasServiceProviders => CountServiceProviders > 0;
        public IEnumerable<ServiceProviderDto> GetServiceProvidersInRange(DateTime start, DateTime end) =>
            _merchant.ServiceProviders?.Where(x => !x.IsDeleted)
                .Where(x => x.AOCExpiryDate >= start && x.AOCExpiryDate <= end)
                ?? Enumerable.Empty<ServiceProviderDto>();
        public IEnumerable<AssessmentControlDto> GetRelatedAssessmentControlsViaPath() =>
            _merchant.Assessments?
                .SelectMany(x => x.AssessmentControls)
                .Where(x => !x.IsDeleted)
                .Distinct()
                ?? Enumerable.Empty<AssessmentControlDto>();
        public IEnumerable<ControlEvidenceDto> GetRelatedControlEvidencesViaPath() =>
            _merchant.Assessments?
                .SelectMany(x => x.ControlEvidences)
                .Where(x => !x.IsDeleted)
                .Distinct()
                ?? Enumerable.Empty<ControlEvidenceDto>();
        public IEnumerable<ROCPackageDto> GetRelatedROCPackagesViaPath() =>
            _merchant.Assessments?
                .SelectMany(x => x.ROCPackages)
                .Where(x => !x.IsDeleted)
                .Distinct()
                ?? Enumerable.Empty<ROCPackageDto>();
        public decimal AverageTransitiveROCPackageRank =>
            GetRelatedROCPackagesViaPath().Any()
                ? GetRelatedROCPackagesViaPath().Average(x => (decimal)x.Rank)
                : 0m;
        public IEnumerable<AssetControlDto> GetRelatedAssetControlsViaPath() =>
            _merchant.Assets?
                .SelectMany(x => x.AssetControls)
                .Where(x => !x.IsDeleted)
                .Distinct()
                ?? Enumerable.Empty<AssetControlDto>();
        public IEnumerable<ScanScheduleDto> GetRelatedScanSchedulesViaPath() =>
            _merchant.Assets?
                .SelectMany(x => x.ScanSchedules)
                .Where(x => !x.IsDeleted)
                .Distinct()
                ?? Enumerable.Empty<ScanScheduleDto>();
        public IEnumerable<DescendantContext<CompensatingControlDto, ControlDto>> GetCompensatingControlsWithContext() =>
            _merchant.CompensatingControls?
                .Where(x => !x.IsDeleted)
                .Select(child => new DescendantContext<CompensatingControlDto, ControlDto>(
                    child,
                    child.Control))
                ?? Enumerable.Empty<DescendantContext<CompensatingControlDto, ControlDto>>();
        private IEnumerable<T> FilterDeleted<T>(IEnumerable<T> collection) where T : class
        {
            var isDeletedProp = typeof(T).GetProperty("IsDeleted");
            return isDeletedProp != null
                ? collection.Where(x => !(bool)isDeletedProp.GetValue(x))
                : collection;
        }
        public record AuditInfo(Guid CreatedBy, Guid? UpdatedBy, DateTime CreatedAt, DateTime? UpdatedAt);
        public record DescendantContext<TChild, TParent>(TChild Child, TParent UsefulParent);

    }
}