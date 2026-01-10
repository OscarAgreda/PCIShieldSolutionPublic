using System;
using PCIShield.BlazorMauiShared.Models;
using PCIShield.Domain.ModelsDto;

namespace BlazorMauiShared.Models.ServiceProvider
{
    public class GetByIdServiceProviderResponse : BaseResponse
    {
        public GetByIdServiceProviderResponse(Guid correlationId)
        : base(correlationId)
        {
        }
        
        public GetByIdServiceProviderResponse()
        {
        }
        
        public ServiceProviderDto ServiceProvider { get; set; } = new();
    }
}

