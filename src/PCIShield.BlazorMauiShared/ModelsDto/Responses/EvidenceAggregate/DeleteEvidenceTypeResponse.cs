using System;
using PCIShield.BlazorMauiShared.Models;
using PCIShield.Domain.ModelsDto;

namespace BlazorMauiShared.Models.EvidenceType
{
    public class DeleteEvidenceTypeResponse : BaseResponse
    {
        public DeleteEvidenceTypeResponse(Guid correlationId)
        : base(correlationId)
        {
        }
        
        public DeleteEvidenceTypeResponse()
        {
        }
        
    }
}

