using System;
using PCIShield.BlazorMauiShared.Models;

namespace BlazorMauiShared.Models.AssessmentControl
{
    public class GetByIdAssessmentControlRequest : BaseRequest
    {
        public Guid AssessmentId { get; set; }
        public Guid ControlId { get; set; }
        public int RowId { get; set; }
        public bool WithPostGraph { get; set; }
    }
}

