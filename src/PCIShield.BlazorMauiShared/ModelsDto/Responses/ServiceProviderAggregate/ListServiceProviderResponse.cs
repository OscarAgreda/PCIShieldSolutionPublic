using System;
using System.Collections.Generic;
using PCIShield.BlazorMauiShared.Models;
using PCIShield.Domain.ModelsDto;

namespace BlazorMauiShared.Models.ServiceProvider
{
    public class ListServiceProviderResponse : BaseResponse
    {
        public ListServiceProviderResponse(Guid correlationId)
        : base(correlationId)
        {
        }
        
        public ListServiceProviderResponse()
        {
        }
        
        public List<ServiceProviderDto>? ServiceProviders { get; set; } = new();
        
        public int Count { get; set; }
        public int TotalCount { get; set; }
        public int PageNumber { get; set; }
        public int PageSize { get; set; }
        public int TotalPages { get; set; }
    }
}

