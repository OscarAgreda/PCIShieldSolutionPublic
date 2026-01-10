using System;
using PCIShield.BlazorMauiShared.Models;

namespace BlazorMauiShared.Models.Evidence
{
    public class DeleteEvidenceRequest : BaseRequest
    {
        public Guid EvidenceId { get; set; }
        public Guid MerchantId { get; set; }
    }
}

