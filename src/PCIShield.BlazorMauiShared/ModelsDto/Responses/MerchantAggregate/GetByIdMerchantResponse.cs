using System;
using PCIShield.BlazorMauiShared.Models;
using PCIShield.Domain.ModelsDto;

namespace BlazorMauiShared.Models.Merchant
{
    public class GetByIdMerchantResponse : BaseResponse
    {
        public GetByIdMerchantResponse(Guid correlationId)
        : base(correlationId)
        {
        }
        
        public GetByIdMerchantResponse()
        {
        }
        
        public MerchantDto Merchant { get; set; } = new();
    }
}

