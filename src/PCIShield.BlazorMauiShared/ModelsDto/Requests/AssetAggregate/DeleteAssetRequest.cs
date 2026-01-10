using System;
using PCIShield.BlazorMauiShared.Models;

namespace BlazorMauiShared.Models.Asset
{
    public class DeleteAssetRequest : BaseRequest
    {
        public Guid AssetId { get; set; }
        public Guid MerchantId { get; set; }
    }
}

