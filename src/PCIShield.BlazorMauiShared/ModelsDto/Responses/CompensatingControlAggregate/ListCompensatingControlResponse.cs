using System;
using System.Collections.Generic;
using PCIShield.BlazorMauiShared.Models;
using PCIShield.Domain.ModelsDto;

namespace BlazorMauiShared.Models.CompensatingControl
{
    public class ListCompensatingControlResponse : BaseResponse
    {
        public ListCompensatingControlResponse(Guid correlationId)
        : base(correlationId)
        {
        }
        
        public ListCompensatingControlResponse()
        {
        }
        
        public List<CompensatingControlDto>? CompensatingControls { get; set; } = new();
        
        public int Count { get; set; }
        public int TotalCount { get; set; }
        public int PageNumber { get; set; }
        public int PageSize { get; set; }
        public int TotalPages { get; set; }
    }
}

