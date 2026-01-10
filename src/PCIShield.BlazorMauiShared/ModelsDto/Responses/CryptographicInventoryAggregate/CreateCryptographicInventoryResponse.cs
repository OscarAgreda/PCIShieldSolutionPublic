using System;
using PCIShield.BlazorMauiShared.Models;
using PCIShield.Domain.ModelsDto;

namespace BlazorMauiShared.Models.CryptographicInventory
{
    public class CreateCryptographicInventoryResponse : BaseResponse
    {
        public CreateCryptographicInventoryResponse(Guid correlationId)
        : base(correlationId)
        {
        }
        
        public CreateCryptographicInventoryResponse()
        {
        }
        
        public CryptographicInventoryDto CryptographicInventory { get; set; } = new();
    }
}

