using System;
using PCIShield.BlazorMauiShared.Models;

namespace BlazorMauiShared.Models.AssetControl
{
    public class DeleteAssetControlRequest : BaseRequest
    {
        public int RowId { get; set; }
        public Guid AssetId { get; set; }
        public Guid ControlId { get; set; }
    }
}

