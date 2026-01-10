using System;
using System.Collections.Generic;
using System.Linq;
using PCIShieldLib.SharedKernel;
using PCIShieldLib.SharedKernel.Interfaces;
using Ardalis.GuardClauses;

namespace PCIShield.Domain.ModelEntityDto
{
    
    public class EvidenceTypeEntityDto : IEntityDto
    {
        public Guid EvidenceTypeId { get;  set; }
        
        public string EvidenceTypeCode { get;  set; }
        
        public string EvidenceTypeName { get;  set; }
        
        public string? FileExtensions { get;  set; }
        
        public int? MaxSizeMB { get;  set; }
        
        public bool IsActive { get;  set; }
        
        public DateTime CreatedAt { get;  set; }
        
        public Guid CreatedBy { get;  set; }
        
        public DateTime? UpdatedAt { get;  set; }
        
        public Guid? UpdatedBy { get;  set; }
        
        public EvidenceTypeEntityDto() {}
        
    }
}

