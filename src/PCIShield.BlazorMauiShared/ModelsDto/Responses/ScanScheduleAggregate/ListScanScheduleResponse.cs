using System;
using System.Collections.Generic;
using PCIShield.BlazorMauiShared.Models;
using PCIShield.Domain.ModelsDto;

namespace BlazorMauiShared.Models.ScanSchedule
{
    public class ListScanScheduleResponse : BaseResponse
    {
        public ListScanScheduleResponse(Guid correlationId)
        : base(correlationId)
        {
        }
        
        public ListScanScheduleResponse()
        {
        }
        
        public List<ScanScheduleDto>? ScanSchedules { get; set; } = new();
        
        public int Count { get; set; }
        public int TotalCount { get; set; }
        public int PageNumber { get; set; }
        public int PageSize { get; set; }
        public int TotalPages { get; set; }
    }
}

