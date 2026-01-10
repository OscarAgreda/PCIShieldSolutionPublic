using System;
using PCIShield.BlazorMauiShared.Models;

namespace BlazorMauiShared.Models.PaymentPage
{
    public class GetByIdPaymentPageRequest : BaseRequest
    {
        public Guid PaymentChannelId { get; set; }
        public Guid PaymentPageId { get; set; }
        public bool WithPostGraph { get; set; }
    }
}

