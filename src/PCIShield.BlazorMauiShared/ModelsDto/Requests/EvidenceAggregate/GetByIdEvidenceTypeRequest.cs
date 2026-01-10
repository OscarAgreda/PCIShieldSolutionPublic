using System;
using PCIShield.BlazorMauiShared.Models;

namespace BlazorMauiShared.Models.EvidenceType
{
    public class GetByIdEvidenceTypeRequest : BaseRequest
    {
        public Guid EvidenceTypeId { get; set; }
        public bool WithPostGraph { get; set; }
    }
}

