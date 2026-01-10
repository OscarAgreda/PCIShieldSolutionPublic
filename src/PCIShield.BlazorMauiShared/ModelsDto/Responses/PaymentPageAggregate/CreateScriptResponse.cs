using System;
using PCIShield.BlazorMauiShared.Models;
using PCIShield.Domain.ModelsDto;

namespace BlazorMauiShared.Models.Script
{
    public class CreateScriptResponse : BaseResponse
    {
        public CreateScriptResponse(Guid correlationId)
        : base(correlationId)
        {
        }
        
        public CreateScriptResponse()
        {
        }
        
        public ScriptDto Script { get; set; } = new();
    }
}

