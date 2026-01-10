using System;
using PCIShield.BlazorMauiShared.Models;
using PCIShield.Domain.ModelsDto;

namespace BlazorMauiShared.Models.ScanSchedule
{
    public class DeleteScanScheduleResponse : BaseResponse
    {
        public DeleteScanScheduleResponse(Guid correlationId)
        : base(correlationId)
        {
        }
        
        public DeleteScanScheduleResponse()
        {
        }
        
    }
}

