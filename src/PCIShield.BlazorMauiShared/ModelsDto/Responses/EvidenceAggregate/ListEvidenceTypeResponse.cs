using System;
using System.Collections.Generic;
using PCIShield.BlazorMauiShared.Models;
using PCIShield.Domain.ModelsDto;

namespace BlazorMauiShared.Models.EvidenceType
{
    public class ListEvidenceTypeResponse : BaseResponse
    {
        public ListEvidenceTypeResponse(Guid correlationId)
        : base(correlationId)
        {
        }
        
        public ListEvidenceTypeResponse()
        {
        }
        
        public List<EvidenceTypeDto>? EvidenceTypes { get; set; } = new();
        
        public int Count { get; set; }
        public int TotalCount { get; set; }
        public int PageNumber { get; set; }
        public int PageSize { get; set; }
        public int TotalPages { get; set; }
    }
}

