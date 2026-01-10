using System;
using System.Collections.Generic;
using System.Linq;
using PCIShieldLib.SharedKernel;
using PCIShieldLib.SharedKernel.Interfaces;
using Ardalis.GuardClauses;

namespace PCIShield.Domain.ModelEntityDto
{
    
    public class ControlCategoryEntityDto : IEntityDto
    {
        public Guid ControlCategoryId { get;  set; }
        
        public string ControlCategoryCode { get;  set; }
        
        public string ControlCategoryName { get;  set; }
        
        public string RequirementSection { get;  set; }
        
        public bool IsActive { get;  set; }
        
        public DateTime CreatedAt { get;  set; }
        
        public Guid CreatedBy { get;  set; }
        
        public DateTime? UpdatedAt { get;  set; }
        
        public Guid? UpdatedBy { get;  set; }
        
        public ControlCategoryEntityDto() {}
        
    }
}

