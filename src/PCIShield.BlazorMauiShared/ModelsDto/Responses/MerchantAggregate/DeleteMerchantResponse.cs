using System;
using PCIShield.BlazorMauiShared.Models;
using PCIShield.Domain.ModelsDto;

namespace BlazorMauiShared.Models.Merchant
{
    public class DeleteMerchantResponse : BaseResponse
    {
        public DeleteMerchantResponse(Guid correlationId)
        : base(correlationId)
        {
        }
        
        public DeleteMerchantResponse()
        {
        }
        
    }
}

