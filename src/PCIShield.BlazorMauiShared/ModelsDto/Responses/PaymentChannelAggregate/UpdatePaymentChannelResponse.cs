using System;
using PCIShield.BlazorMauiShared.Models;
using PCIShield.Domain.ModelsDto;

namespace BlazorMauiShared.Models.PaymentChannel
{
    public class UpdatePaymentChannelResponse : BaseResponse
    {
        public UpdatePaymentChannelResponse(Guid correlationId)
        : base(correlationId)
        {
        }
        
        public UpdatePaymentChannelResponse()
        {
        }
        
        public PaymentChannelDto PaymentChannel { get; set; } = new();
    }
}

