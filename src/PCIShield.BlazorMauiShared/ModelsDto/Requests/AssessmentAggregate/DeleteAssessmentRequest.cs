using System;
using PCIShield.BlazorMauiShared.Models;

namespace BlazorMauiShared.Models.Assessment
{
    public class DeleteAssessmentRequest : BaseRequest
    {
        public Guid AssessmentId { get; set; }
        public Guid MerchantId { get; set; }
    }
}

