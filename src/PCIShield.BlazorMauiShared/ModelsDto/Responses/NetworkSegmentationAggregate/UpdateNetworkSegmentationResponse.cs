using System;
using PCIShield.BlazorMauiShared.Models;
using PCIShield.Domain.ModelsDto;

namespace BlazorMauiShared.Models.NetworkSegmentation
{
    public class UpdateNetworkSegmentationResponse : BaseResponse
    {
        public UpdateNetworkSegmentationResponse(Guid correlationId)
        : base(correlationId)
        {
        }
        
        public UpdateNetworkSegmentationResponse()
        {
        }
        
        public NetworkSegmentationDto NetworkSegmentation { get; set; } = new();
    }
}

