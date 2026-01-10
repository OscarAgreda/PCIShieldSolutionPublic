using System;
using PCIShield.BlazorMauiShared.Models;
using PCIShield.Domain.ModelsDto;

namespace BlazorMauiShared.Models.CryptographicInventory
{
    public class DeleteCryptographicInventoryResponse : BaseResponse
    {
        public DeleteCryptographicInventoryResponse(Guid correlationId)
        : base(correlationId)
        {
        }
        
        public DeleteCryptographicInventoryResponse()
        {
        }
        
    }
}

