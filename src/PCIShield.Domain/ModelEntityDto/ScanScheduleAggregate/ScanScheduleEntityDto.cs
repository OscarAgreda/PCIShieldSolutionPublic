using System;
using System.Collections.Generic;
using System.Linq;
using PCIShieldLib.SharedKernel;
using PCIShieldLib.SharedKernel.Interfaces;
using Ardalis.GuardClauses;

namespace PCIShield.Domain.ModelEntityDto
{
    
    public class ScanScheduleEntityDto : IEntityDto
    {
        public Guid ScanScheduleId { get;  set; }
        
        public Guid TenantId { get;  set; }
        
        public int ScanType { get;  set; }
        
        public string Frequency { get;  set; }
        
        public DateTime NextScanDate { get;  set; }
        
        public TimeSpan? BlackoutStart { get;  set; }
        
        public TimeSpan? BlackoutEnd { get;  set; }
        
        public bool IsEnabled { get;  set; }
        
        public DateTime CreatedAt { get;  set; }
        
        public Guid CreatedBy { get;  set; }
        
        public DateTime? UpdatedAt { get;  set; }
        
        public Guid? UpdatedBy { get;  set; }
        
        public bool IsDeleted { get;  set; }
        
        public Guid AssetId { get;  set; }
        
        public AssetEntityDto Asset { get; set; } = new();
        public int DaysSinceNextScanDate { get; set; }
        public bool IsEnabledFlag { get; set; }
        public bool IsScanTypeCritical { get; set; }
        
        public ScanScheduleEntityDto() {}
        
    }
}

