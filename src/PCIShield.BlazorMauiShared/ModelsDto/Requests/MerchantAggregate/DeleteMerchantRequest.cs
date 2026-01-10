using System;
using PCIShield.BlazorMauiShared.Models;

namespace BlazorMauiShared.Models.Merchant
{
    public class DeleteMerchantRequest : BaseRequest
    {
        public Guid MerchantId { get; set; }
    }
}

