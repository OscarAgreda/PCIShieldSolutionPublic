using System;
using System.Linq;
using Ardalis.GuardClauses;
using Ardalis.Specification;
using PCIShield.Domain.Entities;

namespace PCIShield.Domain.Specifications
{
    public class ControlEvidenceByRelIdsNoTypeSpec : Specification<ControlEvidence>
    {
        public ControlEvidenceByRelIdsNoTypeSpec(Guid controlId, Guid evidenceId, Guid assessmentId)
        {
            Guard.Against.Default(controlId, nameof(controlId));
            Guard.Against.Default(evidenceId, nameof(evidenceId));
            Guard.Against.Default(assessmentId, nameof(assessmentId));

            _ = Query.Where(controlEvidence => controlEvidence.ControlId == controlId && controlEvidence.EvidenceId == evidenceId && controlEvidence.AssessmentId == assessmentId).AsSplitQuery().AsNoTracking().EnableCache($"ControlEvidenceByRelIdsSpec-{controlId}-{controlId}-{evidenceId}-{evidenceId}-{assessmentId}-{assessmentId}");
  }
  }
    public class ControlEvidenceByRelIdsSpec : Specification<ControlEvidence>
    {
        public ControlEvidenceByRelIdsSpec(Guid controlId, Guid evidenceId, Guid assessmentId)
        {
            Guard.Against.Default(controlId, nameof(controlId));
            Guard.Against.Default(evidenceId, nameof(evidenceId));
            Guard.Against.Default(assessmentId, nameof(assessmentId));

            _ = Query.Where(controlEvidence => controlEvidence.ControlId == controlId && controlEvidence.EvidenceId == evidenceId && controlEvidence.AssessmentId == assessmentId).AsSplitQuery().AsNoTracking().EnableCache($"ControlEvidenceByRelIdsSpec-{controlId}-{controlId}-{evidenceId}-{evidenceId}-{assessmentId}-{assessmentId}");
  }
  }
}
