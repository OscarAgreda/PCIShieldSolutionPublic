using System;
using PCIShield.BlazorMauiShared.Models;
using PCIShield.Domain.ModelsDto;

namespace BlazorMauiShared.Models.CompensatingControl
{
    public class CreateCompensatingControlResponse : BaseResponse
    {
        public CreateCompensatingControlResponse(Guid correlationId)
        : base(correlationId)
        {
        }
        
        public CreateCompensatingControlResponse()
        {
        }
        
        public CompensatingControlDto CompensatingControl { get; set; } = new();
    }
}

