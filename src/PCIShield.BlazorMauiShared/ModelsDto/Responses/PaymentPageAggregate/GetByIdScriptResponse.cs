using System;
using PCIShield.BlazorMauiShared.Models;
using PCIShield.Domain.ModelsDto;

namespace BlazorMauiShared.Models.Script
{
    public class GetByIdScriptResponse : BaseResponse
    {
        public GetByIdScriptResponse(Guid correlationId)
        : base(correlationId)
        {
        }
        
        public GetByIdScriptResponse()
        {
        }
        
        public ScriptDto Script { get; set; } = new();
    }
}

