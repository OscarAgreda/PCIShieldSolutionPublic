using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using PCIShield.Infrastructure.Data;
using Microsoft.AspNetCore.Identity;

namespace PCIShield.Api.Auth.ApplicationUserOnly
{
    public class ClaimsGenerationService : IClaimsGenerationService
    {
        private readonly UserManager<CustomPCIShieldUser> _userManager;

        public ClaimsGenerationService(UserManager<CustomPCIShieldUser> userManager)
        {
            _userManager = userManager;
        }

        public async Task<List<Claim>> GenerateClaimsForUserAsync(CustomPCIShieldUser user, IList<string> roles, string pciShieldAppPowerUserId = null, string tenantId = null)
        {
            var existingClaims = await _userManager.GetClaimsAsync(user);
            var claimsToRemove = existingClaims
                .Where(c => c.Type == "permission" ||
                            c.Type == "scope" ||
                            c.Type == "PCIShieldSolutionuperUserId" ||
                            c.Type == "TenantId")
                .ToList();

            foreach (var claim in claimsToRemove)
            {
                await _userManager.RemoveClaimAsync(user, claim);
            }

            var claims = new List<Claim>
    {
        new(JwtRegisteredClaimNames.Sub, user.UserName ?? string.Empty),
        new(ClaimTypes.Name, user.UserName ?? string.Empty),
        new(ClaimTypes.NameIdentifier, user.Id),
        new("ApplicationUserId", user.ApplicationUserId.ToString()),
        new(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString()),
        new(JwtRegisteredClaimNames.Iat, new DateTimeOffset(DateTime.UtcNow).ToUnixTimeSeconds().ToString(), ClaimValueTypes.Integer64)
    };
            if (!string.IsNullOrEmpty(user.SecurityStamp))
            {
                claims.Add(new Claim("AspNet.Identity.SecurityStamp", user.SecurityStamp));
            }
            if (roles.Contains("Admin"))
            {
                claims.Add(new Claim("permission", "manage_users"));
                claims.Add(new Claim("permission", "view_reports"));
                claims.Add(new Claim("scope", "admin.all"));
            }

            if (roles.Contains("ComplianceOfficer"))
            {
                claims.Add(new Claim("permission", "manage_merchants"));
                claims.Add(new Claim("permission", "manage_invoices"));
                claims.Add(new Claim("scope", "complianceOfficer.operations"));
            }

            if (roles.Contains("Merchant") || roles.Contains("StandardUser"))
            {
                claims.Add(new Claim("permission", "view_own_invoices"));
                claims.Add(new Claim("permission", "update_own_profile"));
                claims.Add(new Claim("scope", "merchant.self"));
            }

            if (roles.Any())
            {
                claims.Add(new Claim("permission", "view_dashboard"));
                claims.Add(new Claim("scope", "user.read_own_data"));
            }
            if (!string.IsNullOrEmpty(pciShieldAppPowerUserId))
            {
                claims.Add(new Claim("PCIShieldSolutionuperUserId", pciShieldAppPowerUserId));
            }

            if (!string.IsNullOrEmpty(tenantId))
            {
                claims.Add(new Claim("TenantId", tenantId));
            }
            foreach (var role in roles)
            {
                claims.Add(new Claim(ClaimTypes.Role, role));
            }

            return claims;
        }
    }
}