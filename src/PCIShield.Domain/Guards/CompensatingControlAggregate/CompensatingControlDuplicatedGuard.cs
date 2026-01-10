using System.Collections.Generic;
using System.Linq;
using PCIShield.Domain.Entities;
using PCIShield.Domain.Exceptions;

namespace Ardalis.GuardClauses
{
    public static class CompensatingControlGuardExtensions
    {
        public static void DuplicateCompensatingControl(this IGuardClause guardClause, IEnumerable<CompensatingControl> existingCompensatingControls, CompensatingControl newCompensatingControl, string parameterName)
        {
            if (existingCompensatingControls.Any(a => a.CompensatingControlId == newCompensatingControl.CompensatingControlId))
            {
                throw new DuplicateCompensatingControlException("Cannot add duplicate compensatingControl.", parameterName);
            }
        }
    }
}

