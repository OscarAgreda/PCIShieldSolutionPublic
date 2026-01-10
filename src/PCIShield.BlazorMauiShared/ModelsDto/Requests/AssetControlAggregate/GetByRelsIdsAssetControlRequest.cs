using System;
using PCIShield.BlazorMauiShared.Models;

namespace BlazorMauiShared.Models.AssetControl
{
    public class GetByRelsIdsAssetControlRequest : BaseRequest
    {
        public Guid AssetId { get; set; }
        public Guid ControlId { get; set; }
    }
}

