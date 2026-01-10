using System;
using PCIShield.BlazorMauiShared.Models;
using PCIShield.Domain.ModelsDto;

namespace BlazorMauiShared.Models.PaymentPage
{
    public class UpdatePaymentPageResponse : BaseResponse
    {
        public UpdatePaymentPageResponse(Guid correlationId)
        : base(correlationId)
        {
        }
        
        public UpdatePaymentPageResponse()
        {
        }
        
        public PaymentPageDto PaymentPage { get; set; } = new();
    }
}

