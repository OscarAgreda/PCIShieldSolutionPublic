using System;
using PCIShield.BlazorMauiShared.Models;
using PCIShield.Domain.ModelsDto;

namespace BlazorMauiShared.Models.ScanSchedule
{
    public class GetByIdScanScheduleResponse : BaseResponse
    {
        public GetByIdScanScheduleResponse(Guid correlationId)
        : base(correlationId)
        {
        }
        
        public GetByIdScanScheduleResponse()
        {
        }
        
        public ScanScheduleDto ScanSchedule { get; set; } = new();
    }
}

