using System;
using System.Collections.Generic;
using PCIShield.BlazorMauiShared.Models;
using PCIShield.Domain.ModelsDto;

namespace BlazorMauiShared.Models.AssessmentControl
{
    public class ListAssessmentControlResponse : BaseResponse
    {
        public ListAssessmentControlResponse(Guid correlationId)
        : base(correlationId)
        {
        }
        
        public ListAssessmentControlResponse()
        {
        }
        
        public List<AssessmentControlDto>? AssessmentControls { get; set; } = new();
        
        public int Count { get; set; }
        public int TotalCount { get; set; }
        public int PageNumber { get; set; }
        public int PageSize { get; set; }
        public int TotalPages { get; set; }
    }
}

