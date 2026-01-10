using System;
using System.Collections.Generic;
using System.Linq;
using PCIShieldLib.SharedKernel;
using PCIShieldLib.SharedKernel.Interfaces;
using Ardalis.GuardClauses;

namespace PCIShield.Domain.ModelEntityDto
{
    
    public class ROCPackageEntityDto : IEntityDto
    {
        public Guid ROCPackageId { get;  set; }
        
        public Guid TenantId { get;  set; }
        
        public string PackageVersion { get;  set; }
        
        public DateTime GeneratedDate { get;  set; }
        
        public string? QSAName { get;  set; }
        
        public string? QSACompany { get;  set; }
        
        public DateTime? SignatureDate { get;  set; }
        
        public string? AOCNumber { get;  set; }
        
        public int Rank { get;  set; }
        
        public DateTime CreatedAt { get;  set; }
        
        public Guid CreatedBy { get;  set; }
        
        public DateTime? UpdatedAt { get;  set; }
        
        public Guid? UpdatedBy { get;  set; }
        
        public bool IsDeleted { get;  set; }
        
        public Guid AssessmentId { get;  set; }
        
        public AssessmentEntityDto Assessment { get; set; } = new();
        public int DaysSinceGeneratedDate { get; set; }
        public bool IsDeletedFlag { get; set; }
        public bool IsRankCritical { get; set; }
        
        public ROCPackageEntityDto() {}
        
    }
}

