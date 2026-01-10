using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using PCIShield.Domain.Entities;
using PCIShield.Infrastructure.Services;
using PCIShieldLib.SharedKernel.Interfaces;
using LanguageExt;
using static LanguageExt.Prelude;

namespace PCIShield.Api.Auth.ApplicationUserOnly
{
    public class CreateApplicationUserCommand : AuthBusinessLogicSagaHandlerTemplate
    {
        private readonly IRepository<ApplicationUser> _userRepository;
        private readonly ApplicationUserAuthFactory _applicationUserFactory;
        private readonly IAppLoggerService<CreateApplicationUserCommand> _logger;
        private readonly IEnumerable<IAuthObserver> _observers;

        public CreateApplicationUserCommand(
            IRepository<ApplicationUser> userRepository,
            ApplicationUserAuthFactory applicationUserFactory,
            IAppLoggerService<CreateApplicationUserCommand> logger,
            IEnumerable<IAuthObserver> observers)
        {
            _userRepository = userRepository;
            _applicationUserFactory = applicationUserFactory;
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
                    var applicationUser = _applicationUserFactory.Create(
                        context.CurrentIdentityUser,
                        registerContext.RegisterRequest);
                    applicationUser.SetEmail(registerContext.RegisterRequest.Email);
                    applicationUser.SetFirstName(registerContext.RegisterRequest.FirstName);
                    applicationUser.SetLastName(registerContext.RegisterRequest.LastName);
                    applicationUser.SetPhone(registerContext.RegisterRequest.Phone);

                    _userRepository.BeginTransaction();
                    await _userRepository.AddAsync(applicationUser, cancellationToken);
                    _userRepository.CommitTransaction();

                    context.CurrentApplicationUser = applicationUser;
                    NotifyObservers(applicationUser);
                    return Right<Exception, Unit>(unit);
                }

                return Right<Exception, Unit>(unit);
            }
            catch (Exception ex)
            {
                _userRepository.RollbackTransaction();
                _logger.LogError(ex, "Error creating application user");
                return Left<Exception, Unit>(ex);
            }
        }

        private void NotifyObservers(ApplicationUser user) =>
            _observers.ToList().ForEach(observer => observer.Notify(user));
    }
}