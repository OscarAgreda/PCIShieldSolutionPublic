using System;
using PCIShield.BlazorMauiShared.Models;
using PCIShield.Domain.ModelsDto;

namespace BlazorMauiShared.Models.AssessmentType
{
    public class UpdateAssessmentTypeResponse : BaseResponse
    {
        public UpdateAssessmentTypeResponse(Guid correlationId)
        : base(correlationId)
        {
        }
        
        public UpdateAssessmentTypeResponse()
        {
        }
        
        public AssessmentTypeDto AssessmentType { get; set; } = new();
    }
}

