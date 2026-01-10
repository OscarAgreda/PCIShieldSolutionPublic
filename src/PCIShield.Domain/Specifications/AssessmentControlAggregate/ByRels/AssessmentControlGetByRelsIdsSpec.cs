using System;
using System.Linq;
using Ardalis.GuardClauses;
using Ardalis.Specification;
using PCIShield.Domain.Entities;

namespace PCIShield.Domain.Specifications
{
    public class AssessmentControlByRelIdsNoTypeSpec : Specification<AssessmentControl>
    {
        public AssessmentControlByRelIdsNoTypeSpec(Guid assessmentId, Guid controlId)
        {
            Guard.Against.Default(assessmentId, nameof(assessmentId));
            Guard.Against.Default(controlId, nameof(controlId));

            _ = Query.Where(assessmentControl => assessmentControl.AssessmentId == assessmentId && assessmentControl.ControlId == controlId).AsSplitQuery().AsNoTracking().EnableCache($"AssessmentControlByRelIdsSpec-{assessmentId}-{assessmentId}-{controlId}-{controlId}");
  }
  }
    public class AssessmentControlByRelIdsSpec : Specification<AssessmentControl>
    {
        public AssessmentControlByRelIdsSpec(Guid assessmentId, Guid controlId)
        {
            Guard.Against.Default(assessmentId, nameof(assessmentId));
            Guard.Against.Default(controlId, nameof(controlId));

            _ = Query.Where(assessmentControl => assessmentControl.AssessmentId == assessmentId && assessmentControl.ControlId == controlId).AsSplitQuery().AsNoTracking().EnableCache($"AssessmentControlByRelIdsSpec-{assessmentId}-{assessmentId}-{controlId}-{controlId}");
  }
  }
}
