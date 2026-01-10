using System;
using System.Collections.Generic;
using System.Linq;
using PCIShieldLib.SharedKernel;
using PCIShieldLib.SharedKernel.Interfaces;
using Ardalis.GuardClauses;

namespace PCIShield.Domain.ModelEntityDto
{
    
    public class AssessmentTypeEntityDto : IEntityDto
    {
        public Guid AssessmentTypeId { get;  set; }
        
        public string AssessmentTypeCode { get;  set; }
        
        public string AssessmentTypeName { get;  set; }
        
        public string? Description { get;  set; }
        
        public bool IsActive { get;  set; }
        
        public DateTime CreatedAt { get;  set; }
        
        public Guid CreatedBy { get;  set; }
        
        public DateTime? UpdatedAt { get;  set; }
        
        public Guid? UpdatedBy { get;  set; }
        
        public AssessmentTypeEntityDto() {}
        
    }
}

