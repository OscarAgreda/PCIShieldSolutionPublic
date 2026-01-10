using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
namespace PCIShield.Infrastructure.Services
{
    public class KeyCloakTenantService : IKeyCloakTenantService
    {
        private readonly IHttpContextAccessor _httpContextAccessor;
        private readonly IAppLoggerService<KeyCloakTenantService> _logger;
        public KeyCloakTenantService(IHttpContextAccessor httpContextAccessor, IAppLoggerService<KeyCloakTenantService> logger)
        {
            this._httpContextAccessor = httpContextAccessor;
            this._logger = logger;
        }
        public string GetKeyCloakTenantId()
        {
            var user = this._httpContextAccessor.HttpContext.User;
            var tenantIdClaim = user.FindFirst("realm");
            if (tenantIdClaim == null)
            {
                this._logger.LogError("Tenant ID claim not found in user's claims.");
                throw new Exception("Tenant ID claim not found in user's claims.");
            }
            return tenantIdClaim.Value;
        }
        public string? GetPCIShieldSolutionuperUserId()
        {
            var user = this._httpContextAccessor.HttpContext.User;
            var tenantIdClaim = user.FindFirst("PCIShieldSolutionuperUserId");
            if (tenantIdClaim == null)
            {
                return "7dbcce64-324f-43aa-8d3c-f584503f6737";
                this._logger.LogError("PCIShieldSolutionuperUserId claim not found in user's claims.");
            }
            return tenantIdClaim.Value;
        }
        public string? GetPCIShieldSolutionuperUserIdPassed(string pciShieldappAppSuperUserId)
        {
            if (pciShieldappAppSuperUserId == null)
            {
                this._logger.LogError("PCIShieldSolutionuperUserId GetPCIShieldSolutionuperUserIdPassed.");
                throw new Exception("PCIShieldSolutionuperUserId  GetPCIShieldSolutionuperUserIdPassed.");
            }
            return pciShieldappAppSuperUserId;
        }
        public string GetTenantId()
        {
            var user = this._httpContextAccessor.HttpContext.User;
            var tenantIdClaim = user.FindFirst("TenantId");
            if (tenantIdClaim == null)
            {
                return "7dbcce64-324f-43aa-8d3c-f584503f6737";
                this._logger.LogError("Tenant ID claim not found in user's claims.");
            }
            return tenantIdClaim.Value;
        }
    }
}