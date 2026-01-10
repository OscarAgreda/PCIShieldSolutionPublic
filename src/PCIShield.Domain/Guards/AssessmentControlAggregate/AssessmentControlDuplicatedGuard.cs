using System.Collections.Generic;
using System.Linq;
using PCIShield.Domain.Entities;
using PCIShield.Domain.Exceptions;

namespace Ardalis.GuardClauses
{
    public static class AssessmentControlGuardExtensions
    {
        public static void DuplicateAssessmentControl(this IGuardClause guardClause, IEnumerable<AssessmentControl> existingAssessmentControls, AssessmentControl newAssessmentControl, string parameterName)
        {
            if (existingAssessmentControls.Any(a => a.RowId == newAssessmentControl.RowId))
            {
                throw new DuplicateAssessmentControlException("Cannot add duplicate assessmentControl.", parameterName);
            }
        }
    }
}

