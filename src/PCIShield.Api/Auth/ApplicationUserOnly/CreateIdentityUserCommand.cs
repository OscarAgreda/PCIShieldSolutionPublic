using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

using PCIShield.Infrastructure.Data;
using PCIShield.Infrastructure.Services;
using LanguageExt;
using Microsoft.AspNetCore.Identity;
using static LanguageExt.Prelude;

namespace PCIShield.Api.Auth.ApplicationUserOnly
{
    public class CreateIdentityUserCommand : AuthBusinessLogicSagaHandlerTemplate
    {
        private readonly UserManager<CustomPCIShieldUser> _userManager;
        private readonly RoleManager<IdentityRole> _roleManager;
        private readonly IdentityUserFactory _identityUserFactory;
        private readonly IAppLoggerService<CreateIdentityUserCommand> _logger;
        private readonly IEnumerable<IAuthObserver> _observers;

        public CreateIdentityUserCommand(
            UserManager<CustomPCIShieldUser> userManager,
            RoleManager<IdentityRole> roleManager,
            IdentityUserFactory identityUserFactory,
            IAppLoggerService<CreateIdentityUserCommand> logger,
            IEnumerable<IAuthObserver> observers)
        {
            _userManager = userManager;
            _roleManager = roleManager;
            _identityUserFactory = identityUserFactory;
            _logger = logger;
            _observers = observers;
        }

        protected override async Task<Either<Exception, Unit>> ExecuteSpecificAsync(
            AuthUpdateContext context,
            CancellationToken cancellationToken)
        {
            try
            {
                if (context is RegisterAuthContext registerContext)
                {
                    var user = _identityUserFactory.Create(registerContext.RegisterRequest);
                    var result = await _userManager.CreateAsync(user, registerContext.RegisterRequest.Password);

                    if (!result.Succeeded)
                    {
                        var errorMessage = string.Join(", ", result.Errors.Select(e => e.Description));
                        return Left<Exception, Unit>(
                            new InvalidOperationException($"Failed to create user: {errorMessage}"));
                    }
                    var roleName = registerContext.RegisterRequest.Role ?? "Merchant";
                    if (!await _roleManager.RoleExistsAsync(roleName))
                        await _roleManager.CreateAsync(new IdentityRole(roleName));

                    var roleResult = await _userManager.AddToRoleAsync(user, roleName);
                    if (!roleResult.Succeeded)
                        return Left<Exception, Unit>(
                            new InvalidOperationException(
                                $"Failed to assign role: {string.Join(", ", roleResult.Errors.Select(e => e.Description))}"));

                    context.CurrentIdentityUser = user;
                    NotifyObservers(user);
                    return Right<Exception, Unit>(unit);
                }

                return Right<Exception, Unit>(unit);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error creating identity user");
                return Left<Exception, Unit>(ex);
            }
        }

        private void NotifyObservers(CustomPCIShieldUser user) =>
            _observers.ToList().ForEach(observer => observer.Notify(user));
    }
}