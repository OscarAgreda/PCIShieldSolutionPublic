using System;
using System.Collections.Generic;
using System.Linq;
using PCIShieldLib.SharedKernel;
using PCIShieldLib.SharedKernel.Interfaces;
using Ardalis.GuardClauses;

namespace PCIShield.Domain.ModelEntityDto
{
    
    public class PaymentChannelEntityDto : IEntityDto
    {
        public Guid PaymentChannelId { get;  set; }
        
        public Guid TenantId { get;  set; }
        
        public string ChannelCode { get;  set; }
        
        public string ChannelName { get;  set; }
        
        public int ChannelType { get;  set; }
        
        public decimal ProcessingVolume { get;  set; }
        
        public bool IsInScope { get;  set; }
        
        public bool TokenizationEnabled { get;  set; }
        
        public DateTime CreatedAt { get;  set; }
        
        public Guid CreatedBy { get;  set; }
        
        public DateTime? UpdatedAt { get;  set; }
        
        public Guid? UpdatedBy { get;  set; }
        
        public bool IsDeleted { get;  set; }
        
        public Guid MerchantId { get;  set; }
        
        public List<PaymentPageEntityDto> PaymentPages { get; set; } = new();
        public MerchantEntityDto Merchant { get; set; } = new();
        public int ActivePaymentPageCount { get; set; }
        public int DaysSinceUpdatedAt { get; set; }
        public bool IsChannelTypeCritical { get; set; }
        public bool IsInScopeFlag { get; set; }
        public DateTime? LatestPaymentPageUpdatedAt { get; set; }
        public int TotalPaymentPageCount { get; set; }
        public List<ScriptEntityDto> Scripts { get; set; } = new();
        public Guid ROCPackageId { get; set; }
        public ROCPackageEntityDto? ROCPackage { get; set; }
        public Guid ScanScheduleId { get; set; }
        public ScanScheduleEntityDto? ScanSchedule { get; set; }
        public Guid VulnerabilityId { get; set; }
        public VulnerabilityEntityDto? Vulnerability { get; set; }
        
        public PaymentChannelEntityDto() {}
        
    }
}

