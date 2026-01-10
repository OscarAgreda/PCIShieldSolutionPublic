using System;
using PCIShield.BlazorMauiShared.Models;

namespace BlazorMauiShared.Models.Evidence
{
    public class GetByIdEvidenceRequest : BaseRequest
    {
        public Guid MerchantId { get; set; }
        public Guid EvidenceId { get; set; }
        public bool WithPostGraph { get; set; }
    }
}

