using System;
using PCIShield.BlazorMauiShared.Models;

namespace BlazorMauiShared.Models.ControlEvidence
{
    public class GetByRelsIdsControlEvidenceRequest : BaseRequest
    {
        public Guid ControlId { get; set; }
        public Guid EvidenceId { get; set; }
        public Guid AssessmentId { get; set; }
    }
}

