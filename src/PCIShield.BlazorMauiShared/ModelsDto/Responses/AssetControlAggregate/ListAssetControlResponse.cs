using System;
using System.Collections.Generic;
using PCIShield.BlazorMauiShared.Models;
using PCIShield.Domain.ModelsDto;

namespace BlazorMauiShared.Models.AssetControl
{
    public class ListAssetControlResponse : BaseResponse
    {
        public ListAssetControlResponse(Guid correlationId)
        : base(correlationId)
        {
        }
        
        public ListAssetControlResponse()
        {
        }
        
        public List<AssetControlDto>? AssetControls { get; set; } = new();
        
        public int Count { get; set; }
        public int TotalCount { get; set; }
        public int PageNumber { get; set; }
        public int PageSize { get; set; }
        public int TotalPages { get; set; }
    }
}

