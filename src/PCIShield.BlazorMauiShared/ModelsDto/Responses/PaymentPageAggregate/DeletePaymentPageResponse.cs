using System;
using PCIShield.BlazorMauiShared.Models;
using PCIShield.Domain.ModelsDto;

namespace BlazorMauiShared.Models.PaymentPage
{
    public class DeletePaymentPageResponse : BaseResponse
    {
        public DeletePaymentPageResponse(Guid correlationId)
        : base(correlationId)
        {
        }
        
        public DeletePaymentPageResponse()
        {
        }
        
    }
}

