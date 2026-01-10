using System;
using PCIShield.BlazorMauiShared.Models;
using PCIShield.Domain.ModelsDto;

namespace BlazorMauiShared.Models.EvidenceType
{
    public class UpdateEvidenceTypeResponse : BaseResponse
    {
        public UpdateEvidenceTypeResponse(Guid correlationId)
        : base(correlationId)
        {
        }
        
        public UpdateEvidenceTypeResponse()
        {
        }
        
        public EvidenceTypeDto EvidenceType { get; set; } = new();
    }
}

