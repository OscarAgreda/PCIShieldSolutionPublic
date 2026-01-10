using System;
using PCIShield.BlazorMauiShared.Models;

namespace BlazorMauiShared.Models.CompensatingControl
{
    public class DeleteCompensatingControlRequest : BaseRequest
    {
        public Guid CompensatingControlId { get; set; }
        public Guid ControlId { get; set; }
        public Guid MerchantId { get; set; }
    }
}

