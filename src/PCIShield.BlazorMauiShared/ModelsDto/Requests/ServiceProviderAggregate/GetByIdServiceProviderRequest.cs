using System;
using PCIShield.BlazorMauiShared.Models;

namespace BlazorMauiShared.Models.ServiceProvider
{
    public class GetByIdServiceProviderRequest : BaseRequest
    {
        public Guid MerchantId { get; set; }
        public Guid ServiceProviderId { get; set; }
        public bool WithPostGraph { get; set; }
    }
}

