using System;
using PCIShield.BlazorMauiShared.Models;

namespace BlazorMauiShared.Models.ComplianceOfficer
{
    public class DeleteComplianceOfficerRequest : BaseRequest
    {
        public Guid ComplianceOfficerId { get; set; }
        public Guid MerchantId { get; set; }
    }
}

