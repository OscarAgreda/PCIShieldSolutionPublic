using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;

using Ardalis.GuardClauses;

using LanguageExt;

using PCIShield.Domain.Events;

using PCIShieldLib.SharedKernel;
using PCIShieldLib.SharedKernel.Interfaces;

using static LanguageExt.Prelude;

namespace PCIShield.Domain.Entities
{
    public class Merchant : BaseEntityEv<Guid>, IAggregateRoot, ITrackedEntity, ITenantEntity
    {
        [Key]
        public Guid MerchantId { get; private set; }

        public Guid TenantId { get; private set; }

        public string MerchantCode { get; private set; }

        public string MerchantName { get; private set; }

        public int MerchantLevel { get; private set; }

        public string AcquirerName { get; private set; }

        public string ProcessorMID { get; private set; }

        public decimal AnnualCardVolume { get; private set; }

        public DateTime? LastAssessmentDate { get; private set; }

        public DateTime NextAssessmentDue { get; private set; }

        public int ComplianceRank { get; private set; }

        public DateTime CreatedAt { get; private set; }

        public Guid CreatedBy { get; private set; }

        public DateTime? UpdatedAt { get; private set; }

        public Guid? UpdatedBy { get; private set; }

        public bool IsDeleted { get; private set; }

        public void SetTenantId(Guid tenantId)
        {
            TenantId = Guard.Against.Default(tenantId, nameof(tenantId));
        }

        public void SetMerchantCode(string merchantCode)
        {
            MerchantCode = Guard.Against.NullOrEmpty(merchantCode, nameof(merchantCode));
        }

        public void SetMerchantName(string merchantName)
        {
            MerchantName = Guard.Against.NullOrEmpty(merchantName, nameof(merchantName));
        }

        public void SetMerchantLevel(int merchantLevel)
        {
            MerchantLevel = Guard.Against.Negative(merchantLevel, nameof(merchantLevel));
        }

        public void SetAcquirerName(string acquirerName)
        {
            AcquirerName = Guard.Against.NullOrEmpty(acquirerName, nameof(acquirerName));
        }

        public void SetProcessorMID(string processorMID)
        {
            ProcessorMID = Guard.Against.NullOrEmpty(processorMID, nameof(processorMID));
        }

        public void SetAnnualCardVolume(decimal annualCardVolume)
        {
            AnnualCardVolume = Guard.Against.Negative(annualCardVolume, nameof(annualCardVolume));
        }

        public void SetLastAssessmentDate(DateTime? lastAssessmentDate)
        {
            LastAssessmentDate = lastAssessmentDate;
        }

        public void SetNextAssessmentDue(DateTime nextAssessmentDue)
        {
            NextAssessmentDue = Guard.Against.OutOfSQLDateRange(nextAssessmentDue, nameof(nextAssessmentDue));
        }

        public void SetComplianceRank(int complianceRank)
        {
            ComplianceRank = Guard.Against.Negative(complianceRank, nameof(complianceRank));
        }

        public void SetCreatedAt(DateTime createdAt)
        {
            CreatedAt = Guard.Against.OutOfSQLDateRange(createdAt, nameof(createdAt));
        }

        public void SetCreatedBy(Guid createdBy)
        {
            CreatedBy = Guard.Against.Default(createdBy, nameof(createdBy));
        }

        public void SetUpdatedAt(DateTime? updatedAt)
        {
            UpdatedAt = updatedAt;
        }

        public void SetUpdatedBy(Guid? updatedBy)
        {
            UpdatedBy = updatedBy;
        }

        public void SetIsDeleted(bool isDeleted)
        {
            IsDeleted = Guard.Against.Null(isDeleted, nameof(isDeleted));
        }

        private Merchant()
        { }

        public Merchant(Guid merchantId, Guid tenantId, string merchantCode, string merchantName, int merchantLevel, string acquirerName, string processorMID, decimal annualCardVolume, DateTime nextAssessmentDue, int complianceRank, DateTime createdAt, Guid createdBy, bool isDeleted)
        {
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

            _assessments = new();

            _assets = new();

            _compensatingControls = new();

            _complianceOfficers = new();

            _cryptographicInventories = new();

            _evidences = new();

            _networkSegmentations = new();

            _paymentChannels = new();

            _serviceProviders = new();
        }

