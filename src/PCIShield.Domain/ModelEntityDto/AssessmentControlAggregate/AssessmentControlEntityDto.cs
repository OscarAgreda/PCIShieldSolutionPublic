using System;
using System.Collections.Generic;
using System.Linq;
using PCIShieldLib.SharedKernel;
using PCIShieldLib.SharedKernel.Interfaces;
using Ardalis.GuardClauses;

namespace PCIShield.Domain.ModelEntityDto
{
    
    public class AssessmentControlEntityDto : IEntityDto
    {
        public int RowId { get;  set; }
        
        public Guid TenantId { get;  set; }
        
        public int TestResult { get;  set; }
        
        public DateTime? TestDate { get;  set; }
        
        public Guid? TestedBy { get;  set; }
        
        public string? Notes { get;  set; }
        
        public DateTime CreatedAt { get;  set; }
        
        public Guid CreatedBy { get;  set; }
        
        public DateTime? UpdatedAt { get;  set; }
        
        public Guid? UpdatedBy { get;  set; }
        
        public bool IsDeleted { get;  set; }
        
        public Guid AssessmentId { get;  set; }
        
        public Guid ControlId { get;  set; }
        
        public AssessmentEntityDto Assessment { get; set; } = new();
        public ControlEntityDto Control { get; set; } = new();
        public AssessmentControlEntityDto() {}
        
    }
}

