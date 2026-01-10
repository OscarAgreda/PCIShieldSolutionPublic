using System;
using PCIShield.BlazorMauiShared.Models;
using PCIShield.Domain.ModelsDto;

namespace BlazorMauiShared.Models.ComplianceOfficer
{
    public class CreateComplianceOfficerResponse : BaseResponse
    {
        public CreateComplianceOfficerResponse(Guid correlationId)
        : base(correlationId)
        {
        }
        
        public CreateComplianceOfficerResponse()
        {
        }
        
        public ComplianceOfficerDto ComplianceOfficer { get; set; } = new();
    }
}

