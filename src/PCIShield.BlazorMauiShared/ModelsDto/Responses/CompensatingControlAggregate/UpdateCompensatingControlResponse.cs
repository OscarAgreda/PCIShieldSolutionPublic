using System;
using PCIShield.BlazorMauiShared.Models;
using PCIShield.Domain.ModelsDto;

namespace BlazorMauiShared.Models.CompensatingControl
{
    public class UpdateCompensatingControlResponse : BaseResponse
    {
        public UpdateCompensatingControlResponse(Guid correlationId)
        : base(correlationId)
        {
        }
        
        public UpdateCompensatingControlResponse()
        {
        }
        
        public CompensatingControlDto CompensatingControl { get; set; } = new();
    }
}

