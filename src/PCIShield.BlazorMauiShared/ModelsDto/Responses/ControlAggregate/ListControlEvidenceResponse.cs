using System;
using System.Collections.Generic;
using PCIShield.BlazorMauiShared.Models;
using PCIShield.Domain.ModelsDto;

namespace BlazorMauiShared.Models.ControlEvidence
{
    public class ListControlEvidenceResponse : BaseResponse
    {
        public ListControlEvidenceResponse(Guid correlationId)
        : base(correlationId)
        {
        }
        
        public ListControlEvidenceResponse()
        {
        }
        
        public List<ControlEvidenceDto>? ControlEvidences { get; set; } = new();
        
        public int Count { get; set; }
        public int TotalCount { get; set; }
        public int PageNumber { get; set; }
        public int PageSize { get; set; }
        public int TotalPages { get; set; }
    }
}

