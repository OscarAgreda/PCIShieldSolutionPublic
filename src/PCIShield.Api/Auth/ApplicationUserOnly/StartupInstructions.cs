using System;

using PCIShield.Api.CQRS;

using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace PCIShield.Api.Auth.ApplicationUserOnly
{
    public static class AuthenticationStrategyServiceCollectionExtensions
    {
        public static IServiceCollection AddAuthenticationStrategy(
            this IServiceCollection services,
            IConfiguration configuration)
        {
            services.AddScoped<IdentityUserFactory>();
            services.AddScoped<ApplicationUserAuthFactory>();
            services.AddScoped<IClaimsGenerationService, ClaimsGenerationService>();
            services.AddScoped<ValidateCredentialsCommand>();
            services.AddScoped<CreateIdentityUserCommand>();
            services.AddScoped<GenerateTenantIdCommand>();
            services.AddScoped<GenerateClaimsCommand>();
            services.AddScoped<GenerateJwtTokenCommand>();
            services.AddScoped<GenerateRefreshTokenCommand>();
            services.AddScoped<CreateApplicationUserCommand>();
            services.AddScoped<PreLoadCacheCommand>();
            services.AddScoped<SendEmailConfirmationCommand>();
            services.AddScoped<LoadUserDataForLoginCommand>();
            services.AddScoped<UpdateUserStatusCommand>();
            services.AddScoped<ConfirmEmailCommand>();
            services.AddScoped<LogoutUserCommand>();
            services.AddScoped<RefreshTokenCommand>();

            services.AddScoped<ForgotPasswordCommand>();
            services.AddScoped<ResetPasswordCommand>();
            services.AddScoped<GoogleLoginCallbackCommand>();
            services.AddScoped<ValidateGoogleTokenCommand>();
            services.AddScoped<AppleLoginCallbackCommand>();
            services.AddScoped<ValidateAppleTokenCommand>();
            services.AddScoped<IAuthObserver, AuthNotifier>();
            services.AddScoped<IAuthNotificationChannel, AuthNotifier>();
            services.AddScoped<AuthNotifier>();
            services.AddScoped<IBusinessLogicAuthDataSagaHandler, BusinessLogicAuthDataSagaHandler>();
            services.AddScoped<NullAuthUser>();
            services.AddScoped<IAuthUserIdentity>(provider => provider.GetService<NullAuthUser>()!);

            return services;
        }
        public class AuthenticationStrategyOptions
        {
            public bool EnableMerchantRegistration { get; set; } = true;
            public bool EnableComplianceOfficerRegistration { get; set; } = true;
            public bool EnableEmailConfirmation { get; set; } = true;
            public bool EnableCachePreloading { get; set; } = true;
            public bool EnableTwoFactorAuthentication { get; set; } = false;
            public bool EnableRefreshTokenRotation { get; set; } = true;
            public TimeSpan TokenExpiration { get; set; } = TimeSpan.FromDays(1);
            public TimeSpan RefreshTokenExpiration { get; set; } = TimeSpan.FromDays(7);
        }
    }
}
