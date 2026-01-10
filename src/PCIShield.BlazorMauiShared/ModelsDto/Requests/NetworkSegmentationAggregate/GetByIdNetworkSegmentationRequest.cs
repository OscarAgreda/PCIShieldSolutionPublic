using System;
using PCIShield.BlazorMauiShared.Models;

namespace BlazorMauiShared.Models.NetworkSegmentation
{
    public class GetByIdNetworkSegmentationRequest : BaseRequest
    {
        public Guid MerchantId { get; set; }
        public Guid NetworkSegmentationId { get; set; }
        public bool WithPostGraph { get; set; }
    }
}

