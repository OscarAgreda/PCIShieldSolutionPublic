using System;
using PCIShield.BlazorMauiShared.Models;
using PCIShield.Domain.ModelsDto;

namespace BlazorMauiShared.Models.ScanSchedule
{
    public class CreateScanScheduleResponse : BaseResponse
    {
        public CreateScanScheduleResponse(Guid correlationId)
        : base(correlationId)
        {
        }
        
        public CreateScanScheduleResponse()
        {
        }
        
        public ScanScheduleDto ScanSchedule { get; set; } = new();
    }
}

