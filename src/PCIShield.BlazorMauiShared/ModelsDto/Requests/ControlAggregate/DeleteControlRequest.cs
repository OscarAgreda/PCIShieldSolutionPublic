using System;
using PCIShield.BlazorMauiShared.Models;

namespace BlazorMauiShared.Models.Control
{
    public class DeleteControlRequest : BaseRequest
    {
        public Guid ControlId { get; set; }
    }
}

