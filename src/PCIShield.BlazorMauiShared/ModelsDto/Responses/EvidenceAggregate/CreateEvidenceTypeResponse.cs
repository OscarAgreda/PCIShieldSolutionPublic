using System;
using PCIShield.BlazorMauiShared.Models;
using PCIShield.Domain.ModelsDto;

namespace BlazorMauiShared.Models.EvidenceType
{
    public class CreateEvidenceTypeResponse : BaseResponse
    {
        public CreateEvidenceTypeResponse(Guid correlationId)
        : base(correlationId)
        {
        }
        
        public CreateEvidenceTypeResponse()
        {
        }
        
        public EvidenceTypeDto EvidenceType { get; set; } = new();
    }
}

