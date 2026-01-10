using System;
using System.Collections.Generic;
using PCIShield.BlazorMauiShared.Models;
using PCIShield.Domain.ModelsDto;

namespace BlazorMauiShared.Models.Control
{
    public class ListControlResponse : BaseResponse
    {
        public ListControlResponse(Guid correlationId)
        : base(correlationId)
        {
        }
        
        public ListControlResponse()
        {
        }
        
        public List<ControlDto>? Controls { get; set; } = new();
        
        public int Count { get; set; }
        public int TotalCount { get; set; }
        public int PageNumber { get; set; }
        public int PageSize { get; set; }
        public int TotalPages { get; set; }
    }
}

