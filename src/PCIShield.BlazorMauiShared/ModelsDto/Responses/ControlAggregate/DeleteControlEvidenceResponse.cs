using System;
using PCIShield.BlazorMauiShared.Models;
using PCIShield.Domain.ModelsDto;

namespace BlazorMauiShared.Models.ControlEvidence
{
    public class DeleteControlEvidenceResponse : BaseResponse
    {
        public DeleteControlEvidenceResponse(Guid correlationId)
        : base(correlationId)
        {
        }
        
        public DeleteControlEvidenceResponse()
        {
        }
        
    }
}

