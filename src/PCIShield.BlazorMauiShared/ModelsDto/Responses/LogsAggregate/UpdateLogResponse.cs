using System;
using PCIShield.BlazorMauiShared.Models;
using PCIShield.Domain.ModelsDto;

namespace BlazorMauiShared.Models.Logs
{
    public class UpdateLogsResponse : BaseResponse
    {
        public UpdateLogsResponse(Guid correlationId)
        : base(correlationId)
        {
        }
        
        public UpdateLogsResponse()
        {
        }
        
        public LogsDto Logs { get; set; } = new();
    }
}

