using System;
using PCIShield.BlazorMauiShared.Models;
using PCIShield.Domain.ModelsDto;

namespace BlazorMauiShared.Models.EvidenceType
{
    public class GetByIdEvidenceTypeResponse : BaseResponse
    {
        public GetByIdEvidenceTypeResponse(Guid correlationId)
        : base(correlationId)
        {
        }
        
        public GetByIdEvidenceTypeResponse()
        {
        }
        
        public EvidenceTypeDto EvidenceType { get; set; } = new();
    }
}

