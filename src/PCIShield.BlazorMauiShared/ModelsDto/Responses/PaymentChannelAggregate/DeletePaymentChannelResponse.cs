using System;
using PCIShield.BlazorMauiShared.Models;
using PCIShield.Domain.ModelsDto;

namespace BlazorMauiShared.Models.PaymentChannel
{
    public class DeletePaymentChannelResponse : BaseResponse
    {
        public DeletePaymentChannelResponse(Guid correlationId)
        : base(correlationId)
        {
        }
        
        public DeletePaymentChannelResponse()
        {
        }
        
    }
}

