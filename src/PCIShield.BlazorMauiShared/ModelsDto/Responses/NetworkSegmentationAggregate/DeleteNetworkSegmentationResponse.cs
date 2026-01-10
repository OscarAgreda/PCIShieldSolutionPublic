using System;
using PCIShield.BlazorMauiShared.Models;
using PCIShield.Domain.ModelsDto;

namespace BlazorMauiShared.Models.NetworkSegmentation
{
    public class DeleteNetworkSegmentationResponse : BaseResponse
    {
        public DeleteNetworkSegmentationResponse(Guid correlationId)
        : base(correlationId)
        {
        }
        
        public DeleteNetworkSegmentationResponse()
        {
        }
        
    }
}

