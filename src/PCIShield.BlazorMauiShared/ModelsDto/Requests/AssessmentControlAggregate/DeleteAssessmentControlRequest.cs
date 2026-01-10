using System;
using PCIShield.BlazorMauiShared.Models;

namespace BlazorMauiShared.Models.AssessmentControl
{
    public class DeleteAssessmentControlRequest : BaseRequest
    {
        public int RowId { get; set; }
        public Guid AssessmentId { get; set; }
        public Guid ControlId { get; set; }
    }
}

