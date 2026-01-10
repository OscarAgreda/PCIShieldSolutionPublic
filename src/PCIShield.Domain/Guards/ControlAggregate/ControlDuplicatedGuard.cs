using System.Collections.Generic;
using System.Linq;
using PCIShield.Domain.Entities;
using PCIShield.Domain.Exceptions;

namespace Ardalis.GuardClauses
{
    public static class ControlGuardExtensions
    {
        public static void DuplicateControl(this IGuardClause guardClause, IEnumerable<Control> existingControls, Control newControl, string parameterName)
        {
            if (existingControls.Any(a => a.ControlId == newControl.ControlId))
            {
                throw new DuplicateControlException("Cannot add duplicate control.", parameterName);
            }
        }
    }
}

