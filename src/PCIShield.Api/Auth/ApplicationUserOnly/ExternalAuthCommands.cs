using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Net.Http;
using System.Security.Claims;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

using PCIShield.Api.Auth.ApplicationUserOnly;
using PCIShield.Domain.Entities;
using PCIShield.Infrastructure.Data;
using PCIShield.Infrastructure.Services;

using PCIShieldLib.SharedKernel.Interfaces;

using Google.Apis.Auth;

using LanguageExt;
using static LanguageExt.Prelude;

using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;

using Newtonsoft.Json.Linq;

namespace PCIShield.Api.Auth.ApplicationUserOnly
{
    public class GoogleLoginCallbackCommand : AuthBusinessLogicSagaHandlerTemplate
    {
        private readonly SignInManager<CustomPCIShieldUser> _signInManager;
        private readonly UserManager<CustomPCIShieldUser> _userManager;
        private readonly IRepository<ApplicationUser> _userRepository;
        private readonly IBusinessLogicAuthDataSagaHandler _authSagaHandler;
        private readonly IAppLoggerService<GoogleLoginCallbackCommand> _logger;

        public GoogleLoginCallbackCommand(
            SignInManager<CustomPCIShieldUser> signInManager,
            UserManager<CustomPCIShieldUser> userManager,
            IRepository<ApplicationUser> userRepository,
            IBusinessLogicAuthDataSagaHandler authSagaHandler,
            IAppLoggerService<GoogleLoginCallbackCommand> logger)
        {
            _signInManager = signInManager;
            _userManager = userManager;
            _userRepository = userRepository;
            _authSagaHandler = authSagaHandler;
            _logger = logger;
        }

