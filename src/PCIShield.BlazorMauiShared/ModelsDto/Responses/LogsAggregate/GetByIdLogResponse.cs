using System;
using PCIShield.BlazorMauiShared.Models;
using PCIShield.Domain.ModelsDto;

namespace BlazorMauiShared.Models.Logs
{
    public class GetByIdLogsResponse : BaseResponse
    {
        public GetByIdLogsResponse(Guid correlationId)
        : base(correlationId)
        {
        }
        
        public GetByIdLogsResponse()
        {
        }
        
        public LogsDto Logs { get; set; } = new();
    }
}

