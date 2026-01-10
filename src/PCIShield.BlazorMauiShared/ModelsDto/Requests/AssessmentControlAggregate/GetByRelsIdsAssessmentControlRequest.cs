using System;
using PCIShield.BlazorMauiShared.Models;

namespace BlazorMauiShared.Models.AssessmentControl
{
    public class GetByRelsIdsAssessmentControlRequest : BaseRequest
    {
        public Guid AssessmentId { get; set; }
        public Guid ControlId { get; set; }
    }
}

