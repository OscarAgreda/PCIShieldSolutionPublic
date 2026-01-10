using System;
using PCIShield.BlazorMauiShared.Models;
using PCIShield.Domain.ModelsDto;

namespace BlazorMauiShared.Models.Assessment
{
    public class UpdateAssessmentResponse : BaseResponse
    {
        public UpdateAssessmentResponse(Guid correlationId)
        : base(correlationId)
        {
        }
        
        public UpdateAssessmentResponse()
        {
        }
        
        public AssessmentDto Assessment { get; set; } = new();
    }
}

