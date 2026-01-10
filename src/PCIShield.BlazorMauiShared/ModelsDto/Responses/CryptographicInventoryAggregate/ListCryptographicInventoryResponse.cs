using System;
using System.Collections.Generic;
using PCIShield.BlazorMauiShared.Models;
using PCIShield.Domain.ModelsDto;

namespace BlazorMauiShared.Models.CryptographicInventory
{
    public class ListCryptographicInventoryResponse : BaseResponse
    {
        public ListCryptographicInventoryResponse(Guid correlationId)
        : base(correlationId)
        {
        }
        
        public ListCryptographicInventoryResponse()
        {
        }
        
        public List<CryptographicInventoryDto>? CryptographicInventories { get; set; } = new();
        
        public int Count { get; set; }
        public int TotalCount { get; set; }
        public int PageNumber { get; set; }
        public int PageSize { get; set; }
        public int TotalPages { get; set; }
    }
}

