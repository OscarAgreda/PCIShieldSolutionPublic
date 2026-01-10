using System;
using System.Collections.Generic;
using System.Linq;
using PCIShieldLib.SharedKernel;
using PCIShieldLib.SharedKernel.Interfaces;
using Ardalis.GuardClauses;

namespace PCIShield.Domain.ModelEntityDto
{
    
    public class LogsEntityDto : IEntityDto
    {
        public int Id { get;  set; }
        
        public string? Message { get;  set; }
        
        public string? MessageTemplate { get;  set; }
        
        public string? Level { get;  set; }
        
        public string? Exception { get;  set; }
        
        public string? Properties { get;  set; }
        
        public LogsEntityDto() {}
        
    }
}

