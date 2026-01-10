using System;
using PCIShield.BlazorMauiShared.Models;

namespace BlazorMauiShared.Models.AssessmentType
{
    public class DeleteAssessmentTypeRequest : BaseRequest
    {
        public Guid AssessmentTypeId { get; set; }
    }
}

