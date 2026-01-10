using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using System;
using System.Linq.Expressions;
using PCIShield.Domain.Entities;
using PCIShieldLib.SharedKernel.Interfaces;
using Ardalis.Specification;
using Ardalis.GuardClauses;
using Ardalis.Specification;
namespace PCIShield.Infrastructure.Data.Specifications
{
    public class ApplicationUserByIdForAuthSpec : Specification<ApplicationUser>
    {
        public ApplicationUserByIdForAuthSpec(Guid applicationUserId)
        {
            Query.Where(u => u.ApplicationUserId == applicationUserId
                            && !u.IsDeleted
                            && !u.IsLocked
                            && !u.IsBanned
                            && u.IsLoginAllowed);
        }
    }
    public class ApplicationUserByUsernameSpec : Specification<ApplicationUser>
    {
        public ApplicationUserByUsernameSpec(string username)
        {
            Query.Where(u => u.UserName == username && !u.IsDeleted);
        }
    }
    public class ApplicationUserByEmailSpec : Specification<ApplicationUser>
    {
        public ApplicationUserByEmailSpec(string email)
        {
            Query.Where(u => u.Email == email && !u.IsDeleted);
        }
    }
    public class CustomPCIShieldUserByRefreshTokenSpec : Specification<PCIShield.Infrastructure.Data.CustomPCIShieldUser>
    {
        public CustomPCIShieldUserByRefreshTokenSpec(string refreshToken)
        {
            Query.Where(u => u.RefreshToken == refreshToken && !string.IsNullOrEmpty(u.RefreshToken));
        }
    }
    public class UsernameExistsSpec : Specification<ApplicationUser>
    {
        public UsernameExistsSpec(string username)
        {
            Query.Where(u => u.UserName.ToLower() == username.ToLower() && !u.IsDeleted);
        }
    }
    public class EmailExistsSpec : Specification<ApplicationUser>
    {
        public EmailExistsSpec(string email)
        {
            Query.Where(u => u.Email.ToLower() == email.ToLower() && !u.IsDeleted);
        }
    }
    public class UsersWithFailedLoginAttemptsSpec : Specification<ApplicationUser>
    {
        public UsersWithFailedLoginAttemptsSpec(int minFailedAttempts = 3)
        {
            Query.Where(u => u.FailedLoginCount >= minFailedAttempts
                            && !u.IsDeleted
                            && !u.IsLocked)
                 .OrderByDescending(u => u.FailedLoginCount)
                 .ThenByDescending(u => u.UpdatedAt);
        }
    }
    public class LockedUsersSpec : Specification<ApplicationUser>
    {
        public LockedUsersSpec()
        {
            Query.Where(u => u.IsLocked && !u.IsDeleted)
                 .OrderByDescending(u => u.LockedUntil);
        }
    }
    public class PendingEmailVerificationSpec : Specification<ApplicationUser>
    {
        public PendingEmailVerificationSpec()
        {
            Query.Where(u => !u.IsEmailVerified
                            && !u.IsDeleted
                            && u.CreatedDate > DateTime.UtcNow.AddDays(-30))
                 .OrderBy(u => u.CreatedDate);
        }
    }
    public class ApplicationUsersBySyntheticScopeSpec : Specification<ApplicationUser>
    {
        public ApplicationUsersBySyntheticScopeSpec(Guid tenantId)
        {
            Query.Where(u => u.TenantId == tenantId && !u.IsDeleted);
        }
    }
    public sealed  class OnlineUsersSpec : Specification<ApplicationUser>
    {
        public OnlineUsersSpec()
        {
            Query.Where(u => u.IsOnline.HasValue && u.IsOnline.Value == true && u.IsLoggedIntoApp.HasValue && u.IsLoggedIntoApp.Value == true && !u.IsDeleted
                             && u.TimeLastSignalrPing > DateTime.UtcNow.AddMinutes(-5))
                 .OrderByDescending(u => u.TimeLastSignalrPing);
        }
    }
}