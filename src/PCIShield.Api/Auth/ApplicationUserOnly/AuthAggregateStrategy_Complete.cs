using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

using Ardalis.GuardClauses;

using FastEndpoints;

using PCIShield.Domain.Entities;
using PCIShield.Domain.Events;
using PCIShield.Domain.Interfaces;
using PCIShield.Domain.Specifications;
using PCIShield.Infrastructure.Data;
using PCIShield.Infrastructure.Services;

using PCIShieldLib.SharedKernel.Interfaces;

using LanguageExt;

using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.IdentityModel.Tokens;

using static LanguageExt.Prelude;

namespace PCIShield.Api.Auth.ApplicationUserOnly
{
    public interface IAuthBusinessLogicExecuteSagaHandler
    {
        Task<Either<Exception, Unit>> ExecuteAuthBusinessLogicAsync(
            AuthUpdateContext context,
            CancellationToken cancellationToken);
    }

    public interface IAuthNotificationChannel
    {
        void SendNotification<T>(T entity) where T : class;
    }

    public interface IAuthObserver
    {
        void Notify<T>(T entity) where T : class;
    }

    public interface IBusinessLogicAuthDataSagaHandler
    {
        Task<Either<Exception, Unit>> ExecuteLoginAsync(LoginAuthContext context, CancellationToken cancellationToken);

        Task<Either<Exception, Unit>> ExecuteRegisterAsync(
            RegisterAuthContext context,
            CancellationToken cancellationToken);

        Task<Either<Exception, Unit>> ExecuteLogoutAsync(
            LogoutAuthContext context,
            CancellationToken cancellationToken);

        Task<Either<Exception, Unit>> ExecuteRefreshTokenAsync(
            RefreshTokenAuthContext context,
            CancellationToken cancellationToken);

        Task<Either<Exception, Unit>> ExecuteConfirmEmailAsync(
            ConfirmEmailAuthContext context,
            CancellationToken cancellationToken);

        Task<Either<Exception, Unit>> ExecuteForgotPasswordAsync(ForgotPasswordAuthContext context, CancellationToken cancellationToken);
        Task<Either<Exception, Unit>> ExecuteResetPasswordAsync(ResetPasswordAuthContext context, CancellationToken cancellationToken);

    }

    public interface IAuthUserIdentity
    {
        string UserId { get; }
        string UserName { get; }
        string Email { get; }
    }

    public interface IAuthUpdateContext
    {
        Guid OperationId { get; }
        CustomPCIShieldUser CurrentIdentityUser { get; set; }
        ApplicationUser CurrentApplicationUser { get; set; }
        string GeneratedToken { get; set; }
        string RefreshToken { get; set; }
        string TenantId { get; set; }
        string PCIShieldAppPowerUserId { get; set; }
        Either<Exception, List<Claim>> GeneratedClaims { get; set; }
        Either<Exception, Unit> ValidationResult { get; set; }
        Either<Exception, Unit> EmailConfirmationResult { get; set; }
    }
    public abstract class AuthBusinessLogicSagaHandlerTemplate : IAuthBusinessLogicExecuteSagaHandler
    {
        public async Task<Either<Exception, Unit>> ExecuteAuthBusinessLogicAsync(
            AuthUpdateContext context,
            CancellationToken cancellationToken)
        {
            return await PreExecutionAsync(context, cancellationToken)
                .BindAsync(async _ => await ExecuteSpecificAsync(context, cancellationToken))
                .BindAsync(async _ => await PostExecutionAsync(context, cancellationToken));
        }

        protected virtual Task<Either<Exception, Unit>> PreExecutionAsync(
            AuthUpdateContext context,
            CancellationToken cancellationToken) =>
            Task.FromResult(Right<Exception, Unit>(unit));

        protected abstract Task<Either<Exception, Unit>> ExecuteSpecificAsync(
            AuthUpdateContext context,
            CancellationToken cancellationToken);

        protected virtual Task<Either<Exception, Unit>> PostExecutionAsync(
            AuthUpdateContext context,
            CancellationToken cancellationToken) =>
            Task.FromResult(Right<Exception, Unit>(unit));
    }
    public class AuthNotifier : IAuthObserver, IAuthNotificationChannel
    {
        public void Notify<T>(T entity) where T : class => SendNotification(entity);

        public void SendNotification<T>(T entity) where T : class
        {
        }
    }
    public class AuthEventLogger : IAuthObserver
    {
        private readonly IAppLoggerService<AuthEventLogger> _logger;
        
        public AuthEventLogger(IAppLoggerService<AuthEventLogger> logger)
        {
            _logger = logger;
        }
        
