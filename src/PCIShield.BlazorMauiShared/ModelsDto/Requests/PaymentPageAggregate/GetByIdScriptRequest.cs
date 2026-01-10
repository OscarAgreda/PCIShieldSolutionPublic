using System;
using PCIShield.BlazorMauiShared.Models;

namespace BlazorMauiShared.Models.Script
{
    public class GetByIdScriptRequest : BaseRequest
    {
        public Guid PaymentPageId { get; set; }
        public Guid ScriptId { get; set; }
        public bool WithPostGraph { get; set; }
    }
}

