using System;
using System.Linq;
using Ardalis.GuardClauses;
using Ardalis.Specification;
using  PCIShield.Domain.Entities;
using  PCIShield.Domain.ModelEntityDto;
  	  
namespace  PCIShield.Domain.Specifications
{
    public class AssessmentTypeByIdJustOneSpec : Specification<AssessmentType, AssessmentTypeEntityDto>
    {
        public AssessmentTypeByIdJustOneSpec(Guid assessmentTypeId)
        {
            _ = Guard.Against.NullOrEmpty(assessmentTypeId, nameof(assessmentTypeId));
            _ = Query.Where(assessmentType => assessmentType.AssessmentTypeId == assessmentTypeId);
            _ = Query
                .Select(x => new AssessmentTypeEntityDto
                {
                    AssessmentTypeId = x.AssessmentTypeId,
                })
                .AsNoTracking()
                .EnableCache($"AssessmentTypeByIdJustOne-{assessmentTypeId.ToString()}");
        }
    }
}

