using System;
using PCIShield.BlazorMauiShared.Models;

namespace BlazorMauiShared.Models.CryptographicInventory
{
    public class DeleteCryptographicInventoryRequest : BaseRequest
    {
        public Guid CryptographicInventoryId { get; set; }
        public Guid MerchantId { get; set; }
    }
}

