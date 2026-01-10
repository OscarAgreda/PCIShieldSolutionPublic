using System;
using PCIShield.BlazorMauiShared.Models;
using PCIShield.Domain.ModelsDto;

namespace BlazorMauiShared.Models.PaymentPage
{
    public class CreatePaymentPageResponse : BaseResponse
    {
        public CreatePaymentPageResponse(Guid correlationId)
        : base(correlationId)
        {
        }
        
        public CreatePaymentPageResponse()
        {
        }
        
        public PaymentPageDto PaymentPage { get; set; } = new();
    }
}

