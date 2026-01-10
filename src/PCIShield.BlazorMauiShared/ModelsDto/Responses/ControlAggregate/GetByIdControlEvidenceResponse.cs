using System;
using PCIShield.BlazorMauiShared.Models;
using PCIShield.Domain.ModelsDto;

namespace BlazorMauiShared.Models.ControlEvidence
{
    public class GetByIdControlEvidenceResponse : BaseResponse
    {
        public GetByIdControlEvidenceResponse(Guid correlationId)
        : base(correlationId)
        {
        }
        
        public GetByIdControlEvidenceResponse()
        {
        }
        
        public ControlEvidenceDto ControlEvidence { get; set; } = new();
    }
}

