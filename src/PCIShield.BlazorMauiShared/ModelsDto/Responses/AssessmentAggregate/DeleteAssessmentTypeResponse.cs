using System;
using PCIShield.BlazorMauiShared.Models;
using PCIShield.Domain.ModelsDto;

namespace BlazorMauiShared.Models.AssessmentType
{
    public class DeleteAssessmentTypeResponse : BaseResponse
    {
        public DeleteAssessmentTypeResponse(Guid correlationId)
        : base(correlationId)
        {
        }
        
        public DeleteAssessmentTypeResponse()
        {
        }
        
    }
}