        protected override async Task<Either<Exception, Unit>> ExecuteSpecificAsync(
            AuthUpdateContext context,
            CancellationToken cancellationToken)
        {
            try
            {
                if (context is GoogleLoginAuthContext googleContext)
                {
                    var info = await _signInManager.GetExternalLoginInfoAsync();
                    if (info == null)
                    {
                        return Left<Exception, Unit>(new InvalidOperationException("Error loading external login information"));
                    }
                    var signInResult = await _signInManager.ExternalLoginSignInAsync(
                        info.LoginProvider,
                        info.ProviderKey,
                        isPersistent: false,
                        bypassTwoFactor: true);

                    if (signInResult.Succeeded)
                    {
                        var user = await _userManager.FindByLoginAsync(info.LoginProvider, info.ProviderKey);
                        if (user != null)
                        {
                            var loginContext = new LoginAuthContext
                            {
                                LoginRequest = new LoginRequest 
                                { 
                                    Username = user.UserName ?? user.Email,
                                    Password = string.Empty
                                },
                                CurrentIdentityUser = user,
                                HttpContext = googleContext.HttpContext
                            };
                            await _authSagaHandler.ExecuteLoginAsync(loginContext, cancellationToken);
                            
                            googleContext.CurrentIdentityUser = user;
                            googleContext.GeneratedToken = loginContext.GeneratedToken;
                            googleContext.RefreshToken = loginContext.RefreshToken;
                            googleContext.CurrentApplicationUser = loginContext.CurrentApplicationUser;
                            googleContext.TenantId = loginContext.TenantId;
                            googleContext.IsFullyRegistered = loginContext.IsFullyRegistered;
                            googleContext.PCIShieldAppPowerUserId = loginContext.PCIShieldAppPowerUserId;
                        }

                        return Right(unit);
                    }

                    if (signInResult.IsLockedOut)
                    {
                        return Left<Exception, Unit>(new InvalidOperationException("User account is locked out"));
                    }

                    if (signInResult.IsNotAllowed)
                    {
                        return Left<Exception, Unit>(new InvalidOperationException("User is not allowed to sign in"));
                    }
                    var email = info.Principal.FindFirstValue(ClaimTypes.Email);
                    var name = info.Principal.FindFirstValue(ClaimTypes.Name);
                    var givenName = info.Principal.FindFirstValue(ClaimTypes.GivenName);
                    var surname = info.Principal.FindFirstValue(ClaimTypes.Surname);

                    if (string.IsNullOrEmpty(email))
                    {
                        return Left<Exception, Unit>(new InvalidOperationException("Email not provided by external provider"));
                    }
                    var existingUser = await _userManager.FindByEmailAsync(email);
                    if (existingUser != null)
                    {
                        var addLoginResult = await _userManager.AddLoginAsync(existingUser, info);
                        if (!addLoginResult.Succeeded)
                        {
                            return Left<Exception, Unit>(new InvalidOperationException(
                                $"Failed to link external login: {string.Join(", ", addLoginResult.Errors.Select(e => e.Description))}"));
                        }
                        await _signInManager.SignInAsync(existingUser, isPersistent: false);
                        var loginContext = new LoginAuthContext
                        {
                            LoginRequest = new LoginRequest 
                            { 
                                Username = existingUser.UserName ?? existingUser.Email,
                                Password = string.Empty
                            },
                            CurrentIdentityUser = existingUser,
                            HttpContext = googleContext.HttpContext
                        };

                        await _authSagaHandler.ExecuteLoginAsync(loginContext, cancellationToken);
                        
                        googleContext.CurrentIdentityUser = existingUser;
                        googleContext.GeneratedToken = loginContext.GeneratedToken;
                        googleContext.RefreshToken = loginContext.RefreshToken;
                        googleContext.CurrentApplicationUser = loginContext.CurrentApplicationUser;
                        googleContext.TenantId = loginContext.TenantId;
                        googleContext.IsFullyRegistered = loginContext.IsFullyRegistered;
                        googleContext.PCIShieldAppPowerUserId = loginContext.PCIShieldAppPowerUserId;

                        return Right(unit);
                    }
                    var registerContext = new RegisterAuthContext
                    {
                        RegisterRequest = new RegisterRequest
                        {
                            Email = email,
                            Username = email,
                            FirstName = givenName ?? name?.Split(' ').FirstOrDefault() ?? "Unknown",
                            LastName = surname ?? name?.Split(' ').LastOrDefault() ?? "User",
                            Password = GenerateRandomPassword(),
                            Phone = string.Empty,
                            Role = "Merchant"
                        },
                        ExternalLoginInfo = info
                    };
                    var registrationResult = await _authSagaHandler.ExecuteRegisterAsync(registerContext, cancellationToken);
                    
                    return registrationResult.Match(
                        Right: _ =>
                        {
                            var newUser = registerContext.CurrentIdentityUser;
                            if (newUser != null)
                            {
                                _userManager.AddLoginAsync(newUser, info).Wait();
                                var loginContext = new LoginAuthContext
                                {
                                    LoginRequest = new LoginRequest 
                                    { 
                                        Username = newUser.UserName ?? newUser.Email,
                                        Password = string.Empty
                                    },
                                    CurrentIdentityUser = newUser,
                                    HttpContext = googleContext.HttpContext
                                };

                                _authSagaHandler.ExecuteLoginAsync(loginContext, cancellationToken).Wait();
                                
                                googleContext.CurrentIdentityUser = newUser;
                                googleContext.GeneratedToken = loginContext.GeneratedToken;
                                googleContext.RefreshToken = loginContext.RefreshToken;
                                googleContext.CurrentApplicationUser = loginContext.CurrentApplicationUser;
                                googleContext.TenantId = loginContext.TenantId;
                                googleContext.IsFullyRegistered = loginContext.IsFullyRegistered;
                                googleContext.PCIShieldAppPowerUserId = loginContext.PCIShieldAppPowerUserId;
                            }
                            
                            return Right(unit);
                        },
                        Left: ex => Left<Exception, Unit>(ex)
                    );
                }

                return Right(unit);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error in Google login callback");
                return Left(ex);
            }
        }

        private string GenerateRandomPassword()
        {
            const string chars = "ABCDEFGHIJKLMNOPQRSTUVWXYZabcdefghijklmnopqrstuvwxyz0123456789!@#$%^&*()";
            var random = new Random();
            return new string(Enumerable.Repeat(chars, 16)
                .Select(s => s[random.Next(s.Length)]).ToArray());
        }
    }
    public class ValidateGoogleTokenCommand : AuthBusinessLogicSagaHandlerTemplate
    {
        private readonly UserManager<CustomPCIShieldUser> _userManager;
        private readonly IConfiguration _configuration;
        private readonly IBusinessLogicAuthDataSagaHandler _authSagaHandler;
        private readonly IAppLoggerService<ValidateGoogleTokenCommand> _logger;

