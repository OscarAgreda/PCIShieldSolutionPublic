using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Ardalis.GuardClauses;
using Ardalis.Specification;

namespace PCIShield.Infrastructure.Data.Specifications
{
    public sealed class AppLocalizationByKeyAndCultureSpec : Specification<AppLocalization>
    {
        public AppLocalizationByKeyAndCultureSpec(string key, string culture, Guid tenantId)
        {
            Guard.Against.NullOrWhiteSpace(key, nameof(key));
            Guard.Against.NullOrWhiteSpace(culture, nameof(culture));
            Guard.Against.Default(tenantId, nameof(tenantId));

            Query
                .Where(x => x.Key == key && x.Culture == culture && x.TenantId == tenantId && !x.IsDeleted)
                .AsNoTracking()
                .EnableCache($"AppLocalizationByKeyAndCulture-{key}-{culture}-{tenantId}");
        }
    }
    public sealed class AppLocalizationByKeyDefaultCultureSpec : Specification<AppLocalization>
    {
        public AppLocalizationByKeyDefaultCultureSpec(string key, Guid tenantId)
        {
            Guard.Against.NullOrWhiteSpace(key, nameof(key));
            Guard.Against.Default(tenantId, nameof(tenantId));

            Query
                .Where(x => x.Key == key && x.Culture == "en-US" && x.TenantId == tenantId && !x.IsDeleted)
                .AsNoTracking()
                .EnableCache($"AppLocalizationByKeyDefaultCulture-{key}-{tenantId}");
        }
    }
}
