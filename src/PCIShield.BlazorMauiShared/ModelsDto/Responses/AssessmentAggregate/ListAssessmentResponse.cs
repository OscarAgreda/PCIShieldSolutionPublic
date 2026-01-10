using System;
using System.Collections.Generic;
using PCIShield.BlazorMauiShared.Models;
using PCIShield.Domain.ModelsDto;

namespace BlazorMauiShared.Models.Assessment
{
    public class ListAssessmentResponse : BaseResponse
    {
        public ListAssessmentResponse(Guid correlationId)
        : base(correlationId)
        {
        }
        
        public ListAssessmentResponse()
        {
        }
        
        public List<AssessmentDto>? Assessments { get; set; } = new();
        
        public int Count { get; set; }
        public int TotalCount { get; set; }
        public int PageNumber { get; set; }
        public int PageSize { get; set; }
        public int TotalPages { get; set; }
    }
}

