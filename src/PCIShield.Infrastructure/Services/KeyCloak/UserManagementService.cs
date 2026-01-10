using System;
using System.Security.Claims;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
namespace PCIShield.Infrastructure.Services
{
    public class UserManagementService : IUserManagementService
    {
        private readonly IAppLoggerService<UserManagementService> _logger;
        private readonly IHttpContextAccessor _httpContextAccessor;
        public UserManagementService(IHttpContextAccessor httpContextAccessor, IAppLoggerService<UserManagementService> logger)
        {
            _httpContextAccessor = httpContextAccessor;
            _logger = logger;
        }
        public string GetUserId()
        {
            var userId = "954f5237-1489-89f0-2dba-4375ccc41091";
            if (userId != null)
            {
                return userId;
            }
            var user = _httpContextAccessor.HttpContext.User;
            var userIdClaim = user.FindFirst(ClaimTypes.NameIdentifier);
            if (userIdClaim == null)
            {
                _logger.LogError("User ID claim not found in user's claims.");
                throw new Exception("User ID claim not found in user's claims.");
            }
            userId = userIdClaim.Value;
            return userId;
        }
        public string GetJwtToken()
        {
            var jwtToken = "JustIneToken";
            if (jwtToken != null)
            {
                return jwtToken;
            }
            return jwtToken;
        }
    }
}
