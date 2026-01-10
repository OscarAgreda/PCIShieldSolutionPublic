using System;
using PCIShield.BlazorMauiShared.Models;
using PCIShield.Domain.ModelsDto;

namespace BlazorMauiShared.Models.CompensatingControl
{
    public class GetByIdCompensatingControlResponse : BaseResponse
    {
        public GetByIdCompensatingControlResponse(Guid correlationId)
        : base(correlationId)
        {
        }
        
        public GetByIdCompensatingControlResponse()
        {
        }
        
        public CompensatingControlDto CompensatingControl { get; set; } = new();
    }
}

