using System;
using PCIShield.BlazorMauiShared.Models;

namespace BlazorMauiShared.Models.ComplianceOfficer
{
    public class GetByIdComplianceOfficerRequest : BaseRequest
    {
        public Guid MerchantId { get; set; }
        public Guid ComplianceOfficerId { get; set; }
        public bool WithPostGraph { get; set; }
    }
}

