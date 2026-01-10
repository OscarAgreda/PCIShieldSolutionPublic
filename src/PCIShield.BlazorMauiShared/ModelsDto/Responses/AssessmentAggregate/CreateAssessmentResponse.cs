using System;
using PCIShield.BlazorMauiShared.Models;
using PCIShield.Domain.ModelsDto;

namespace BlazorMauiShared.Models.Assessment
{
    public class CreateAssessmentResponse : BaseResponse
    {
        public CreateAssessmentResponse(Guid correlationId)
        : base(correlationId)
        {
        }
        
        public CreateAssessmentResponse()
        {
        }
        
        public AssessmentDto Assessment { get; set; } = new();
    }
}

