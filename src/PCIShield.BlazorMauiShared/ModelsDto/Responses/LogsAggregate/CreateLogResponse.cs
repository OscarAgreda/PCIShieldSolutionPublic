using System;
using PCIShield.BlazorMauiShared.Models;
using PCIShield.Domain.ModelsDto;

namespace BlazorMauiShared.Models.Logs
{
    public class CreateLogsResponse : BaseResponse
    {
        public CreateLogsResponse(Guid correlationId)
        : base(correlationId)
        {
        }
        
        public CreateLogsResponse()
        {
        }
        
        public LogsDto Logs { get; set; } = new();
    }
}

