using System;
using PCIShield.BlazorMauiShared.Models;
using PCIShield.Domain.ModelsDto;

namespace BlazorMauiShared.Models.CompensatingControl
{
    public class DeleteCompensatingControlResponse : BaseResponse
    {
        public DeleteCompensatingControlResponse(Guid correlationId)
        : base(correlationId)
        {
        }
        
        public DeleteCompensatingControlResponse()
        {
        }
        
    }
}

