using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Ardalis.GuardClauses;
using Ardalis.Specification;

namespace PCIShield.Infrastructure.Data.Specifications
{
    public sealed class UserKeyIdSpec : Specification<CustomPCIShieldUser>
    {
        public UserKeyIdSpec(Guid applicationUserId)
        {
            Guard.Against.Default(applicationUserId, nameof(applicationUserId));

            Query
                .Where(x => x.ApplicationUserId == applicationUserId)
                .AsNoTracking();
        }
    }
    public sealed class UserByEmailSpec : Specification<CustomPCIShieldUser>
    {
        public UserByEmailSpec(string email)
        {
            Guard.Against.NullOrWhiteSpace(email, nameof(email));

            Query
                .Where(x => x.Email == email)
                .AsNoTracking();
        }
    }
}
