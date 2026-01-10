using System;
using PCIShield.BlazorMauiShared.Models;
using PCIShield.Domain.ModelsDto;

namespace BlazorMauiShared.Models.PaymentChannel
{
    public class CreatePaymentChannelResponse : BaseResponse
    {
        public CreatePaymentChannelResponse(Guid correlationId)
        : base(correlationId)
        {
        }
        
        public CreatePaymentChannelResponse()
        {
        }
        
        public PaymentChannelDto PaymentChannel { get; set; } = new();
    }
}

