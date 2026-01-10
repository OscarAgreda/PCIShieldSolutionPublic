using System;
using PCIShield.BlazorMauiShared.Models;
using PCIShield.Domain.ModelsDto;

namespace BlazorMauiShared.Models.ScanSchedule
{
    public class UpdateScanScheduleResponse : BaseResponse
    {
        public UpdateScanScheduleResponse(Guid correlationId)
        : base(correlationId)
        {
        }
        
        public UpdateScanScheduleResponse()
        {
        }
        
        public ScanScheduleDto ScanSchedule { get; set; } = new();
    }
}

