using System;
using PCIShield.BlazorMauiShared.Models;
using PCIShield.Domain.ModelsDto;

namespace BlazorMauiShared.Models.ServiceProvider
{
    public class CreateServiceProviderResponse : BaseResponse
    {
        public CreateServiceProviderResponse(Guid correlationId)
        : base(correlationId)
        {
        }
        
        public CreateServiceProviderResponse()
        {
        }
        
        public ServiceProviderDto ServiceProvider { get; set; } = new();
    }
}

