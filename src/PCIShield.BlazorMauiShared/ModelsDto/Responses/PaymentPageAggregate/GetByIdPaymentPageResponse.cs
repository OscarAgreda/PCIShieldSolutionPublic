using System;
using PCIShield.BlazorMauiShared.Models;
using PCIShield.Domain.ModelsDto;

namespace BlazorMauiShared.Models.PaymentPage
{
    public class GetByIdPaymentPageResponse : BaseResponse
    {
        public GetByIdPaymentPageResponse(Guid correlationId)
        : base(correlationId)
        {
        }
        
        public GetByIdPaymentPageResponse()
        {
        }
        
        public PaymentPageDto PaymentPage { get; set; } = new();
    }
}