        private readonly List<Assessment> _assessments = new();
        public IEnumerable<Assessment> Assessments => _assessments.AsReadOnly();

        private readonly List<Asset> _assets = new();
        public IEnumerable<Asset> Assets => _assets.AsReadOnly();

        private readonly List<CompensatingControl> _compensatingControls = new();
        public IEnumerable<CompensatingControl> CompensatingControls => _compensatingControls.AsReadOnly();

        private readonly List<ComplianceOfficer> _complianceOfficers = new();
        public IEnumerable<ComplianceOfficer> ComplianceOfficers => _complianceOfficers.AsReadOnly();

        private readonly List<CryptographicInventory> _cryptographicInventories = new();
        public IEnumerable<CryptographicInventory> CryptographicInventories => _cryptographicInventories.AsReadOnly();

        private readonly List<Evidence> _evidences = new();
        public IEnumerable<Evidence> Evidences => _evidences.AsReadOnly();

        private readonly List<NetworkSegmentation> _networkSegmentations = new();
        public IEnumerable<NetworkSegmentation> NetworkSegmentations => _networkSegmentations.AsReadOnly();

        private readonly List<PaymentChannel> _paymentChannels = new();
        public IEnumerable<PaymentChannel> PaymentChannels => _paymentChannels.AsReadOnly();

        private readonly List<ServiceProvider> _serviceProviders = new();
        public IEnumerable<ServiceProvider> ServiceProviders => _serviceProviders.AsReadOnly();

        public override bool Equals(object? obj) =>
        obj is Merchant merchant && Equals(merchant);

        public bool Equals(Merchant other)
        {
            if (ReferenceEquals(null, other))
            {
                return false;
            }

            if (ReferenceEquals(this, other))
            {
                return true;
            }

            return MerchantId.Equals(other.MerchantId);
        }

        public override int GetHashCode() => MerchantId.GetHashCode();

        public static bool operator !=(Merchant left, Merchant right) => !(left == right);

        public static bool operator ==(Merchant left, Merchant right) => left?.Equals(right) ?? right is null;

        private void RaiseDomainEvent(BaseDomainEvent domainEvent)
        {
            Events.Add(domainEvent);
        }

        public static Eff<Validation<string, Merchant>> Create(
        Guid merchantId,
        Guid tenantId,
        string merchantCode,
        string merchantName,
        int merchantLevel,
        string acquirerName,
        string processorMID,
        decimal annualCardVolume,
        DateTime nextAssessmentDue,
        int complianceRank,
        DateTime createdAt,
        Guid createdBy,
        bool isDeleted
        )
        {
            return Eff(() => CreateMerchant(
            merchantId,
            tenantId,
            merchantCode,
            merchantName,
            merchantLevel,
            acquirerName,
            processorMID,
            annualCardVolume,
            nextAssessmentDue,
            complianceRank,
            createdAt,
            createdBy,
            isDeleted
            ));
        }

        private static Validation<string, Merchant> CreateMerchant(
        Guid merchantId,
        Guid tenantId,
        string merchantCode,
        string merchantName,
        int merchantLevel,
        string acquirerName,
        string processorMID,
        decimal annualCardVolume,
        DateTime nextAssessmentDue,
        int complianceRank,
        DateTime createdAt,
        Guid createdBy,
        bool isDeleted
        )
        {
            try
            {
                var now = DateTime.UtcNow;
                isDeleted = false;

                var merchant = new Merchant
                (merchantId,
                tenantId,
                merchantCode,
                merchantName,
                merchantLevel,
                acquirerName,
                processorMID,
                annualCardVolume,
                nextAssessmentDue,
                complianceRank,
                createdAt,
                createdBy,
                isDeleted)
                ;

                return Validation<string, Merchant>.Success(merchant);
            }
            catch (Exception ex)
            {
                return Validation<string, Merchant>.Fail(new Seq<string> { ex.Message });
            }
        }

