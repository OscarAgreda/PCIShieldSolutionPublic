using System;
using PCIShield.BlazorMauiShared.Models;
using PCIShield.Domain.ModelsDto;

namespace BlazorMauiShared.Models.ServiceProvider
{
    public class UpdateServiceProviderResponse : BaseResponse
    {
        public UpdateServiceProviderResponse(Guid correlationId)
        : base(correlationId)
        {
        }
        
        public UpdateServiceProviderResponse()
        {
        }
        
        public ServiceProviderDto ServiceProvider { get; set; } = new();
    }
}

