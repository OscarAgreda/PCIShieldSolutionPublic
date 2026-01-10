using System;
using PCIShield.BlazorMauiShared.Models;
using PCIShield.Domain.ModelsDto;

namespace BlazorMauiShared.Models.NetworkSegmentation
{
    public class GetByIdNetworkSegmentationResponse : BaseResponse
    {
        public GetByIdNetworkSegmentationResponse(Guid correlationId)
        : base(correlationId)
        {
        }
        
        public GetByIdNetworkSegmentationResponse()
        {
        }
        
        public NetworkSegmentationDto NetworkSegmentation { get; set; } = new();
    }
}

