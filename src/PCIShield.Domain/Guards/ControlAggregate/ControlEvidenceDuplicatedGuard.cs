using System.Collections.Generic;
using System.Linq;
using PCIShield.Domain.Entities;
using PCIShield.Domain.Exceptions;

namespace Ardalis.GuardClauses
{
    public static class ControlEvidenceGuardExtensions
    {
        public static void DuplicateControlEvidence(this IGuardClause guardClause, IEnumerable<ControlEvidence> existingControlEvidences, ControlEvidence newControlEvidence, string parameterName)
        {
            if (existingControlEvidences.Any(a => a.RowId == newControlEvidence.RowId))
            {
                throw new DuplicateControlEvidenceException("Cannot add duplicate controlEvidence.", parameterName);
            }
        }
    }
}

