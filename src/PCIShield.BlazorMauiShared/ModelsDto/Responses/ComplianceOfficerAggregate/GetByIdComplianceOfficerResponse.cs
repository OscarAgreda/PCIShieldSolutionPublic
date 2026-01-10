using System;
using PCIShield.BlazorMauiShared.Models;
using PCIShield.Domain.ModelsDto;

namespace BlazorMauiShared.Models.ComplianceOfficer
{
    public class GetByIdComplianceOfficerResponse : BaseResponse
    {
        public GetByIdComplianceOfficerResponse(Guid correlationId)
        : base(correlationId)
        {
        }
        
        public GetByIdComplianceOfficerResponse()
        {
        }
        
        public ComplianceOfficerDto ComplianceOfficer { get; set; } = new();
    }
}