        public ValidateGoogleTokenCommand(
            UserManager<CustomPCIShieldUser> userManager,
            IConfiguration configuration,
            IBusinessLogicAuthDataSagaHandler authSagaHandler,
            IAppLoggerService<ValidateGoogleTokenCommand> logger)
        {
            _userManager = userManager;
            _configuration = configuration;
            _authSagaHandler = authSagaHandler;
            _logger = logger;
        }

        protected override async Task<Either<Exception, Unit>> ExecuteSpecificAsync(
            AuthUpdateContext context,
            CancellationToken cancellationToken)
        {
            try
            {
                if (context is GoogleTokenAuthContext googleContext)
                {
                    var payload = await GoogleJsonWebSignature.ValidateAsync(
                        googleContext.TokenId,
                        new GoogleJsonWebSignature.ValidationSettings
                        {
                            Audience = new[] { _configuration["Authentication:Google:ClientId"] }
                        });

                    if (payload == null)
                    {
                        return Left<Exception, Unit>(new InvalidOperationException("Invalid Google token"));
                    }
                    var user = await _userManager.FindByEmailAsync(payload.Email);
                    if (user == null)
                    {
                        return Left<Exception, Unit>(new InvalidOperationException("User not found"));
                    }
                    var loginContext = new LoginAuthContext
                    {
                        LoginRequest = new LoginRequest 
                        { 
                            Username = user.UserName ?? user.Email,
                            Password = string.Empty
                        },
                        CurrentIdentityUser = user,
                        HttpContext = googleContext.HttpContext
                    };

                    var loginResult = await _authSagaHandler.ExecuteLoginAsync(loginContext, cancellationToken);
                    
                    return loginResult.Match(
                        Right: _ =>
                        {
                            googleContext.CurrentIdentityUser = user;
                            googleContext.GeneratedToken = loginContext.GeneratedToken;
                            googleContext.RefreshToken = loginContext.RefreshToken;
                            googleContext.CurrentApplicationUser = loginContext.CurrentApplicationUser;
                            return Right(unit);
                        },
                        Left: ex => Left<Exception, Unit>(ex)
                    );
                }

                return Right(unit);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error validating Google token");
                return Left(ex);
            }
        }
    }
    public class GoogleLoginAuthContext : AuthUpdateContext
    {
        public string? Code { get; set; }
        public string? State { get; set; }
        public Microsoft.AspNetCore.Http.HttpContext? HttpContext { get; set; }
        public string? GeneratedToken { get; set; }
        public string? RefreshToken { get; set; }
        public string? TenantId { get; set; }
        public bool IsFullyRegistered { get; set; }
        public string? PCIShieldAppPowerUserId { get; set; }
    }
    public class GoogleTokenAuthContext : AuthUpdateContext
    {
        public string TokenId { get; set; } = string.Empty;
        public Microsoft.AspNetCore.Http.HttpContext? HttpContext { get; set; }
        public string? GeneratedToken { get; set; }
        public string? RefreshToken { get; set; }
    }
    public class GoogleLoginRequest
    {
        public string TokenId { get; set; } = string.Empty;
    }
    public class AppleLoginCallbackCommand : AuthBusinessLogicSagaHandlerTemplate
    {
        private readonly SignInManager<CustomPCIShieldUser> _signInManager;
        private readonly UserManager<CustomPCIShieldUser> _userManager;
        private readonly IRepository<ApplicationUser> _userRepository;
        private readonly IBusinessLogicAuthDataSagaHandler _authSagaHandler;
        private readonly IConfiguration _configuration;
        private readonly IAppLoggerService<AppleLoginCallbackCommand> _logger;

        public AppleLoginCallbackCommand(
            SignInManager<CustomPCIShieldUser> signInManager,
            UserManager<CustomPCIShieldUser> userManager,
            IRepository<ApplicationUser> userRepository,
            IBusinessLogicAuthDataSagaHandler authSagaHandler,
            IConfiguration configuration,
            IAppLoggerService<AppleLoginCallbackCommand> logger)
        {
            _signInManager = signInManager;
            _userManager = userManager;
            _userRepository = userRepository;
            _authSagaHandler = authSagaHandler;
            _configuration = configuration;
            _logger = logger;
        }

