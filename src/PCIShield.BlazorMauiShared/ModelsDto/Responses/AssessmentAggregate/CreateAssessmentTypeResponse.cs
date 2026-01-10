using System;
using PCIShield.BlazorMauiShared.Models;
using PCIShield.Domain.ModelsDto;

namespace BlazorMauiShared.Models.AssessmentType
{
    public class CreateAssessmentTypeResponse : BaseResponse
    {
        public CreateAssessmentTypeResponse(Guid correlationId)
        : base(correlationId)
        {
        }
        
        public CreateAssessmentTypeResponse()
        {
        }
        
        public AssessmentTypeDto AssessmentType { get; set; } = new();
    }
}

