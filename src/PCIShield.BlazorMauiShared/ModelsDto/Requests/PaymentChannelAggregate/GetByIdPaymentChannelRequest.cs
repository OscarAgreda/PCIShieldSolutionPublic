using System;
using PCIShield.BlazorMauiShared.Models;

namespace BlazorMauiShared.Models.PaymentChannel
{
    public class GetByIdPaymentChannelRequest : BaseRequest
    {
        public Guid MerchantId { get; set; }
        public Guid PaymentChannelId { get; set; }
        public bool WithPostGraph { get; set; }
    }
}

