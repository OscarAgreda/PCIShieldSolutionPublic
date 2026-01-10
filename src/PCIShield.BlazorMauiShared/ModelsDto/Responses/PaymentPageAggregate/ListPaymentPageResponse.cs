using System;
using System.Collections.Generic;
using PCIShield.BlazorMauiShared.Models;
using PCIShield.Domain.ModelsDto;

namespace BlazorMauiShared.Models.PaymentPage
{
    public class ListPaymentPageResponse : BaseResponse
    {
        public ListPaymentPageResponse(Guid correlationId)
        : base(correlationId)
        {
        }
        
        public ListPaymentPageResponse()
        {
        }
        
        public List<PaymentPageDto>? PaymentPages { get; set; } = new();
        
        public int Count { get; set; }
        public int TotalCount { get; set; }
        public int PageNumber { get; set; }
        public int PageSize { get; set; }
        public int TotalPages { get; set; }
    }
}

