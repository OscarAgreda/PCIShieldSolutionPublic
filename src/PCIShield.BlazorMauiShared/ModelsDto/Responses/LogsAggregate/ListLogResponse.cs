using System;
using System.Collections.Generic;
using PCIShield.BlazorMauiShared.Models;
using PCIShield.Domain.ModelsDto;

namespace BlazorMauiShared.Models.Logs
{
    public class ListLogsResponse : BaseResponse
    {
        public ListLogsResponse(Guid correlationId)
        : base(correlationId)
        {
        }
        
        public ListLogsResponse()
        {
        }
        
        public List<LogsDto>? Logs { get; set; } = new();
        
        public int Count { get; set; }
        public int TotalCount { get; set; }
        public int PageNumber { get; set; }
        public int PageSize { get; set; }
        public int TotalPages { get; set; }
    }
}

