using System;
using PCIShield.BlazorMauiShared.Models;
using PCIShield.Domain.ModelsDto;

namespace BlazorMauiShared.Models.Script
{
    public class UpdateScriptResponse : BaseResponse
    {
        public UpdateScriptResponse(Guid correlationId)
        : base(correlationId)
        {
        }
        
        public UpdateScriptResponse()
        {
        }
        
        public ScriptDto Script { get; set; } = new();
    }
}

