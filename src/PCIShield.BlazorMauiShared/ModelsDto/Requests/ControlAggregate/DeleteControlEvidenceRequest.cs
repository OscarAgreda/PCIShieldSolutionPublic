using System;
using PCIShield.BlazorMauiShared.Models;

namespace BlazorMauiShared.Models.ControlEvidence
{
    public class DeleteControlEvidenceRequest : BaseRequest
    {
        public int RowId { get; set; }
        public Guid ControlId { get; set; }
        public Guid EvidenceId { get; set; }
        public Guid AssessmentId { get; set; }
    }
}

