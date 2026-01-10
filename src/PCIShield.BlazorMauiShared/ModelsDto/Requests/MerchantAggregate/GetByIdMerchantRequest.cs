using System;
using PCIShield.BlazorMauiShared.Models;

namespace BlazorMauiShared.Models.Merchant
{
    public class GetByIdMerchantRequest : BaseRequest
    {
        public Guid MerchantId { get; set; }
        public bool WithPostGraph { get; set; }
    }
}

