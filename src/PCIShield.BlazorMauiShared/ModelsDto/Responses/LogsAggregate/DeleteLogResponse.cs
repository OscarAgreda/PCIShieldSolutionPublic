using System;
using PCIShield.BlazorMauiShared.Models;
using PCIShield.Domain.ModelsDto;

namespace BlazorMauiShared.Models.Logs
{
    public class DeleteLogsResponse : BaseResponse
    {
        public DeleteLogsResponse(Guid correlationId)
        : base(correlationId)
        {
        }
        
        public DeleteLogsResponse()
        {
        }
        
    }
}

