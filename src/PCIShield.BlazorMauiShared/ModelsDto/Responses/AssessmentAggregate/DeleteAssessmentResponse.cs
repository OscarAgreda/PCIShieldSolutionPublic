using System;
using PCIShield.BlazorMauiShared.Models;
using PCIShield.Domain.ModelsDto;

namespace BlazorMauiShared.Models.Assessment
{
    public class DeleteAssessmentResponse : BaseResponse
    {
        public DeleteAssessmentResponse(Guid correlationId)
        : base(correlationId)
        {
        }
        
        public DeleteAssessmentResponse()
        {
        }
        
    }
}