        public static Eff<Validation<string, Merchant>> Update(
        Merchant existingMerchant,
        Guid merchantId,
        Guid tenantId,
        string merchantCode,
        string merchantName,
        int merchantLevel,
        string acquirerName,
        string processorMID,
        decimal annualCardVolume,
        DateTime? lastAssessmentDate,
        DateTime nextAssessmentDue,
        int complianceRank,
        DateTime createdAt,
        Guid createdBy,
        DateTime? updatedAt,
        Guid? updatedBy,
        bool isDeleted
        )
        {
            return Eff(() => UpdateMerchant(
            existingMerchant,
            merchantId,
            tenantId,
            merchantCode,
            merchantName,
            merchantLevel,
            acquirerName,
            processorMID,
            annualCardVolume,
            lastAssessmentDate.ToOption().ToNullable(),
            nextAssessmentDue,
            complianceRank,
            createdAt,
            createdBy,
            updatedAt.ToOption().ToNullable(),
            updatedBy.ToOption().ToNullable(),
            isDeleted
            ));
        }

        private static Validation<string, Merchant> UpdateMerchant(
        Merchant merchant,
        Guid merchantId,
        Guid tenantId,
        string merchantCode,
        string merchantName,
        int merchantLevel,
        string acquirerName,
        string processorMID,
        decimal annualCardVolume,
        DateTime? lastAssessmentDate,
        DateTime nextAssessmentDue,
        int complianceRank,
        DateTime createdAt,
        Guid createdBy,
        DateTime? updatedAt,
        Guid? updatedBy,
        bool isDeleted
        )
        {
            try
            {
                merchant.MerchantId = merchantId;
                merchant.TenantId = tenantId;
                merchant.MerchantCode = merchantCode;
                merchant.MerchantName = merchantName;
                merchant.MerchantLevel = merchantLevel;
                merchant.AcquirerName = acquirerName;
                merchant.ProcessorMID = processorMID;
                merchant.AnnualCardVolume = annualCardVolume;
                merchant.LastAssessmentDate = lastAssessmentDate;
                merchant.NextAssessmentDue = nextAssessmentDue;
                merchant.ComplianceRank = complianceRank;
                merchant.CreatedAt = createdAt;
                merchant.CreatedBy = createdBy;
                merchant.UpdatedAt = updatedAt;
                merchant.UpdatedBy = updatedBy;
                merchant.IsDeleted = isDeleted;
                merchant.RaiseDomainEvent(new MerchantUpdatedEvent(merchant, "Updated"));

                return Success<string, Merchant>(merchant);
            }
            catch (Exception ex)
            {
                return Validation<string, Merchant>.Fail(new Seq<string> { ex.Message });
            }
        }

        public Eff<Option<string>> AddAssessment(Assessment assessment)
        {
            return Eff(() =>
            {
                if (assessment is null)
                    return Option<string>.Some("Assessment cannot be null.");
                if (string.IsNullOrWhiteSpace(assessment.AssessmentCode))
                    return Option<string>.Some("Assessment Code cannot be empty.");
                if (assessment.AssessmentCode.Length > 30)
                    return Option<string>.Some("Assessment Code cannot exceed 30 characters.");
                if (assessment.AssessmentType < 0)
                    return Option<string>.Some("Assessment Type is out of valid range.");
                if (string.IsNullOrWhiteSpace(assessment.AssessmentPeriod))
                    return Option<string>.Some("Assessment Period cannot be empty.");
                if (assessment.AssessmentPeriod.Length > 20)
                    return Option<string>.Some("Assessment Period cannot exceed 20 characters.");
                if (assessment.StartDate == default)
                    return Option<string>.Some("Start Date must be set.");
                if (assessment.StartDate < new DateTime(2024, 1, 1) || assessment.StartDate > DateTime.UtcNow.AddYears(100))
                    return Option<string>.Some("Start Date is out of valid range.");
                if (assessment.EndDate == default)
                    return Option<string>.Some("End Date must be set.");
                if (assessment.EndDate < new DateTime(2024, 1, 1) || assessment.EndDate > DateTime.UtcNow.AddYears(100))
                    return Option<string>.Some("End Date is out of valid range.");
                if (assessment.Rank < 0)
                    return Option<string>.Some("Rank is out of valid range.");
                if (assessment.CreatedAt == default)
                    return Option<string>.Some("Created At must be set.");
                if (assessment.CreatedAt < new DateTime(2024, 1, 1) || assessment.CreatedAt > DateTime.UtcNow.AddYears(100))
                    return Option<string>.Some("Created At is out of valid range.");
                _assessments.Add(assessment);
                RaiseDomainEvent(new AssessmentCreatedEvent(assessment, "Created"));
                return Option<string>.None;
            });
        }

