using System;
using PCIShield.BlazorMauiShared.Models;

namespace BlazorMauiShared.Models.ScanSchedule
{
    public class GetByIdScanScheduleRequest : BaseRequest
    {
        public Guid AssetId { get; set; }
        public Guid ScanScheduleId { get; set; }
        public bool WithPostGraph { get; set; }
    }
}

