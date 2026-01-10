using System;
using PCIShield.BlazorMauiShared.Models;
using PCIShield.Domain.ModelsDto;

namespace BlazorMauiShared.Models.NetworkSegmentation
{
    public class CreateNetworkSegmentationResponse : BaseResponse
    {
        public CreateNetworkSegmentationResponse(Guid correlationId)
        : base(correlationId)
        {
        }
        
        public CreateNetworkSegmentationResponse()
        {
        }
        
        public NetworkSegmentationDto NetworkSegmentation { get; set; } = new();
    }
}

