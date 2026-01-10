using System;
using System.Collections.Generic;
using System.Linq;
using PCIShieldLib.SharedKernel;
using PCIShieldLib.SharedKernel.Interfaces;
using Ardalis.GuardClauses;

namespace PCIShield.Domain.ModelEntityDto
{
    
    public class ControlEvidenceEntityDto : IEntityDto
    {
        public int RowId { get;  set; }
        
        public Guid TenantId { get;  set; }
        
        public bool IsPrimary { get;  set; }
        
        public DateTime CreatedAt { get;  set; }
        
        public Guid CreatedBy { get;  set; }
        
        public DateTime? UpdatedAt { get;  set; }
        
        public Guid? UpdatedBy { get;  set; }
        
        public bool IsDeleted { get;  set; }
        
        public Guid AssessmentId { get;  set; }
        
        public Guid ControlId { get;  set; }
        
        public Guid EvidenceId { get;  set; }
        
        public AssessmentEntityDto Assessment { get; set; } = new();
        public ControlEntityDto Control { get; set; } = new();
        public EvidenceEntityDto Evidence { get; set; } = new();
        public ControlEvidenceEntityDto() {}
        
    }
}

