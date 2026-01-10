using System;
using System.Collections.Generic;
using PCIShield.BlazorMauiShared.Models;
using PCIShield.Domain.ModelsDto;

namespace BlazorMauiShared.Models.Evidence
{
    public class ListEvidenceResponse : BaseResponse
    {
        public ListEvidenceResponse(Guid correlationId)
        : base(correlationId)
        {
        }
        
        public ListEvidenceResponse()
        {
        }
        
        public List<EvidenceDto>? Evidences { get; set; } = new();
        
        public int Count { get; set; }
        public int TotalCount { get; set; }
        public int PageNumber { get; set; }
        public int PageSize { get; set; }
        public int TotalPages { get; set; }
    }
}

