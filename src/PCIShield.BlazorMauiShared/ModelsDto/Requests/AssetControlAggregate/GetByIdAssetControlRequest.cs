using System;
using PCIShield.BlazorMauiShared.Models;

namespace BlazorMauiShared.Models.AssetControl
{
    public class GetByIdAssetControlRequest : BaseRequest
    {
        public Guid AssetId { get; set; }
        public Guid ControlId { get; set; }
        public int RowId { get; set; }
        public bool WithPostGraph { get; set; }
    }
}