        public Eff<Option<string>> AddAsset(Asset asset)
        {
            return Eff(() =>
            {
                if (asset is null)
                    return Option<string>.Some("Asset cannot be null.");
                if (string.IsNullOrWhiteSpace(asset.AssetCode))
                    return Option<string>.Some("Asset Code cannot be empty.");
                if (asset.AssetCode.Length > 50)
                    return Option<string>.Some("Asset Code cannot exceed 50 characters.");
                if (string.IsNullOrWhiteSpace(asset.AssetName))
                    return Option<string>.Some("Asset Name cannot be empty.");
                if (asset.AssetName.Length > 200)
                    return Option<string>.Some("Asset Name cannot exceed 200 characters.");
                if (asset.CreatedAt == default)
                    return Option<string>.Some("Created At must be set.");
                if (asset.CreatedAt < new DateTime(2024, 1, 1) || asset.CreatedAt > DateTime.UtcNow.AddYears(100))
                    return Option<string>.Some("Created At is out of valid range.");
                _assets.Add(asset);
                RaiseDomainEvent(new AssetCreatedEvent(asset, "Created"));
                return Option<string>.None;
            });
        }

        public Eff<Option<string>> AddCompensatingControl(CompensatingControl compensatingControl)
        {
            return Eff(() =>
            {
                if (compensatingControl is null)
                    return Option<string>.Some("CompensatingControl cannot be null.");
                if (string.IsNullOrWhiteSpace(compensatingControl.Justification))
                    return Option<string>.Some("Justification cannot be empty.");
                if (compensatingControl.Justification.Length > -1)
                    return Option<string>.Some("Justification cannot exceed -1 characters.");
                if (string.IsNullOrWhiteSpace(compensatingControl.ImplementationDetails))
                    return Option<string>.Some("Implementation Details cannot be empty.");
                if (compensatingControl.ImplementationDetails.Length > -1)
                    return Option<string>.Some("Implementation Details cannot exceed -1 characters.");
                if (compensatingControl.ExpiryDate == default)
                    return Option<string>.Some("Expiry Date must be set.");
                if (compensatingControl.ExpiryDate < new DateTime(2024, 1, 1) || compensatingControl.ExpiryDate > DateTime.UtcNow.AddYears(100))
                    return Option<string>.Some("Expiry Date is out of valid range.");
                if (compensatingControl.Rank < 0)
                    return Option<string>.Some("Rank is out of valid range.");
                if (compensatingControl.CreatedAt == default)
                    return Option<string>.Some("Created At must be set.");
                if (compensatingControl.CreatedAt < new DateTime(2024, 1, 1) || compensatingControl.CreatedAt > DateTime.UtcNow.AddYears(100))
                    return Option<string>.Some("Created At is out of valid range.");
                _compensatingControls.Add(compensatingControl);
                RaiseDomainEvent(new CompensatingControlCreatedEvent(compensatingControl, "Created"));
                return Option<string>.None;
            });
        }

