using System;
using PCIShield.BlazorMauiShared.Models;

namespace BlazorMauiShared.Models.Assessment
{
    public class GetByIdAssessmentRequest : BaseRequest
    {
        public Guid MerchantId { get; set; }
        public Guid AssessmentId { get; set; }
        public bool WithPostGraph { get; set; }
    }
}

