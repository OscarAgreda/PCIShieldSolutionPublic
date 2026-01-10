using System;
using PCIShield.BlazorMauiShared.Models;

namespace BlazorMauiShared.Models.ServiceProvider
{
    public class DeleteServiceProviderRequest : BaseRequest
    {
        public Guid ServiceProviderId { get; set; }
        public Guid MerchantId { get; set; }
    }
}

