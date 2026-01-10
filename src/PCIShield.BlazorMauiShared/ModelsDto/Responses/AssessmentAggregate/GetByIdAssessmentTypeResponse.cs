using System;
using PCIShield.BlazorMauiShared.Models;
using PCIShield.Domain.ModelsDto;

namespace BlazorMauiShared.Models.AssessmentType
{
    public class GetByIdAssessmentTypeResponse : BaseResponse
    {
        public GetByIdAssessmentTypeResponse(Guid correlationId)
        : base(correlationId)
        {
        }
        
        public GetByIdAssessmentTypeResponse()
        {
        }
        
        public AssessmentTypeDto AssessmentType { get; set; } = new();
    }
}

