using System;
using PCIShield.BlazorMauiShared.Models;
using PCIShield.Domain.ModelsDto;

namespace BlazorMauiShared.Models.AssessmentControl
{
    public class CreateAssessmentControlResponse : BaseResponse
    {
        public CreateAssessmentControlResponse(Guid correlationId)
        : base(correlationId)
        {
        }
        
        public CreateAssessmentControlResponse()
        {
        }
        
        public AssessmentControlDto AssessmentControl { get; set; } = new();
    }
}

