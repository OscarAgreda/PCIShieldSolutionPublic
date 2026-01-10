using System;
using PCIShield.BlazorMauiShared.Models;

namespace BlazorMauiShared.Models.ControlEvidence
{
    public class GetByIdControlEvidenceRequest : BaseRequest
    {
        public Guid AssessmentId { get; set; }
        public Guid ControlId { get; set; }
        public Guid EvidenceId { get; set; }
        public int RowId { get; set; }
        public bool WithPostGraph { get; set; }
    }
}

