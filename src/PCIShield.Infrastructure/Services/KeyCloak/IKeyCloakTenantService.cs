using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
namespace PCIShield.Infrastructure.Services
{
    public interface IKeyCloakTenantService
    {
        string? GetPCIShieldSolutionuperUserIdPassed(string pciShieldappAppSuperUserId);
        string? GetPCIShieldSolutionuperUserId();
        string GetTenantId();
        string GetKeyCloakTenantId();
    }
}
