using System;
using PCIShield.BlazorMauiShared.Models;
using PCIShield.Domain.ModelsDto;

namespace BlazorMauiShared.Models.ServiceProvider
{
    public class DeleteServiceProviderResponse : BaseResponse
    {
        public DeleteServiceProviderResponse(Guid correlationId)
        : base(correlationId)
        {
        }
        
        public DeleteServiceProviderResponse()
        {
        }
        
    }
}

