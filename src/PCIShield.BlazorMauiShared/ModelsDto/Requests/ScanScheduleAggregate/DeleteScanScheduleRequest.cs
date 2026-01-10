using System;
using PCIShield.BlazorMauiShared.Models;

namespace BlazorMauiShared.Models.ScanSchedule
{
    public class DeleteScanScheduleRequest : BaseRequest
    {
        public Guid ScanScheduleId { get; set; }
        public Guid AssetId { get; set; }
    }
}

