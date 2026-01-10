using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;
using System.Linq;
using Ardalis.GuardClauses;
using System.Collections.Immutable;
using System.Text.Json;
using LanguageExt;
using static LanguageExt.Prelude;
using PCIShield.Domain.Exceptions;
using PCIShield.Domain.Events;
using PCIShieldLib.SharedKernel;
using PCIShieldLib.SharedKernel.Interfaces;

namespace PCIShield.Domain.Entities
{
    
    public class Asset : BaseEntityEv<Guid>, IAggregateRoot, ITrackedEntity, ITenantEntity
    {
        [Key]
        public Guid AssetId { get; private set; }
        
        public Guid TenantId { get; private set; }
        
        public string AssetCode { get; private set; }
        
        public string AssetName { get; private set; }
        
        public int AssetType { get; private set; }
        
        public string? IPAddress { get; private set; }
        
        public string? Hostname { get; private set; }
        
        public bool IsInCDE { get; private set; }
        
        public string? NetworkZone { get; private set; }
        
        public DateTime? LastScanDate { get; private set; }
        
        public DateTime CreatedAt { get; private set; }
        
        public Guid CreatedBy { get; private set; }
        
        public DateTime? UpdatedAt { get; private set; }
        
        public Guid? UpdatedBy { get; private set; }
        
        public bool IsDeleted { get; private set; }
        
