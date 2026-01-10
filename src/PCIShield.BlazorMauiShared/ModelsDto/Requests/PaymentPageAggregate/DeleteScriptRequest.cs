using System;
using PCIShield.BlazorMauiShared.Models;

namespace BlazorMauiShared.Models.Script
{
    public class DeleteScriptRequest : BaseRequest
    {
        public Guid ScriptId { get; set; }
        public Guid PaymentPageId { get; set; }
    }
}