        public Eff<Option<string>> AddComplianceOfficer(ComplianceOfficer complianceOfficer)
        {
            return Eff(() =>
            {
                if (complianceOfficer is null)
                    return Option<string>.Some("ComplianceOfficer cannot be null.");
                if (string.IsNullOrWhiteSpace(complianceOfficer.OfficerCode))
                    return Option<string>.Some("Officer Code cannot be empty.");
                if (complianceOfficer.OfficerCode.Length > 30)
                    return Option<string>.Some("Officer Code cannot exceed 30 characters.");
                if (string.IsNullOrWhiteSpace(complianceOfficer.FirstName))
                    return Option<string>.Some("First Name cannot be empty.");
                if (complianceOfficer.FirstName.Length > 100)
                    return Option<string>.Some("First Name cannot exceed 100 characters.");
                if (string.IsNullOrWhiteSpace(complianceOfficer.LastName))
                    return Option<string>.Some("Last Name cannot be empty.");
                if (complianceOfficer.LastName.Length > 100)
                    return Option<string>.Some("Last Name cannot exceed 100 characters.");
                if (string.IsNullOrWhiteSpace(complianceOfficer.Email))
                    return Option<string>.Some("Email cannot be empty.");
                if (complianceOfficer.Email.Length > 320)
                    return Option<string>.Some("Email cannot exceed 320 characters.");
                if (complianceOfficer.CreatedAt == default)
                    return Option<string>.Some("Created At must be set.");
                if (complianceOfficer.CreatedAt < new DateTime(2024, 1, 1) || complianceOfficer.CreatedAt > DateTime.UtcNow.AddYears(100))
                    return Option<string>.Some("Created At is out of valid range.");
                _complianceOfficers.Add(complianceOfficer);
                RaiseDomainEvent(new ComplianceOfficerCreatedEvent(complianceOfficer, "Created"));
                return Option<string>.None;
            });
        }

        public Eff<Option<string>> AddCryptographicInventory(CryptographicInventory cryptographicInventory)
        {
            return Eff(() =>
            {
                if (cryptographicInventory is null)
                    return Option<string>.Some("CryptographicInventory cannot be null.");
                if (string.IsNullOrWhiteSpace(cryptographicInventory.KeyName))
                    return Option<string>.Some("Key Name cannot be empty.");
                if (cryptographicInventory.KeyName.Length > 200)
                    return Option<string>.Some("Key Name cannot exceed 200 characters.");
                if (string.IsNullOrWhiteSpace(cryptographicInventory.KeyType))
                    return Option<string>.Some("Key Type cannot be empty.");
                if (cryptographicInventory.KeyType.Length > 50)
                    return Option<string>.Some("Key Type cannot exceed 50 characters.");
                if (string.IsNullOrWhiteSpace(cryptographicInventory.Algorithm))
                    return Option<string>.Some("Algorithm cannot be empty.");
                if (cryptographicInventory.Algorithm.Length > 50)
                    return Option<string>.Some("Algorithm cannot exceed 50 characters.");
                if (cryptographicInventory.KeyLength < 0)
                    return Option<string>.Some("Key Length is out of valid range.");
                if (string.IsNullOrWhiteSpace(cryptographicInventory.KeyLocation))
                    return Option<string>.Some("Key Location cannot be empty.");
                if (cryptographicInventory.KeyLocation.Length > 200)
                    return Option<string>.Some("Key Location cannot exceed 200 characters.");
                if (cryptographicInventory.CreationDate == default)
                    return Option<string>.Some("Creation Date must be set.");
                if (cryptographicInventory.CreationDate < new DateTime(2024, 1, 1) || cryptographicInventory.CreationDate > DateTime.UtcNow.AddYears(100))
                    return Option<string>.Some("Creation Date is out of valid range.");
                if (cryptographicInventory.NextRotationDue == default)
                    return Option<string>.Some("Next Rotation Due must be set.");
                if (cryptographicInventory.NextRotationDue < new DateTime(2024, 1, 1) || cryptographicInventory.NextRotationDue > DateTime.UtcNow.AddYears(100))
                    return Option<string>.Some("Next Rotation Due is out of valid range.");
                if (cryptographicInventory.CreatedAt == default)
                    return Option<string>.Some("Created At must be set.");
                if (cryptographicInventory.CreatedAt < new DateTime(2024, 1, 1) || cryptographicInventory.CreatedAt > DateTime.UtcNow.AddYears(100))
                    return Option<string>.Some("Created At is out of valid range.");
                _cryptographicInventories.Add(cryptographicInventory);
                RaiseDomainEvent(new CryptographicInventoryCreatedEvent(cryptographicInventory, "Created"));
                return Option<string>.None;
            });
        }

