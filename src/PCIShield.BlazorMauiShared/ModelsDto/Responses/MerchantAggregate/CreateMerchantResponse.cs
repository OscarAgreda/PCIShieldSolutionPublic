using System;
using PCIShield.BlazorMauiShared.Models;
using PCIShield.Domain.ModelsDto;

namespace BlazorMauiShared.Models.Merchant
{
    public class CreateMerchantResponse : BaseResponse
    {
        public CreateMerchantResponse(Guid correlationId)
        : base(correlationId)
        {
        }
        
        public CreateMerchantResponse()
        {
        }
        
        public MerchantDto Merchant { get; set; } = new();
    }
}

