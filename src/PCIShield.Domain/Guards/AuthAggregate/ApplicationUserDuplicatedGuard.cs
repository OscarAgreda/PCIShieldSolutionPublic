using System.Collections.Generic;
using System.Linq;
using PCIShield.Domain.Entities;
using PCIShield.Domain.Exceptions;

namespace Ardalis.GuardClauses
{
    public static class ApplicationUserGuardExtensions
    {
        public static void DuplicateApplicationUser(this IGuardClause guardClause, IEnumerable<ApplicationUser> existingApplicationUsers, ApplicationUser newApplicationUser, string parameterName)
        {
            if (existingApplicationUsers.Any(a => a.ApplicationUserId == newApplicationUser.ApplicationUserId))
            {
                throw new DuplicateApplicationUserException("Cannot add duplicate applicationUser.", parameterName);
            }
        }
    }
}