        protected override async Task<Either<Exception, Unit>> ExecuteSpecificAsync(
            AuthUpdateContext context,
            CancellationToken cancellationToken)
        {
            try
            {
                if (context is AppleLoginAuthContext appleContext)
                {
                    var info = await _signInManager.GetExternalLoginInfoAsync();
                    if (info == null)
                    {
                        return Left<Exception, Unit>(new InvalidOperationException("Error loading external login information from Apple"));
                    }
                    var signInResult = await _signInManager.ExternalLoginSignInAsync(
                        info.LoginProvider,
                        info.ProviderKey,
                        isPersistent: false,
                        bypassTwoFactor: true);

                    if (signInResult.Succeeded)
                    {
                        var user = await _userManager.FindByLoginAsync(info.LoginProvider, info.ProviderKey);
                        if (user != null)
                        {
                            var loginContext = new LoginAuthContext
                            {
                                LoginRequest = new LoginRequest 
                                { 
                                    Username = user.UserName ?? user.Email,
                                    Password = string.Empty
                                },
                                CurrentIdentityUser = user,
                                HttpContext = appleContext.HttpContext
                            };
                            await _authSagaHandler.ExecuteLoginAsync(loginContext, cancellationToken);
                            
                            appleContext.CurrentIdentityUser = user;
                            appleContext.GeneratedToken = loginContext.GeneratedToken;
                            appleContext.RefreshToken = loginContext.RefreshToken;
                            appleContext.CurrentApplicationUser = loginContext.CurrentApplicationUser;
                            appleContext.TenantId = loginContext.TenantId;
                            appleContext.IsFullyRegistered = loginContext.IsFullyRegistered;
                            appleContext.PCIShieldAppPowerUserId = loginContext.PCIShieldAppPowerUserId;
                        }

                        return Right(unit);
                    }

                    if (signInResult.IsLockedOut)
                    {
                        return Left<Exception, Unit>(new InvalidOperationException("User account is locked out"));
                    }

                    if (signInResult.IsNotAllowed)
                    {
                        return Left<Exception, Unit>(new InvalidOperationException("User is not allowed to sign in"));
                    }
                    var email = info.Principal.FindFirstValue(ClaimTypes.Email) 
                        ?? info.Principal.FindFirstValue("http://schemas.xmlsoap.org/ws/2005/05/identity/claims/emailaddress");
                    var nameClaimValue = info.Principal.FindFirstValue("name");
                    string firstName = "Apple";
                    string lastName = "User";
                    
                    if (!string.IsNullOrEmpty(nameClaimValue))
                    {
                        try
                        {
                            var nameData = System.Text.Json.JsonSerializer.Deserialize<AppleNameData>(nameClaimValue);
                            firstName = nameData?.FirstName ?? firstName;
                            lastName = nameData?.LastName ?? lastName;
                        }
                        catch
                        {
                            firstName = nameClaimValue;
                        }
                    }

                    if (string.IsNullOrEmpty(email))
                    {
                        return Left<Exception, Unit>(new InvalidOperationException("Email not provided by Apple"));
                    }
                    var existingUser = await _userManager.FindByEmailAsync(email);
                    if (existingUser != null)
                    {
                        var addLoginResult = await _userManager.AddLoginAsync(existingUser, info);
                        if (!addLoginResult.Succeeded)
                        {
                            return Left<Exception, Unit>(new InvalidOperationException(
                                $"Failed to link Apple login: {string.Join(", ", addLoginResult.Errors.Select(e => e.Description))}"));
                        }
                        await _signInManager.SignInAsync(existingUser, isPersistent: false);
                        var loginContext = new LoginAuthContext
                        {
                            LoginRequest = new LoginRequest 
                            { 
                                Username = existingUser.UserName ?? existingUser.Email,
                                Password = string.Empty
                            },
                            CurrentIdentityUser = existingUser,
                            HttpContext = appleContext.HttpContext
                        };

                        await _authSagaHandler.ExecuteLoginAsync(loginContext, cancellationToken);
                        
                        appleContext.CurrentIdentityUser = existingUser;
                        appleContext.GeneratedToken = loginContext.GeneratedToken;
                        appleContext.RefreshToken = loginContext.RefreshToken;
                        appleContext.CurrentApplicationUser = loginContext.CurrentApplicationUser;
                        appleContext.TenantId = loginContext.TenantId;
                        appleContext.IsFullyRegistered = loginContext.IsFullyRegistered;
                        appleContext.PCIShieldAppPowerUserId = loginContext.PCIShieldAppPowerUserId;

                        return Right(unit);
                    }
                    var registerContext = new RegisterAuthContext
                    {
                        RegisterRequest = new RegisterRequest
                        {
                            Email = email,
                            Username = email,
                            FirstName = firstName,
                            LastName = lastName,
                            Password = GenerateRandomPassword(),
                            Phone = string.Empty,
                            Role = "Merchant"
                        },
                        ExternalLoginInfo = info
                    };
                    var registrationResult = await _authSagaHandler.ExecuteRegisterAsync(registerContext, cancellationToken);
                    
                    return registrationResult.Match(
                        Right: _ =>
                        {
                            var newUser = registerContext.CurrentIdentityUser;
                            if (newUser != null)
                            {
                                _userManager.AddLoginAsync(newUser, info).Wait();
                                var loginContext = new LoginAuthContext
                                {
                                    LoginRequest = new LoginRequest 
                                    { 
                                        Username = newUser.UserName ?? newUser.Email,
                                        Password = string.Empty
                                    },
                                    CurrentIdentityUser = newUser,
                                    HttpContext = appleContext.HttpContext
                                };

                                _authSagaHandler.ExecuteLoginAsync(loginContext, cancellationToken).Wait();
                                
                                appleContext.CurrentIdentityUser = newUser;
                                appleContext.GeneratedToken = loginContext.GeneratedToken;
                                appleContext.RefreshToken = loginContext.RefreshToken;
                                appleContext.CurrentApplicationUser = loginContext.CurrentApplicationUser;
                                appleContext.TenantId = loginContext.TenantId;
                                appleContext.IsFullyRegistered = loginContext.IsFullyRegistered;
                                appleContext.PCIShieldAppPowerUserId = loginContext.PCIShieldAppPowerUserId;
                            }
                            
                            return Right(unit);
                        },
                        Left: ex => Left<Exception, Unit>(ex)
                    );
                }

                return Right(unit);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error in Apple login callback");
                return Left(ex);
            }
        }

