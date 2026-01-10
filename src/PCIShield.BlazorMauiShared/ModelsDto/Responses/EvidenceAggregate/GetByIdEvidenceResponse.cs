using System;
using PCIShield.BlazorMauiShared.Models;
using PCIShield.Domain.ModelsDto;

namespace BlazorMauiShared.Models.Evidence
{
    public class GetByIdEvidenceResponse : BaseResponse
    {
        public GetByIdEvidenceResponse(Guid correlationId)
        : base(correlationId)
        {
        }
        
        public GetByIdEvidenceResponse()
        {
        }
        
        public EvidenceDto Evidence { get; set; } = new();
    }
}

