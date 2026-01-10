using System;
using PCIShield.BlazorMauiShared.Models;

namespace BlazorMauiShared.Models.CompensatingControl
{
    public class GetByIdCompensatingControlRequest : BaseRequest
    {
        public Guid ControlId { get; set; }
        public Guid MerchantId { get; set; }
        public Guid CompensatingControlId { get; set; }
        public bool WithPostGraph { get; set; }
    }
}