        private string GenerateRandomPassword()
        {
            const string chars = "ABCDEFGHIJKLMNOPQRSTUVWXYZabcdefghijklmnopqrstuvwxyz0123456789!@#$%^&*()";
            var random = new Random();
            return new string(Enumerable.Repeat(chars, 16)
                .Select(s => s[random.Next(s.Length)]).ToArray());
        }

        private class AppleNameData
        {
            public string FirstName { get; set; } = string.Empty;
            public string LastName { get; set; } = string.Empty;
        }
    }
    public class ValidateAppleTokenCommand : AuthBusinessLogicSagaHandlerTemplate
    {
        private readonly UserManager<CustomPCIShieldUser> _userManager;
        private readonly IConfiguration _configuration;
        private readonly IBusinessLogicAuthDataSagaHandler _authSagaHandler;
        private readonly IAppLoggerService<ValidateAppleTokenCommand> _logger;
        private readonly HttpClient _httpClient;

        public ValidateAppleTokenCommand(
            UserManager<CustomPCIShieldUser> userManager,
            IConfiguration configuration,
            IBusinessLogicAuthDataSagaHandler authSagaHandler,
            IAppLoggerService<ValidateAppleTokenCommand> logger,
            HttpClient httpClient)
        {
            _userManager = userManager;
            _configuration = configuration;
            _authSagaHandler = authSagaHandler;
            _logger = logger;
            _httpClient = httpClient;
        }

