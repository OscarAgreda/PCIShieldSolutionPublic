using System;
using PCIShield.BlazorMauiShared.Models;

namespace BlazorMauiShared.Models.AssessmentType
{
    public class GetByIdAssessmentTypeRequest : BaseRequest
    {
        public Guid AssessmentTypeId { get; set; }
        public bool WithPostGraph { get; set; }
    }
}

