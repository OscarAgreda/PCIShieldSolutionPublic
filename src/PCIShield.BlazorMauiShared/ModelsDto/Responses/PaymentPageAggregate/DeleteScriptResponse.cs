using System;
using PCIShield.BlazorMauiShared.Models;
using PCIShield.Domain.ModelsDto;

namespace BlazorMauiShared.Models.Script
{
    public class DeleteScriptResponse : BaseResponse
    {
        public DeleteScriptResponse(Guid correlationId)
        : base(correlationId)
        {
        }
        
        public DeleteScriptResponse()
        {
        }
        
    }
}

