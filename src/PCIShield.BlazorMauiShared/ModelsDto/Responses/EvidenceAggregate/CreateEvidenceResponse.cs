using System;
using PCIShield.BlazorMauiShared.Models;
using PCIShield.Domain.ModelsDto;

namespace BlazorMauiShared.Models.Evidence
{
    public class CreateEvidenceResponse : BaseResponse
    {
        public CreateEvidenceResponse(Guid correlationId)
        : base(correlationId)
        {
        }
        
        public CreateEvidenceResponse()
        {
        }
        
        public EvidenceDto Evidence { get; set; } = new();
    }
}

