using System;
using System.Collections.Generic;
using System.Linq;
using PCIShieldLib.SharedKernel;
using PCIShieldLib.SharedKernel.Interfaces;
using Ardalis.GuardClauses;

namespace PCIShield.Domain.ModelEntityDto
{
    
    public class AssetEntityDto : IEntityDto
    {
        public Guid AssetId { get;  set; }
        
        public Guid TenantId { get;  set; }
        
        public string AssetCode { get;  set; }
        
        public string AssetName { get;  set; }
        
        public int AssetType { get;  set; }
        
        public string? IPAddress { get;  set; }
        
        public string? Hostname { get;  set; }
        
        public bool IsInCDE { get;  set; }
        
        public string? NetworkZone { get;  set; }
        
        public DateTime? LastScanDate { get;  set; }
        
        public DateTime CreatedAt { get;  set; }
        
        public Guid CreatedBy { get;  set; }
        
        public DateTime? UpdatedAt { get;  set; }
        
        public Guid? UpdatedBy { get;  set; }
        
        public bool IsDeleted { get;  set; }
        
        public Guid MerchantId { get;  set; }
        
        public List<ScanScheduleEntityDto> ScanSchedules { get; set; } = new();
        public List<VulnerabilityEntityDto> Vulnerabilities { get; set; } = new();
        public MerchantEntityDto Merchant { get; set; } = new();
        public List<AssetControlEntityDto> AssetControls { get; set; } = new();
        public int ActiveAssetControlCount { get; set; }
        public int ActiveScanScheduleCount { get; set; }
        public int ActiveVulnerabilityCount { get; set; }
        public int CriticalScanScheduleCount { get; set; }
        public int CriticalVulnerabilityCount { get; set; }
        public int DaysSinceLastScanDate { get; set; }
        public bool IsAssetTypeCritical { get; set; }
        public bool IsInCDEFlag { get; set; }
        public DateTime? LatestAssetControlUpdatedAt { get; set; }
        public DateTime? LatestScanScheduleNextScanDate { get; set; }
        public DateTime? LatestVulnerabilityDetectedDate { get; set; }
        public int TotalAssetControlCount { get; set; }
        public int TotalScanScheduleCount { get; set; }
        public int TotalVulnerabilityCount { get; set; }
        public Guid ROCPackageId { get; set; }
        public ROCPackageEntityDto? ROCPackage { get; set; }
        public Guid PaymentPageId { get; set; }
        public PaymentPageEntityDto? PaymentPage { get; set; }
        public Guid ScriptId { get; set; }
        public ScriptEntityDto? Script { get; set; }
        
        public AssetEntityDto() {}
        
    }
}

