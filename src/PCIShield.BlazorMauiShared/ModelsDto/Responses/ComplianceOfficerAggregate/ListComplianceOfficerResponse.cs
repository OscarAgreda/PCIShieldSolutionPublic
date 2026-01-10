using System;
using System.Collections.Generic;
using PCIShield.BlazorMauiShared.Models;
using PCIShield.Domain.ModelsDto;

namespace BlazorMauiShared.Models.ComplianceOfficer
{
    public class ListComplianceOfficerResponse : BaseResponse
    {
        public ListComplianceOfficerResponse(Guid correlationId)
        : base(correlationId)
        {
        }
        
        public ListComplianceOfficerResponse()
        {
        }
        
        public List<ComplianceOfficerDto>? ComplianceOfficers { get; set; } = new();
        
        public int Count { get; set; }
        public int TotalCount { get; set; }
        public int PageNumber { get; set; }
        public int PageSize { get; set; }
        public int TotalPages { get; set; }
    }
}

