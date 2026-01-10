using System;
using PCIShield.BlazorMauiShared.Models;
using PCIShield.Domain.ModelsDto;

namespace BlazorMauiShared.Models.Evidence
{
    public class UpdateEvidenceResponse : BaseResponse
    {
        public UpdateEvidenceResponse(Guid correlationId)
        : base(correlationId)
        {
        }
        
        public UpdateEvidenceResponse()
        {
        }
        
        public EvidenceDto Evidence { get; set; } = new();
    }
}

