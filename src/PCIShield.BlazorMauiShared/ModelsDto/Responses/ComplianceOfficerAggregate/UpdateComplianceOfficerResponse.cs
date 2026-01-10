using System;
using PCIShield.BlazorMauiShared.Models;
using PCIShield.Domain.ModelsDto;

namespace BlazorMauiShared.Models.ComplianceOfficer
{
    public class UpdateComplianceOfficerResponse : BaseResponse
    {
        public UpdateComplianceOfficerResponse(Guid correlationId)
        : base(correlationId)
        {
        }
        
        public UpdateComplianceOfficerResponse()
        {
        }
        
        public ComplianceOfficerDto ComplianceOfficer { get; set; } = new();
    }
}