        public void SetTenantId(Guid tenantId)
        {
            TenantId = Guard.Against.Default(tenantId, nameof(tenantId));
        }
        public void SetMerchantId(Guid merchantId)
        {
            MerchantId = Guard.Against.Default(merchantId, nameof(merchantId));
        }
        public void SetAssetCode(string assetCode)
        {
            AssetCode = Guard.Against.NullOrEmpty(assetCode, nameof(assetCode));
        }
        public void SetAssetName(string assetName)
        {
            AssetName = Guard.Against.NullOrEmpty(assetName, nameof(assetName));
        }
        public void SetAssetType(int assetType)
        {
            AssetType = Guard.Against.Negative(assetType, nameof(assetType));
        }
        public void SetIPAddress(string ipaddress)
        {
            IPAddress = ipaddress;
        }
        public void SetHostname(string hostname)
        {
            Hostname = hostname;
        }
        public void SetIsInCDE(bool isInCDE)
        {
            IsInCDE = Guard.Against.Null(isInCDE, nameof(isInCDE));
        }
        public void SetNetworkZone(string networkZone)
        {
            NetworkZone = networkZone;
        }
        public void SetLastScanDate(DateTime? lastScanDate)
        {
            LastScanDate = lastScanDate;
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
        
        public void UpdateMerchantForAsset(Guid newMerchantId)
        {
            Guard.Against.Default(newMerchantId, nameof(newMerchantId));
            if (newMerchantId == MerchantId)
            {
                return;
            }
            
            MerchantId = newMerchantId;
            var assetUpdatedEvent = new AssetUpdatedEvent(this, "UpdatedEvent merchant");
            Events.Add(assetUpdatedEvent);
        }
        
        public virtual Merchant Merchant { get; private set; }
        
        public Guid MerchantId { get; private set; }
        
        private Asset() { }
        
        public Asset(Guid assetId, Guid merchantId, Guid tenantId, string assetCode, string assetName, int assetType, bool isInCDE, DateTime createdAt, Guid createdBy, bool isDeleted)
        {
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
            
            _assetControls = new();
            
            _scanSchedules = new();
            
            _vulnerabilities = new();
            
        }
        private readonly List<AssetControl> _assetControls = new();
        public IEnumerable<AssetControl> AssetControls => _assetControls.AsReadOnly();
        
        private readonly List<ScanSchedule> _scanSchedules = new();
        public IEnumerable<ScanSchedule> ScanSchedules => _scanSchedules.AsReadOnly();
        
        private readonly List<Vulnerability> _vulnerabilities = new();
        public IEnumerable<Vulnerability> Vulnerabilities => _vulnerabilities.AsReadOnly();
        
        public override bool Equals(object? obj) =>
        obj is Asset asset && Equals(asset);
        
        public bool Equals(Asset other)
        {
            if (ReferenceEquals(null, other))
            {
                return false;
            }
            
            if (ReferenceEquals(this, other))
            {
                return true;
            }
            
            return AssetId.Equals(other.AssetId);
        }
        
        public override int GetHashCode() => AssetId.GetHashCode();
        
        public static bool operator !=(Asset left, Asset right) => !(left == right);
        
        public static bool operator ==(Asset left, Asset right) => left?.Equals(right) ?? right is null;
        
        private void ValidateInvariants()
        {
            if (string.IsNullOrWhiteSpace(AssetCode))
            throw new InvalidOperationException("AssetCode cannot be null or whitespace.");
            if (AssetCode?.Length > 50)
            throw new InvalidOperationException("AssetCode cannot exceed 50 characters.");
            if (string.IsNullOrWhiteSpace(AssetName))
            throw new InvalidOperationException("AssetName cannot be null or whitespace.");
            if (AssetName?.Length > 200)
            throw new InvalidOperationException("AssetName cannot exceed 200 characters.");
            if (IPAddress?.Length > 45)
            throw new InvalidOperationException("IPAddress cannot exceed 45 characters.");
            if (Hostname?.Length > 255)
            throw new InvalidOperationException("Hostname cannot exceed 255 characters.");
            if (NetworkZone?.Length > 100)
            throw new InvalidOperationException("NetworkZone cannot exceed 100 characters.");
            if (LastScanDate == default)
            throw new InvalidOperationException("LastScanDate must be set.");
            if (UpdatedAt == default)
            throw new InvalidOperationException("UpdatedAt must be set.");
        }
        
        private void RaiseDomainEvent(BaseDomainEvent domainEvent)
        {
            Events.Add(domainEvent);
        }
        public static Eff<Validation<string, Asset>> Create(
        Guid assetId,
        Guid merchantId,
        Guid tenantId,
        string assetCode,
        string assetName,
        int assetType,
        bool isInCDE,
        DateTime createdAt,
        Guid createdBy,
        bool isDeleted
        )
        {
            return Eff(() => CreateAsset(
            assetId,
            merchantId,
            tenantId,
            assetCode,
            assetName,
            assetType,
            isInCDE,
            createdAt,
            createdBy,
            isDeleted
            ));
        }
        
        private static Validation<string, Asset> CreateAsset(
        Guid assetId,
        Guid merchantId,
        Guid tenantId,
        string assetCode,
        string assetName,
        int assetType,
        bool isInCDE,
        DateTime createdAt,
        Guid createdBy,
        bool isDeleted
        )
        {
            try
            {
                var now = DateTime.UtcNow;
                isDeleted = false;
                
                var asset = new Asset
                ( assetId,
                merchantId,
                tenantId,
                assetCode,
                assetName,
                assetType,
                isInCDE,
                createdAt,
                createdBy,
                isDeleted)
                ;
                
                return Validation<string, Asset>.Success(asset);
            }
            catch (Exception ex)
            {
                return Validation<string, Asset>.Fail(new Seq<string> { ex.Message });
            }
        }
        public static Eff<Validation<string, Asset>> Update(
        Asset existingAsset,
        Guid assetId,
        Guid tenantId,
        Guid merchantId,
        string assetCode,
        string assetName,
        int assetType,
        string ipaddress,
        string hostname,
        bool isInCDE,
        string networkZone,
        DateTime? lastScanDate,
        DateTime createdAt,
        Guid createdBy,
        DateTime? updatedAt,
        Guid? updatedBy,
        bool isDeleted
        )
        {
            return Eff(() => UpdateAsset(
            existingAsset,
            assetId,
            tenantId,
            merchantId,
            assetCode,
            assetName,
            assetType,
            ipaddress,
            hostname,
            isInCDE,
            networkZone,
            lastScanDate.ToOption().ToNullable(),
            createdAt,
            createdBy,
            updatedAt.ToOption().ToNullable(),
            updatedBy.ToOption().ToNullable(),
            isDeleted
            ));
        }
        
        private static Validation<string, Asset> UpdateAsset(
        Asset asset,
        Guid assetId,
        Guid tenantId,
        Guid merchantId,
        string assetCode,
        string assetName,
        int assetType,
        string ipaddress,
        string hostname,
        bool isInCDE,
        string networkZone,
        DateTime? lastScanDate,
        DateTime createdAt,
        Guid createdBy,
        DateTime? updatedAt,
        Guid? updatedBy,
        bool isDeleted
        )
        {
            try
            {
                
                asset.AssetId = assetId;
                asset.TenantId = tenantId;
                asset.MerchantId = merchantId;
                asset.AssetCode = assetCode;
                asset.AssetName = assetName;
                asset.AssetType = assetType;
                asset.IPAddress = ipaddress;
                asset.Hostname = hostname;
                asset.IsInCDE = isInCDE;
                asset.NetworkZone = networkZone;
                asset.LastScanDate = lastScanDate;
                asset.CreatedAt = createdAt;
                asset.CreatedBy = createdBy;
                asset.UpdatedAt = updatedAt;
                asset.UpdatedBy = updatedBy;
                asset.IsDeleted = isDeleted;
                asset.RaiseDomainEvent(new AssetUpdatedEvent(asset, "Updated"));
                
                return Success<string, Asset>(asset);
            }
            catch (Exception ex)
            {
                return Validation<string, Asset>.Fail(new Seq<string> { ex.Message });
            }
        }
        
        public Eff<Option<string>> AddAssetControl(AssetControl assetControl)
        {
            return Eff(() =>
            {
                if (assetControl is null)
                return Option<string>.Some("AssetControl cannot be null.");
                if (assetControl.CreatedAt == default)
                return Option<string>.Some("Created At must be set.");
                if (assetControl.CreatedAt < new DateTime(2024, 1, 1) || assetControl.CreatedAt > DateTime.UtcNow.AddYears(100))
                return Option<string>.Some("Created At is out of valid range.");
                _assetControls.Add(assetControl);
                RaiseDomainEvent(new AssetControlCreatedEvent(assetControl, "Created"));
                return Option<string>.None;
            });
        }
        public Eff<Option<string>> AddScanSchedule(ScanSchedule scanSchedule)
        {
            return Eff(() =>
            {
                if (scanSchedule is null)
                return Option<string>.Some("ScanSchedule cannot be null.");
                if (string.IsNullOrWhiteSpace(scanSchedule.Frequency))
                return Option<string>.Some("Frequency cannot be empty.");
                if (scanSchedule.Frequency.Length > 50)
                return Option<string>.Some("Frequency cannot exceed 50 characters.");
                if (scanSchedule.NextScanDate == default)
                return Option<string>.Some("Next Scan Date must be set.");
                if (scanSchedule.NextScanDate < new DateTime(2024, 1, 1) || scanSchedule.NextScanDate > DateTime.UtcNow.AddYears(100))
                return Option<string>.Some("Next Scan Date is out of valid range.");
                if (scanSchedule.CreatedAt == default)
                return Option<string>.Some("Created At must be set.");
                if (scanSchedule.CreatedAt < new DateTime(2024, 1, 1) || scanSchedule.CreatedAt > DateTime.UtcNow.AddYears(100))
                return Option<string>.Some("Created At is out of valid range.");
                _scanSchedules.Add(scanSchedule);
                RaiseDomainEvent(new ScanScheduleCreatedEvent(scanSchedule, "Created"));
                return Option<string>.None;
            });
        }
        public Eff<Option<string>> AddVulnerability(Vulnerability vulnerability)
        {
            return Eff(() =>
            {
                if (vulnerability is null)
                return Option<string>.Some("Vulnerability cannot be null.");
                if (string.IsNullOrWhiteSpace(vulnerability.VulnerabilityCode))
                return Option<string>.Some("Vulnerability Code cannot be empty.");
                if (vulnerability.VulnerabilityCode.Length > 50)
                return Option<string>.Some("Vulnerability Code cannot exceed 50 characters.");
                if (string.IsNullOrWhiteSpace(vulnerability.Title))
                return Option<string>.Some("Title cannot be empty.");
                if (vulnerability.Title.Length > 500)
                return Option<string>.Some("Title cannot exceed 500 characters.");
                if (vulnerability.DetectedDate == default)
                return Option<string>.Some("Detected Date must be set.");
                if (vulnerability.DetectedDate < new DateTime(2024, 1, 1) || vulnerability.DetectedDate > DateTime.UtcNow.AddYears(100))
                return Option<string>.Some("Detected Date is out of valid range.");
                if (vulnerability.Rank < 0)
                return Option<string>.Some("Rank is out of valid range.");
                if (vulnerability.CreatedAt == default)
                return Option<string>.Some("Created At must be set.");
                if (vulnerability.CreatedAt < new DateTime(2024, 1, 1) || vulnerability.CreatedAt > DateTime.UtcNow.AddYears(100))
                return Option<string>.Some("Created At is out of valid range.");
                _vulnerabilities.Add(vulnerability);
                RaiseDomainEvent(new VulnerabilityCreatedEvent(vulnerability, "Created"));
                return Option<string>.None;
            });
        }
        
        public static class AssetCalculations
        {
            public static decimal CalculateTotal(int assettype)
            {
                return assettype;
            }
            
            public static int CalculateDaysBetween(DateTime lastscandate, DateTime createdat)
            {
                return (createdat - lastscandate).Days;
            }
            
        }
    }
}

