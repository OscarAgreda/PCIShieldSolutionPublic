using System;
using PCIShield.BlazorMauiShared.Models;
using PCIShield.Domain.ModelsDto;

namespace BlazorMauiShared.Models.Control
{
    public class DeleteControlResponse : BaseResponse
    {
        public DeleteControlResponse(Guid correlationId)
        : base(correlationId)
        {
        }
        
        public DeleteControlResponse()
        {
        }
        
    }
}

