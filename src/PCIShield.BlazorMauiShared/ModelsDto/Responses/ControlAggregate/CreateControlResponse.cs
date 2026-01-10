using System;
using PCIShield.BlazorMauiShared.Models;
using PCIShield.Domain.ModelsDto;

namespace BlazorMauiShared.Models.Control
{
    public class CreateControlResponse : BaseResponse
    {
        public CreateControlResponse(Guid correlationId)
        : base(correlationId)
        {
        }
        
        public CreateControlResponse()
        {
        }
        
        public ControlDto Control { get; set; } = new();
    }
}

