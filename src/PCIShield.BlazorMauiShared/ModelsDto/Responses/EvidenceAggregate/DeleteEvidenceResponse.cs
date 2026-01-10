using System;
using PCIShield.BlazorMauiShared.Models;
using PCIShield.Domain.ModelsDto;

namespace BlazorMauiShared.Models.Evidence
{
    public class DeleteEvidenceResponse : BaseResponse
    {
        public DeleteEvidenceResponse(Guid correlationId)
        : base(correlationId)
        {
        }
        
        public DeleteEvidenceResponse()
        {
        }
        
    }
}

