using System;
using PCIShield.BlazorMauiShared.Models;
using PCIShield.Domain.ModelsDto;

namespace BlazorMauiShared.Models.AssessmentControl
{
    public class UpdateAssessmentControlResponse : BaseResponse
    {
        public UpdateAssessmentControlResponse(Guid correlationId)
        : base(correlationId)
        {
        }
        
        public UpdateAssessmentControlResponse()
        {
        }
        
        public AssessmentControlDto AssessmentControl { get; set; } = new();
    }
}

