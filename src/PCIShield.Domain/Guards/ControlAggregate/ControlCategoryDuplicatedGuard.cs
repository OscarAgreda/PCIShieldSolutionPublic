using System.Collections.Generic;
using System.Linq;
using PCIShield.Domain.Entities;
using PCIShield.Domain.Exceptions;

namespace Ardalis.GuardClauses
{
    public static class ControlCategoryGuardExtensions
    {
        public static void DuplicateControlCategory(this IGuardClause guardClause, IEnumerable<ControlCategory> existingControlCategories, ControlCategory newControlCategory, string parameterName)
        {
            if (existingControlCategories.Any(a => a.ControlCategoryId == newControlCategory.ControlCategoryId))
            {
                throw new DuplicateControlCategoryException("Cannot add duplicate controlCategory.", parameterName);
            }
        }
    }
}

