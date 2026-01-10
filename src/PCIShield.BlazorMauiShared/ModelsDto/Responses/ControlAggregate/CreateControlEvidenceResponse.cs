using System;
using PCIShield.BlazorMauiShared.Models;
using PCIShield.Domain.ModelsDto;

namespace BlazorMauiShared.Models.ControlEvidence
{
    public class CreateControlEvidenceResponse : BaseResponse
    {
        public CreateControlEvidenceResponse(Guid correlationId)
        : base(correlationId)
        {
        }
        
        public CreateControlEvidenceResponse()
        {
        }
        
        public ControlEvidenceDto ControlEvidence { get; set; } = new();
    }
}

