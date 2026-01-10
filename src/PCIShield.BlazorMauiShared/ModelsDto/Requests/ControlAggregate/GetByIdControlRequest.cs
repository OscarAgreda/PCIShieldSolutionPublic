using System;
using PCIShield.BlazorMauiShared.Models;

namespace BlazorMauiShared.Models.Control
{
    public class GetByIdControlRequest : BaseRequest
    {
        public Guid ControlId { get; set; }
        public bool WithPostGraph { get; set; }
    }
}

