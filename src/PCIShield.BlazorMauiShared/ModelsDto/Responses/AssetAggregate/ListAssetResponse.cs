using System;
using System.Collections.Generic;
using PCIShield.BlazorMauiShared.Models;
using PCIShield.Domain.ModelsDto;

namespace BlazorMauiShared.Models.Asset
{
    public class ListAssetResponse : BaseResponse
    {
        public ListAssetResponse(Guid correlationId)
        : base(correlationId)
        {
        }
        
        public ListAssetResponse()
        {
        }
        
        public List<AssetDto>? Assets { get; set; } = new();
        
        public int Count { get; set; }
        public int TotalCount { get; set; }
        public int PageNumber { get; set; }
        public int PageSize { get; set; }
        public int TotalPages { get; set; }
    }
}

