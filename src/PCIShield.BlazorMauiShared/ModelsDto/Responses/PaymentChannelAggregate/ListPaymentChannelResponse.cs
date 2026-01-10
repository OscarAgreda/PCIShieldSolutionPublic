using System;
using System.Collections.Generic;
using PCIShield.BlazorMauiShared.Models;
using PCIShield.Domain.ModelsDto;

namespace BlazorMauiShared.Models.PaymentChannel
{
    public class ListPaymentChannelResponse : BaseResponse
    {
        public ListPaymentChannelResponse(Guid correlationId)
        : base(correlationId)
        {
        }
        
        public ListPaymentChannelResponse()
        {
        }
        
        public List<PaymentChannelDto>? PaymentChannels { get; set; } = new();
        
        public int Count { get; set; }
        public int TotalCount { get; set; }
        public int PageNumber { get; set; }
        public int PageSize { get; set; }
        public int TotalPages { get; set; }
    }
}

