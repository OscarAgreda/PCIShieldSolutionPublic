using System;
using PCIShield.BlazorMauiShared.Models;

namespace BlazorMauiShared.Models.PaymentPage
{
    public class DeletePaymentPageRequest : BaseRequest
    {
        public Guid PaymentPageId { get; set; }
        public Guid PaymentChannelId { get; set; }
    }
}

