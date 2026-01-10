using System.Collections.Generic;
using System.Linq;
using PCIShield.Domain.Entities;
using PCIShield.Domain.Exceptions;

namespace Ardalis.GuardClauses
{
    public static class EvidenceTypeGuardExtensions
    {
        public static void DuplicateEvidenceType(this IGuardClause guardClause, IEnumerable<EvidenceType> existingEvidenceTypes, EvidenceType newEvidenceType, string parameterName)
        {
            if (existingEvidenceTypes.Any(a => a.EvidenceTypeId == newEvidenceType.EvidenceTypeId))
            {
                throw new DuplicateEvidenceTypeException("Cannot add duplicate evidenceType.", parameterName);
            }
        }
    }
}