        public void Notify<T>(T entity) where T : class
        {
            _logger.LogInformation($"Auth event: {typeof(T).Name} - {entity}");
        }
    }
    public class AuthNotificationChannel : IAuthNotificationChannel
    {
        private readonly IAppLoggerService<AuthNotificationChannel> _logger;
        
        public AuthNotificationChannel(IAppLoggerService<AuthNotificationChannel> logger)
        {
            _logger = logger;
        }
        
        public void SendNotification<T>(T entity) where T : class
        {
            _logger.LogInformation($"Notification sent for: {typeof(T).Name}");
        }
    }
    public class IdentityUserFactory : IEntityFactory<CustomPCIShieldUser>
    {
        public CustomPCIShieldUser Create(params object[] args)
        {
            var request = (RegisterRequest)args[0];
            return new CustomPCIShieldUser
            {
                UserName = request.Username,
                Email = request.Email,
                ApplicationUserId = Guid.NewGuid(),
                TenantId = Guid.NewGuid(),
                CreatedAt = DateTime.UtcNow,
                CreatedBy = Guid.Empty,
                FirstName = request.FirstName,
                LastName = request.LastName
            };
        }
    }

    public class ApplicationUserAuthFactory : IEntityFactory<ApplicationUser>
    {
        public ApplicationUser Create(params object[] args)
        {
            var identityUser = (CustomPCIShieldUser)args[0];
            var request = (RegisterRequest)args[1];
            var now = DateTime.UtcNow;

            return new ApplicationUser(
                applicationUserId: identityUser.ApplicationUserId,
                userName: request.Username,
                createdDate: now,
                createdBy: Guid.Empty,
                isLoginAllowed: true,
                failedLoginCount: 0,
                isUserApproved: false,
                isPhoneVerified: false,
                isEmailVerified: false,
                isLocked: false,
                isDeleted: false,
                isUserFullyRegistered: false,
                isBanned: false,
                isFullyRegistered: false,
                createdAt: now,
                tenantId: identityUser.TenantId);
        }
    }
    public class AuthUpdateContext : IAuthUpdateContext
    {
        public Guid OperationId { get; private set; } = Guid.NewGuid();
        public CustomPCIShieldUser CurrentIdentityUser { get; set; } = null!;
        public ApplicationUser CurrentApplicationUser { get; set; } = null!;
        public string GeneratedToken { get; set; } = string.Empty;
        public string RefreshToken { get; set; } = string.Empty;
        public string TenantId { get; set; } = string.Empty;
        public string PCIShieldAppPowerUserId { get; set; } = string.Empty;
        public Either<Exception, List<Claim>> GeneratedClaims { get; set; }
        public Either<Exception, Unit> ValidationResult { get; set; }
        public Either<Exception, Unit> EmailConfirmationResult { get; set; }
    }

    public class LoginAuthContext : AuthUpdateContext
    {
        public LoginRequest LoginRequest { get; set; } = null!;
        public bool IsFullyRegistered { get; set; }
        public HttpContext? HttpContext { get; set; }
    }

    public class RegisterAuthContext : AuthUpdateContext
    {
        public RegisterRequest RegisterRequest { get; set; } = null!;
        public string ConfirmationLink { get; set; } = string.Empty;
        public Microsoft.AspNetCore.Identity.ExternalLoginInfo? ExternalLoginInfo { get; set; }
    }

    public class LogoutAuthContext : AuthUpdateContext
    {
        public string UserId { get; set; } = string.Empty;
    }

    public class RefreshTokenAuthContext : AuthUpdateContext
    {
        public string RefreshTokenValue { get; set; } = string.Empty;
    }

    public class ConfirmEmailAuthContext : AuthUpdateContext
    {
        public string Token { get; set; } = string.Empty;
        public string Email { get; set; } = string.Empty;
    }
    public class ForgotPasswordAuthContext : AuthUpdateContext
    {
        public string Email { get; set; } = string.Empty;
        public string ResetPasswordUrl { get; set; } = string.Empty;
    }

    public class ResetPasswordAuthContext : AuthUpdateContext
    {
        public string Email { get; set; } = string.Empty;
        public string Token { get; set; } = string.Empty;
        public string NewPassword { get; set; } = string.Empty;
    }
    public class NullAuthUser : IAuthUserIdentity
    {
        public string UserId => string.Empty;
        public string UserName => string.Empty;
        public string Email => string.Empty;
    }

    public class AuthUserProxy : IAuthUserIdentity
    {
        private readonly CustomPCIShieldUser _user;

        public AuthUserProxy(CustomPCIShieldUser user) => _user = user;

        public string UserId => _user?.Id ?? string.Empty;
        public string UserName => _user?.UserName ?? string.Empty;
        public string Email => _user?.Email ?? string.Empty;
    }
}