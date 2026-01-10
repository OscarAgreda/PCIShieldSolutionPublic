using System;
using System.Collections.Generic;
using PCIShield.BlazorMauiShared.Models;
using PCIShield.Domain.ModelsDto;

namespace BlazorMauiShared.Models.NetworkSegmentation
{
    public class ListNetworkSegmentationResponse : BaseResponse
    {
        public ListNetworkSegmentationResponse(Guid correlationId)
        : base(correlationId)
        {
        }
        
        public ListNetworkSegmentationResponse()
        {
        }
        
        public List<NetworkSegmentationDto>? NetworkSegmentations { get; set; } = new();
        
        public int Count { get; set; }
        public int TotalCount { get; set; }
        public int PageNumber { get; set; }
        public int PageSize { get; set; }
        public int TotalPages { get; set; }
    }
}

