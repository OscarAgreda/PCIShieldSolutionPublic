using System.Collections.Generic;
using System.Linq;
using PCIShield.Domain.Entities;
using PCIShield.Domain.Exceptions;

namespace Ardalis.GuardClauses
{
    public static class AssessmentTypeGuardExtensions
    {
        public static void DuplicateAssessmentType(this IGuardClause guardClause, IEnumerable<AssessmentType> existingAssessmentTypes, AssessmentType newAssessmentType, string parameterName)
        {
            if (existingAssessmentTypes.Any(a => a.AssessmentTypeId == newAssessmentType.AssessmentTypeId))
            {
                throw new DuplicateAssessmentTypeException("Cannot add duplicate assessmentType.", parameterName);
            }
        }
    }
}

