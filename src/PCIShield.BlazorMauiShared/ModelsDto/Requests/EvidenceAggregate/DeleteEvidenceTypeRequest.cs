using System;
using PCIShield.BlazorMauiShared.Models;

namespace BlazorMauiShared.Models.EvidenceType
{
    public class DeleteEvidenceTypeRequest : BaseRequest
    {
        public Guid EvidenceTypeId { get; set; }
    }
}

