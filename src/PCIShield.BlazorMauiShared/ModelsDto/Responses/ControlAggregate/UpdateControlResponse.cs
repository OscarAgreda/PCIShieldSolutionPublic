using System;
using PCIShield.BlazorMauiShared.Models;
using PCIShield.Domain.ModelsDto;

namespace BlazorMauiShared.Models.Control
{
    public class UpdateControlResponse : BaseResponse
    {
        public UpdateControlResponse(Guid correlationId)
        : base(correlationId)
        {
        }
        
        public UpdateControlResponse()
        {
        }
        
        public ControlDto Control { get; set; } = new();
    }
}

