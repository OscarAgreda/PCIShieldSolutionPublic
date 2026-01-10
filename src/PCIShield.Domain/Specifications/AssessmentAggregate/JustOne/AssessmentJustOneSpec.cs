using System;
using System.Linq;
using Ardalis.GuardClauses;
using Ardalis.Specification;
using  PCIShield.Domain.Entities;
using  PCIShield.Domain.ModelEntityDto;
  	  
namespace  PCIShield.Domain.Specifications
{
    public class AssessmentByIdJustOneSpec : Specification<Assessment, AssessmentEntityDto>
    {
        public AssessmentByIdJustOneSpec(Guid assessmentId)
        {
            _ = Guard.Against.NullOrEmpty(assessmentId, nameof(assessmentId));
            _ = Query.Where(assessment => assessment.AssessmentId == assessmentId);
            _ = Query
                .Select(x => new AssessmentEntityDto
                {
                    AssessmentId = x.AssessmentId,
                    MerchantId = x.MerchantId,
                })
                .AsNoTracking()
                .EnableCache($"AssessmentByIdJustOne-{assessmentId.ToString()}");
        }
    }
}

