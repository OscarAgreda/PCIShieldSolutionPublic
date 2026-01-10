using System;
using PCIShield.BlazorMauiShared.Models;
using PCIShield.Domain.ModelsDto;

namespace BlazorMauiShared.Models.CryptographicInventory
{
    public class GetByIdCryptographicInventoryResponse : BaseResponse
    {
        public GetByIdCryptographicInventoryResponse(Guid correlationId)
        : base(correlationId)
        {
        }
        
        public GetByIdCryptographicInventoryResponse()
        {
        }
        
        public CryptographicInventoryDto CryptographicInventory { get; set; } = new();
    }
}