        public Eff<Option<string>> AddEvidence(Evidence evidence)
        {
            return Eff(() =>
            {
                if (evidence is null)
                    return Option<string>.Some("Evidence cannot be null.");
                if (string.IsNullOrWhiteSpace(evidence.EvidenceCode))
                    return Option<string>.Some("Evidence Code cannot be empty.");
                if (evidence.EvidenceCode.Length > 50)
                    return Option<string>.Some("Evidence Code cannot exceed 50 characters.");
                if (string.IsNullOrWhiteSpace(evidence.EvidenceTitle))
                    return Option<string>.Some("Evidence Title cannot be empty.");
                if (evidence.EvidenceTitle.Length > 200)
                    return Option<string>.Some("Evidence Title cannot exceed 200 characters.");
                if (evidence.CollectedDate == default)
                    return Option<string>.Some("Collected Date must be set.");
                if (evidence.CollectedDate < new DateTime(2024, 1, 1) || evidence.CollectedDate > DateTime.UtcNow.AddYears(100))
                    return Option<string>.Some("Collected Date is out of valid range.");
                if (evidence.CreatedAt == default)
                    return Option<string>.Some("Created At must be set.");
                if (evidence.CreatedAt < new DateTime(2024, 1, 1) || evidence.CreatedAt > DateTime.UtcNow.AddYears(100))
                    return Option<string>.Some("Created At is out of valid range.");
                _evidences.Add(evidence);
                RaiseDomainEvent(new EvidenceCreatedEvent(evidence, "Created"));
                return Option<string>.None;
            });
        }

        public Eff<Option<string>> AddNetworkSegmentation(NetworkSegmentation networkSegmentation)
        {
            return Eff(() =>
            {
                if (networkSegmentation is null)
                    return Option<string>.Some("NetworkSegmentation cannot be null.");
                if (string.IsNullOrWhiteSpace(networkSegmentation.SegmentName))
                    return Option<string>.Some("Segment Name cannot be empty.");
                if (networkSegmentation.SegmentName.Length > 100)
                    return Option<string>.Some("Segment Name cannot exceed 100 characters.");
                if (string.IsNullOrWhiteSpace(networkSegmentation.IPRange))
                    return Option<string>.Some("Iprange cannot be empty.");
                if (networkSegmentation.IPRange.Length > 50)
                    return Option<string>.Some("Iprange cannot exceed 50 characters.");
                if (networkSegmentation.CreatedAt == default)
                    return Option<string>.Some("Created At must be set.");
                if (networkSegmentation.CreatedAt < new DateTime(2024, 1, 1) || networkSegmentation.CreatedAt > DateTime.UtcNow.AddYears(100))
                    return Option<string>.Some("Created At is out of valid range.");
                _networkSegmentations.Add(networkSegmentation);
                RaiseDomainEvent(new NetworkSegmentationCreatedEvent(networkSegmentation, "Created"));
                return Option<string>.None;
            });
        }

        public Eff<Option<string>> AddPaymentChannel(PaymentChannel paymentChannel)
        {
            return Eff(() =>
            {
                if (paymentChannel is null)
                    return Option<string>.Some("PaymentChannel cannot be null.");
                if (string.IsNullOrWhiteSpace(paymentChannel.ChannelCode))
                    return Option<string>.Some("Channel Code cannot be empty.");
                if (paymentChannel.ChannelCode.Length > 30)
                    return Option<string>.Some("Channel Code cannot exceed 30 characters.");
                if (string.IsNullOrWhiteSpace(paymentChannel.ChannelName))
                    return Option<string>.Some("Channel Name cannot be empty.");
                if (paymentChannel.ChannelName.Length > 100)
                    return Option<string>.Some("Channel Name cannot exceed 100 characters.");
                if (paymentChannel.ChannelType < 0)
                    return Option<string>.Some("Channel Type is out of valid range.");
                if (paymentChannel.CreatedAt == default)
                    return Option<string>.Some("Created At must be set.");
                if (paymentChannel.CreatedAt < new DateTime(2024, 1, 1) || paymentChannel.CreatedAt > DateTime.UtcNow.AddYears(100))
                    return Option<string>.Some("Created At is out of valid range.");
                _paymentChannels.Add(paymentChannel);
                RecalculatePaymentChannelTotal();
                RaiseDomainEvent(new PaymentChannelCreatedEvent(paymentChannel, "Created"));
                return Option<string>.None;
            });
        }

