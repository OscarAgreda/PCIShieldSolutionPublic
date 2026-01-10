using System;
using PCIShield.BlazorMauiShared.Models;

namespace BlazorMauiShared.Models.CryptographicInventory
{
    public class GetByIdCryptographicInventoryRequest : BaseRequest
    {
        public Guid MerchantId { get; set; }
        public Guid CryptographicInventoryId { get; set; }
        public bool WithPostGraph { get; set; }
    }
}

