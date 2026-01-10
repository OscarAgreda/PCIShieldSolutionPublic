using System;
using System.Collections.Generic;
using PCIShield.BlazorMauiShared.Models;
using PCIShield.Domain.ModelsDto;

namespace BlazorMauiShared.Models.Merchant
{
    public class ListMerchantResponse : BaseResponse
    {
        public ListMerchantResponse(Guid correlationId)
        : base(correlationId)
        {
        }
        
        public ListMerchantResponse()
        {
        }
        
        public List<MerchantDto>? Merchants { get; set; } = new();
        
        public int Count { get; set; }
        public int TotalCount { get; set; }
        public int PageNumber { get; set; }
        public int PageSize { get; set; }
        public int TotalPages { get; set; }
    }
}

