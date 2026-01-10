using System;
using PCIShield.BlazorMauiShared.Models;

namespace BlazorMauiShared.Models.NetworkSegmentation
{
    public class DeleteNetworkSegmentationRequest : BaseRequest
    {
        public Guid NetworkSegmentationId { get; set; }
        public Guid MerchantId { get; set; }
    }
}