        protected override async Task<Either<Exception, Unit>> ExecuteSpecificAsync(
            AuthUpdateContext context,
            CancellationToken cancellationToken)
        {
            try
            {
                if (context is AppleTokenAuthContext appleContext)
                {
                    var isValid = await ValidateAppleIdToken(appleContext.IdentityToken);
                    if (!isValid)
                    {
                        return Left<Exception, Unit>(new InvalidOperationException("Invalid Apple ID token"));
                    }
                    var handler = new JwtSecurityTokenHandler();
                    var jsonToken = handler.ReadJwtToken(appleContext.IdentityToken);
                    
                    var email = jsonToken.Claims.FirstOrDefault(c => c.Type == "email")?.Value;
                    var sub = jsonToken.Claims.FirstOrDefault(c => c.Type == "sub")?.Value;
                    
                    if (string.IsNullOrEmpty(email) || string.IsNullOrEmpty(sub))
                    {
                        return Left<Exception, Unit>(new InvalidOperationException("Email or user ID not found in token"));
                    }
                    var user = await _userManager.FindByEmailAsync(email);
                    if (user == null)
                    {
                        return Left<Exception, Unit>(new InvalidOperationException("User not found"));
                    }
                    var loginContext = new LoginAuthContext
                    {
                        LoginRequest = new LoginRequest 
                        { 
                            Username = user.UserName ?? user.Email,
                            Password = string.Empty
                        },
                        CurrentIdentityUser = user,
                        HttpContext = appleContext.HttpContext
                    };

                    var loginResult = await _authSagaHandler.ExecuteLoginAsync(loginContext, cancellationToken);
                    
                    return loginResult.Match(
                        Right: _ =>
                        {
                            appleContext.CurrentIdentityUser = user;
                            appleContext.GeneratedToken = loginContext.GeneratedToken;
                            appleContext.RefreshToken = loginContext.RefreshToken;
                            appleContext.CurrentApplicationUser = loginContext.CurrentApplicationUser;
                            return Right(unit);
                        },
                        Left: ex => Left<Exception, Unit>(ex)
                    );
                }

                return Right(unit);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error validating Apple token");
                return Left(ex);
            }
        }

        private async Task<bool> ValidateAppleIdToken(string identityToken)
        {
            try
            {
                var response = await _httpClient.GetStringAsync("https://appleid.apple.com/auth/keys");
                var keys = System.Text.Json.JsonSerializer.Deserialize<AppleKeys>(response);
                var handler = new JwtSecurityTokenHandler();
                var jsonToken = handler.ReadJwtToken(identityToken);
                if (jsonToken.Issuer != "https://appleid.apple.com")
                    return false;
                    
                var aud = jsonToken.Claims.FirstOrDefault(c => c.Type == "aud")?.Value;
                if (aud != _configuration["Authentication:Apple:ServiceId"])
                    return false;
                    
                if (jsonToken.ValidTo < DateTime.UtcNow)
                    return false;
                
                return true;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error validating Apple ID token");
                return false;
            }
        }

        private class AppleKeys
        {
            public List<AppleKey> Keys { get; set; } = new();
        }

        private class AppleKey
        {
            public string Kty { get; set; } = string.Empty;
            public string Kid { get; set; } = string.Empty;
            public string Use { get; set; } = string.Empty;
            public string Alg { get; set; } = string.Empty;
            public string N { get; set; } = string.Empty;
            public string E { get; set; } = string.Empty;
        }
    }
    public class AppleLoginAuthContext : AuthUpdateContext
    {
        public string? Code { get; set; }
        public string? State { get; set; }
        public string? User { get; set; }
        public Microsoft.AspNetCore.Http.HttpContext? HttpContext { get; set; }
        public string? GeneratedToken { get; set; }
        public string? RefreshToken { get; set; }
        public string? TenantId { get; set; }
        public bool IsFullyRegistered { get; set; }
        public string? PCIShieldAppPowerUserId { get; set; }
    }
    public class AppleTokenAuthContext : AuthUpdateContext
    {
        public string IdentityToken { get; set; } = string.Empty;
        public string? AuthorizationCode { get; set; }
        public string? User { get; set; }
        public Microsoft.AspNetCore.Http.HttpContext? HttpContext { get; set; }
        public string? GeneratedToken { get; set; }
        public string? RefreshToken { get; set; }
    }
    public class AppleLoginRequest
    {
        public string IdentityToken { get; set; } = string.Empty;
        public string? AuthorizationCode { get; set; }
        public string? User { get; set; }
    }
}