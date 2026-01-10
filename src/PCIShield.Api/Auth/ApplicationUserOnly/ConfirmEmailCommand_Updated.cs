// Updated ConfirmEmailCommand for ApplicationUser-only implementation

using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

using PCIShield.Api.Auth.ApplicationUserOnly;
using PCIShield.Domain.Entities;
using PCIShield.Infrastructure.Data;
using PCIShield.Infrastructure.Services;

using PCIShieldLib.SharedKernel.Interfaces;

using LanguageExt;
using static LanguageExt.Prelude;

using Microsoft.AspNetCore.Identity;

public class ConfirmEmailCommand : AuthBusinessLogicSagaHandlerTemplate
{
    private readonly UserManager<CustomPCIShieldUser> _userManager;
    private readonly IRepository<ApplicationUser> _userRepository;
    private readonly IAppLoggerService<ConfirmEmailCommand> _logger;

    public ConfirmEmailCommand(
        UserManager<CustomPCIShieldUser> userManager,
        IRepository<ApplicationUser> userRepository,
        IAppLoggerService<ConfirmEmailCommand> logger)
    {
        _userManager = userManager;
        _userRepository = userRepository;
        _logger = logger;
    }

    protected override async Task<Either<Exception, Unit>> ExecuteSpecificAsync(
        AuthUpdateContext context,
        CancellationToken cancellationToken)
    {
        try
        {
            if (context is ConfirmEmailAuthContext confirmContext)
            {
                var user = await _userManager.FindByEmailAsync(confirmContext.Email);
                if (user == null) return Left<Exception, Unit>(new InvalidOperationException("User not found"));
                var result = await _userManager.ConfirmEmailAsync(user, confirmContext.Token);
                if (!result.Succeeded)
                    return Left<Exception, Unit>(
                        new InvalidOperationException(
                            $"Email confirmation failed: {string.Join(", ", result.Errors.Select(e => e.Description))}"));
                var userId = new Guid(user.Id);
                var applicationUser = await _userRepository.GetByIdAsync(userId, cancellationToken);
                if (applicationUser != null)
                {
                    applicationUser.SetIsEmailVerified(true);
                    applicationUser.SetIsLoginAllowed(true);
                    applicationUser.SetIsUserApproved(true);
                    applicationUser.SetIsPhoneVerified(true);
                    applicationUser.SetIsLocked(false);
                    applicationUser.SetIsDeleted(false);
                    applicationUser.SetUpdatedDate(DateTime.UtcNow);
                    var roles = await _userManager.GetRolesAsync(user);
                    if (roles.Contains("ComplianceOfficer") || roles.Contains("Admin"))
                    {
                        applicationUser.SetIsErpOwner(true);
                    }
                    
                    await _userRepository.UpdateAsync(applicationUser, cancellationToken);
                }

                context.CurrentIdentityUser = user;
                context.CurrentApplicationUser = applicationUser;
                return Right(unit);
            }

            return Right(unit);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error confirming email");
            return Left(ex);
        }
    }
}