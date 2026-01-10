using System;
using PCIShield.BlazorMauiShared.Models;
using PCIShield.Domain.ModelsDto;

namespace BlazorMauiShared.Models.PaymentChannel
{
    public class GetByIdPaymentChannelResponse : BaseResponse
    {
        public GetByIdPaymentChannelResponse(Guid correlationId)
        : base(correlationId)
        {
        }
        
        public GetByIdPaymentChannelResponse()
        {
        }
        
        public PaymentChannelDto PaymentChannel { get; set; } = new();
    }
}

