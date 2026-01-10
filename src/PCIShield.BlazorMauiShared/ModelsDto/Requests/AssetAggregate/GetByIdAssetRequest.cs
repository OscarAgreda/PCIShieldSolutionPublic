using System;
using PCIShield.BlazorMauiShared.Models;

namespace BlazorMauiShared.Models.Asset
{
    public class GetByIdAssetRequest : BaseRequest
    {
        public Guid MerchantId { get; set; }
        public Guid AssetId { get; set; }
        public bool WithPostGraph { get; set; }
    }
}

