using System.Collections.Generic;
using System.Linq;
using PCIShield.Domain.Entities;
using PCIShield.Domain.Exceptions;

namespace Ardalis.GuardClauses
{
    public static class AssessmentGuardExtensions
    {
        public static void DuplicateAssessment(this IGuardClause guardClause, IEnumerable<Assessment> existingAssessments, Assessment newAssessment, string parameterName)
        {
            if (existingAssessments.Any(a => a.AssessmentId == newAssessment.AssessmentId))
            {
                throw new DuplicateAssessmentException("Cannot add duplicate assessment.", parameterName);
            }
        }
    }
}

