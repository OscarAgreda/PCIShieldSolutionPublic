using System;
using PCIShield.BlazorMauiShared.Models;
using PCIShield.Domain.ModelsDto;

namespace BlazorMauiShared.Models.AssessmentControl
{
    public class DeleteAssessmentControlResponse : BaseResponse
    {
        public DeleteAssessmentControlResponse(Guid correlationId)
        : base(correlationId)
        {
        }
        
        public DeleteAssessmentControlResponse()
        {
        }
        
    }
}

