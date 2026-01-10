using System;
using PCIShield.BlazorMauiShared.Models;
using PCIShield.Domain.ModelsDto;

namespace BlazorMauiShared.Models.AssessmentControl
{
    public class GetByIdAssessmentControlResponse : BaseResponse
    {
        public GetByIdAssessmentControlResponse(Guid correlationId)
        : base(correlationId)
        {
        }
        
        public GetByIdAssessmentControlResponse()
        {
        }
        
        public AssessmentControlDto AssessmentControl { get; set; } = new();
    }
}

