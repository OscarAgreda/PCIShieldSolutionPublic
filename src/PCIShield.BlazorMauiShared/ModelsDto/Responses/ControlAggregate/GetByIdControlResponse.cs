using System;
using PCIShield.BlazorMauiShared.Models;
using PCIShield.Domain.ModelsDto;

namespace BlazorMauiShared.Models.Control
{
    public class GetByIdControlResponse : BaseResponse
    {
        public GetByIdControlResponse(Guid correlationId)
        : base(correlationId)
        {
        }
        
        public GetByIdControlResponse()
        {
        }
        
        public ControlDto Control { get; set; } = new();
    }
}

