using System.Collections.Generic;
using System.Linq;
using PCIShield.Domain.Entities;
using PCIShield.Domain.Exceptions;

namespace Ardalis.GuardClauses
{
    public static class EvidenceGuardExtensions
    {
        public static void DuplicateEvidence(this IGuardClause guardClause, IEnumerable<Evidence> existingEvidences, Evidence newEvidence, string parameterName)
        {
            if (existingEvidences.Any(a => a.EvidenceId == newEvidence.EvidenceId))
            {
                throw new DuplicateEvidenceException("Cannot add duplicate evidence.", parameterName);
            }
        }
    }
}