        private void RecalculatePaymentChannelTotal()
        {
            try
            {
                throw new InvalidOperationException("Unable to find an appropriate Total value object");
            }
            catch (Exception ex)
            {
                throw new InvalidOperationException($"Error recalculating PaymentChannel total: {ex.Message}", ex);
            }
        }



        private decimal GetTaxRate(string taxCode)
        {
            return taxCode switch
            {
                "DEFAULT" => 0.1m,
                "REDUCED" => 0.05m,
                _ => 0m
            };
        }

        public Eff<Option<string>> AddServiceProvider(ServiceProvider serviceProvider)
        {
            return Eff(() =>
            {
                if (serviceProvider is null)
                    return Option<string>.Some("ServiceProvider cannot be null.");
                if (string.IsNullOrWhiteSpace(serviceProvider.ProviderName))
                    return Option<string>.Some("Provider Name cannot be empty.");
                if (serviceProvider.ProviderName.Length > 200)
                    return Option<string>.Some("Provider Name cannot exceed 200 characters.");
                if (string.IsNullOrWhiteSpace(serviceProvider.ServiceType))
                    return Option<string>.Some("Service Type cannot be empty.");
                if (serviceProvider.ServiceType.Length > 100)
                    return Option<string>.Some("Service Type cannot exceed 100 characters.");
                if (serviceProvider.CreatedAt == default)
                    return Option<string>.Some("Created At must be set.");
                if (serviceProvider.CreatedAt < new DateTime(2024, 1, 1) || serviceProvider.CreatedAt > DateTime.UtcNow.AddYears(100))
                    return Option<string>.Some("Created At is out of valid range.");
                _serviceProviders.Add(serviceProvider);
                RaiseDomainEvent(new ServiceProviderCreatedEvent(serviceProvider, "Created"));
                return Option<string>.None;
            });
        }

        public static class MerchantCalculations
        {
            public static decimal CalculateTotal(int merchantlevel, decimal annualcardvolume, int compliancerank)
            {
                return merchantlevel + annualcardvolume + compliancerank;
            }

            public static decimal CalculateAverage(int merchantlevel, decimal annualcardvolume, int compliancerank)
            {
                return (merchantlevel + annualcardvolume + compliancerank) / 3;
            }

            public static decimal CalculateMerchantLevelPerAnnualCardVolume(int merchantlevel, decimal annualcardvolume)
            {
                if (annualcardvolume == 0)
                    throw new DivideByZeroException("Cannot calculate rate: AnnualCardVolume is zero.");
                return (decimal)merchantlevel / annualcardvolume;
            }

            public static decimal CalculateMerchantLevelPerComplianceRank(int merchantlevel, int compliancerank)
            {
                if (compliancerank == 0)
                    throw new DivideByZeroException("Cannot calculate rate: ComplianceRank is zero.");
                return (decimal)merchantlevel / compliancerank;
            }

            public static decimal CalculateAnnualCardVolumePerComplianceRank(decimal annualcardvolume, int compliancerank)
            {
                if (compliancerank == 0)
                    throw new DivideByZeroException("Cannot calculate rate: ComplianceRank is zero.");
                return (decimal)annualcardvolume / compliancerank;
            }

            public static int CalculateDaysBetween(DateTime lastassessmentdate, DateTime nextassessmentdue)
            {
                return (nextassessmentdue - lastassessmentdate).Days;
            }

            public static decimal CalculateMerchantTotalForPaymentChannel(IEnumerable<PaymentChannel> paymentChannels)
            {
                return paymentChannels.Sum(x => x.ProcessingVolume);
            }
        }
    }
}