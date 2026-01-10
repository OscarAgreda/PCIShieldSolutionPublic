using System;
using PCIShield.BlazorMauiShared.Models;
using PCIShield.Domain.ModelsDto;

namespace BlazorMauiShared.Models.Merchant
{
    public class UpdateMerchantResponse : BaseResponse
    {
        public UpdateMerchantResponse(Guid correlationId)
        : base(correlationId)
        {
        }
        
        public UpdateMerchantResponse()
        {
        }
        
        public MerchantDto Merchant { get; set; } = new();
    }
}

