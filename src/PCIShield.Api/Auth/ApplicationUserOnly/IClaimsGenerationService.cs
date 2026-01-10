using System.Collections.Generic;
using System.Security.Claims;
using System.Threading.Tasks;
using PCIShield.Infrastructure.Data;

namespace PCIShield.Api.Auth.ApplicationUserOnly
{
    public interface IClaimsGenerationService
    {
        Task<List<Claim>> GenerateClaimsForUserAsync(
            CustomPCIShieldUser user,
            IList<string> roles,
            string pciShieldAppPowerUserId = null,
            string tenantId = null);
    }
}