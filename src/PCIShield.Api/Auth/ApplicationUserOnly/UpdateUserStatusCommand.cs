using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using PCIShield.Domain.Entities;
using PCIShield.Infrastructure.Services;
using PCIShieldLib.SharedKernel.Interfaces;
using LanguageExt;
using Microsoft.AspNetCore.Http;
using static LanguageExt.Prelude;

namespace PCIShield.Api.Auth.ApplicationUserOnly
{
    public class UpdateUserStatusCommand : AuthBusinessLogicSagaHandlerTemplate
    {
        private readonly IRepository<ApplicationUser> _userRepository;
        private readonly IHttpContextAccessor _httpContextAccessor;
        private readonly IAppLoggerService<UpdateUserStatusCommand> _logger;

        public UpdateUserStatusCommand(
            IRepository<ApplicationUser> userRepository,
            IHttpContextAccessor httpContextAccessor,
            IAppLoggerService<UpdateUserStatusCommand> logger)
        {
            _userRepository = userRepository;
            _httpContextAccessor = httpContextAccessor;
            _logger = logger;
        }

        protected override async Task<Either<Exception, Unit>> ExecuteSpecificAsync(
            AuthUpdateContext context,
            CancellationToken cancellationToken)
        {
            try
            {
                if (context is LoginAuthContext loginContext && context.CurrentApplicationUser != null)
                {
                    var applicationUser = context.CurrentApplicationUser;
                    applicationUser.SetLastLogin(DateTime.UtcNow);
                    applicationUser.SetAvailabilityRank("Online");
                    applicationUser.SetIsOnline(true);
                    applicationUser.SetIsLoggedIntoApp(true);
                    applicationUser.SetTimeLastLoggedToApp(DateTime.UtcNow);
                    applicationUser.SetTimeLastSignalrPing(DateTime.UtcNow);
                    string clientIpAddress = "Unknown";
                    var httpContext = loginContext.HttpContext ?? _httpContextAccessor.HttpContext;
                    
                    if (httpContext != null)
                    {
                        var forwardedFor = httpContext.Request.Headers["X-Forwarded-For"].FirstOrDefault();
                        if (!string.IsNullOrWhiteSpace(forwardedFor))
                        {
                            clientIpAddress = forwardedFor.Split(',').FirstOrDefault()?.Trim() ?? "Unknown";
                        }
                        else if (httpContext.Connection.RemoteIpAddress != null)
                        {
                            clientIpAddress = httpContext.Connection.RemoteIpAddress.ToString();
                            if (clientIpAddress == "::1")
                            {
                                clientIpAddress = "127.0.0.1";
                            }
                        }
                    }
                    
                    applicationUser.SetLastLoginIP(clientIpAddress);
                    
                    _logger.LogInformation("User {UserId} logged in from IP: {IpAddress}", 
                        applicationUser.ApplicationUserId, 
                        clientIpAddress);

                    await _userRepository.UpdateAsync(applicationUser, cancellationToken);
                    return Right<Exception, Unit>(unit);
                }

                return Right<Exception, Unit>(unit);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error updating user status");
                return Left<Exception, Unit>(ex);
            }
        }
    }
}