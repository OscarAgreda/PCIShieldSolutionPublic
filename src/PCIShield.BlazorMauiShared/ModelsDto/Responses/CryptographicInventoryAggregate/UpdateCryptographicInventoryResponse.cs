using System;
using PCIShield.BlazorMauiShared.Models;
using PCIShield.Domain.ModelsDto;

namespace BlazorMauiShared.Models.CryptographicInventory
{
    public class UpdateCryptographicInventoryResponse : BaseResponse
    {
        public UpdateCryptographicInventoryResponse(Guid correlationId)
        : base(correlationId)
        {
        }
        
        public UpdateCryptographicInventoryResponse()
        {
        }
        
        public CryptographicInventoryDto CryptographicInventory { get; set; } = new();
    }
}

