using System;
using PCIShield.BlazorMauiShared.Models;
using PCIShield.Domain.ModelsDto;

namespace BlazorMauiShared.Models.ComplianceOfficer
{
    public class DeleteComplianceOfficerResponse : BaseResponse
    {
        public DeleteComplianceOfficerResponse(Guid correlationId)
        : base(correlationId)
        {
        }
        
        public DeleteComplianceOfficerResponse()
        {
        }
        
    }
}

