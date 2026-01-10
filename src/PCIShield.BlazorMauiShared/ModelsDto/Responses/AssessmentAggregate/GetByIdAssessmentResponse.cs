using System;
using PCIShield.BlazorMauiShared.Models;
using PCIShield.Domain.ModelsDto;

namespace BlazorMauiShared.Models.Assessment
{
    public class GetByIdAssessmentResponse : BaseResponse
    {
        public GetByIdAssessmentResponse(Guid correlationId)
        : base(correlationId)
        {
        }
        
        public GetByIdAssessmentResponse()
        {
        }
        
        public AssessmentDto Assessment { get; set; } = new();
    }
}

