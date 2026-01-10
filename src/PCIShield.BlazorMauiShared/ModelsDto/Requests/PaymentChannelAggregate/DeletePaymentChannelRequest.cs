using System;
using PCIShield.BlazorMauiShared.Models;

namespace BlazorMauiShared.Models.PaymentChannel
{
    public class DeletePaymentChannelRequest : BaseRequest
    {
        public Guid PaymentChannelId { get; set; }
        public Guid MerchantId { get; set; }
    }
}

