using System;
using PCIShield.BlazorMauiShared.Models;
using PCIShield.Domain.ModelsDto;

namespace BlazorMauiShared.Models.ControlEvidence
{
    public class UpdateControlEvidenceResponse : BaseResponse
    {
        public UpdateControlEvidenceResponse(Guid correlationId)
        : base(correlationId)
        {
        }
        
        public UpdateControlEvidenceResponse()
        {
        }
        
        public ControlEvidenceDto ControlEvidence { get; set; } = new();
    }
}

