using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;

using LanguageExt;

using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection;

using PCIShield.Api.Auth.ApplicationUserOnly;
using PCIShield.Domain.Entities;
using PCIShield.Domain.Interfaces;
using PCIShield.Infrastructure.Data;
using PCIShield.Infrastructure.Services;

using PCIShieldLib.SharedKernel.Interfaces;

using static LanguageExt.Prelude;

namespace PCIShield.Api.Auth
{

    public class ForgotPasswordRequest
    {
        [Required]
        [EmailAddress]
        [StringLength(100)]
        public string Email { get; set; } = string.Empty;
    }

    public class ResetPasswordRequest
    {
        [Required]
        [EmailAddress]
        [StringLength(100)]
        public string Email { get; set; } = string.Empty;

        [Required]
        public string Token { get; set; } = string.Empty;

        [Required]
        [StringLength(100, MinimumLength = 6)]
        public string Password { get; set; } = string.Empty;
    }
    public class RegisterRequest
    {
        [Required]
        [StringLength(50)]
        public string Username { get; set; } = string.Empty;

        [Required]
        [EmailAddress]
        [StringLength(100)]
        public string Email { get; set; } = string.Empty;

        [Required]
        [StringLength(100, MinimumLength = 6)]
        public string Password { get; set; } = string.Empty;

        [Required]
        [StringLength(50)]
        public string FirstName { get; set; } = string.Empty;

        [Required]
        [StringLength(50)]
        public string LastName { get; set; } = string.Empty;

        [Phone]
        [StringLength(20)]
        public string Phone { get; set; } = string.Empty;

        [StringLength(20)]
        public string Role { get; set; } = "Merchant";
    }

    public class LoginRequest
    {
        [Required]
        [StringLength(50)]
        public string Username { get; set; } = string.Empty;

        [Required]
        [StringLength(100)]
        public string Password { get; set; } = string.Empty;

        [StringLength(6)]
        public string? TwoFactorCode { get; set; } = string.Empty;

        [StringLength(20)]
        public string? TwoFactorRecoveryCode { get; set; } = string.Empty;
    }

    public class LogoutRequest
    {
        [Required]
        public string UserId { get; set; } = string.Empty;
    }

    public class RefreshTokenRequest
    {
        [Required]
        public string RefreshToken { get; set; } = string.Empty;
    }

    public class ValidateTokenRequest
    {
        [Required]
        public string Token { get; set; } = string.Empty;
    }
    public class AuthResponse
    {
        public bool IsSuccess { get; set; }
        public string Message { get; set; } = string.Empty;
        public string? ErrorMessage { get; set; }
        public object? Data { get; set; }
    }

    public class LoginResponse
    {
        public ApplicationUser? ApplicationUser { get; set; }
        public string? TenantId { get; set; }
        public bool IsFullyRegistered { get; set; }
        public string Token { get; set; } = string.Empty;
        public string RefreshToken { get; set; } = string.Empty;
        public bool IsValidToken { get; set; }
        public string PCIShieldAppPowerUserId { get; set; } = string.Empty;
        public string Role { get; set; } = string.Empty;
        public string UserType { get; internal set; }
    }

    public class TokenValidationResponse
    {
        public bool IsValidToken { get; set; }
        public string? Error { get; set; }
        public string? UserId { get; set; }
        public string? ApplicationUserId { get; set; }
    }
    [ApiController]

    public class AccountAuthV2Controller : ControllerBase
    {
        private readonly IAuthRepository<CustomPCIShieldUser> _identityUserRepository;

        private readonly IAuthRepository<CustomPCIShieldUser> _NoticeThatThisIsHowIcanUseTheEFAuthRepositoties;

        private readonly IServiceScopeFactory _serviceScopeFactory;
        private readonly PCIShieldLib.SharedKernel.Interfaces.IRepository<ComplianceOfficer> _complianceOfficerDbRepository;
        private readonly IConfiguration _configuration;

        private readonly IReadInMemoryRepository<Domain.Entities.ComplianceOfficer> _complianceOfficerCacheRepository;

        private readonly PCIShieldLib.SharedKernel.Interfaces.IRepository<Merchant> _merchantDbRepository;
        private readonly IMimeKitEmailSender _emailGmailService;
        private readonly IKeyCloakTenantService _iKeyCloakTenantService;
        private readonly IAppLoggerService<AccountAuthV2Controller> _logger;
        private readonly RoleManager<IdentityRole> _roleManager;

        private readonly PCIShieldLib.SharedKernel.Interfaces.IRepository<ApplicationUser> _userDbRepository;
        private readonly UserManager<CustomPCIShieldUser> _userManager;
        public AccountAuthV2Controller(UserManager<CustomPCIShieldUser> userManager, RoleManager<IdentityRole> roleManager,
            IKeyCloakTenantService iKeyCloakTenantService,
            IServiceScopeFactory serviceScopeFactory,
         
            IConfiguration configuration, IAppLoggerService<AccountAuthV2Controller> logger,
            IMimeKitEmailSender emailService,
            IAuthRepository<CustomPCIShieldUser> identityUserRepository,
            IReadInMemoryRepository<Domain.Entities.ComplianceOfficer> complianceOfficerCacheRepository,
            PCIShieldLib.SharedKernel.Interfaces.IRepository<ApplicationUser> userDbRepository,
            PCIShieldLib.SharedKernel.Interfaces.IRepository<Merchant> merchantDbRepository,

            PCIShieldLib.SharedKernel.Interfaces.IRepository<ComplianceOfficer> complianceOfficerDbRepository,
            IBusinessLogicAuthDataSagaHandler authSagaHandler)
        {

            _complianceOfficerCacheRepository = complianceOfficerCacheRepository;
          
            _serviceScopeFactory = serviceScopeFactory;
            _identityUserRepository = identityUserRepository;
            _iKeyCloakTenantService = iKeyCloakTenantService;

            _emailGmailService = emailService;
            _merchantDbRepository = merchantDbRepository;
            _userManager = userManager;
            _configuration = configuration;
            _logger = logger;
            _roleManager = roleManager;
            _userDbRepository = userDbRepository;
            _complianceOfficerDbRepository = complianceOfficerDbRepository;
            _authSagaHandler = authSagaHandler ?? throw new ArgumentNullException(nameof(authSagaHandler));
        }
        private readonly IBusinessLogicAuthDataSagaHandler _authSagaHandler;
        [HttpPost("register")]
        public async Task<IActionResult> RegisterUser([FromBody] RegisterRequest model, CancellationToken cancellationToken = default)
        {

            if (!ModelState.IsValid)
            {
                _logger.LogError("Invalid registration request for user: {Username}", model.Username);
                return BadRequest(new AuthResponse
                {
                    IsSuccess = false,
                    Message = "Invalid request",
                    ErrorMessage = string.Join(", ", ModelState.Values.SelectMany(v => v.Errors).Select(e => e.ErrorMessage))
                });
            }
            if (string.IsNullOrWhiteSpace(model.Role))
            {
                model.Role = "StandardUser";
            }

            var context = new RegisterAuthContext
            {
                RegisterRequest = model
            };

            var result = await _authSagaHandler.ExecuteRegisterAsync(context, cancellationToken);

            return result.Match<IActionResult>(
                Right: _ =>
                {
                    _logger.LogInformation("User {Username} registered successfully with role {Role}", model.Username, model.Role);
                    return Ok(new AuthResponse
                    {
                        IsSuccess = true,
                        Message = "User registered successfully!",
                        Data = new
                        {
                            Username = model.Username,
                            Email = model.Email,
                            Role = model.Role
                        }
                    });
                },
                Left: ex =>
                {
                    _logger.LogError(ex, "User registration failed for {Username}", model.Username);
                    return StatusCode(500, new AuthResponse
                    {
                        IsSuccess = false,
                        Message = "Failed to register user",
                        ErrorMessage = ex.Message
                    });
                });
        }
        [HttpPost("api/auth/v2/login")]
        public async Task<IActionResult> LoginUser([FromBody] LoginRequest model, CancellationToken cancellationToken = default)
        {

            if (!ModelState.IsValid)
            {
                _logger.LogError("Invalid login request for user: {Username}", model.Username);
                return BadRequest(new AuthResponse
                {
                    IsSuccess = false,
                    Message = "Invalid request"
                });
            }

            var context = new LoginAuthContext
            {
                LoginRequest = model,
                HttpContext = HttpContext
            };

            var result = await _authSagaHandler.ExecuteLoginAsync(context, cancellationToken);

            return result.Match<IActionResult>(
                Right: _ =>
                {
                    _logger.LogInformation("User {Username} logged in successfully", model.Username);
                    
                    var roles = (_authSagaHandler is BusinessLogicAuthDataSagaHandler sagaHandler && context.CurrentIdentityUser != null)
                                ? (_userManager.GetRolesAsync(context.CurrentIdentityUser).Result).FirstOrDefault() ?? string.Empty
                                : string.Empty;

                    var response = new LoginResponse
                    {
                        ApplicationUser = context.CurrentApplicationUser,
                        TenantId = context.TenantId,
                        IsFullyRegistered = context.IsFullyRegistered,
                        Token = context.GeneratedToken,
                        RefreshToken = context.RefreshToken,
                        IsValidToken = !string.IsNullOrEmpty(context.GeneratedToken),
                        PCIShieldAppPowerUserId = context.PCIShieldAppPowerUserId,
                        Role = roles
                    };

                    return Ok(response);
                },
                Left: ex =>
                {
                    _logger.LogError(ex, "User login failed for {Username}", model.Username);
                    return Unauthorized(new AuthResponse
                    {
                        IsSuccess = false,
                        Message = "Login failed",
                        ErrorMessage = ex.Message
                    });
                });
        }
        [HttpPost("logout_v2")]
        public async Task<IActionResult> LogoutV2([FromBody] LogoutRequest model, CancellationToken cancellationToken = default)
        {

            if (!ModelState.IsValid)
            {
                return BadRequest(new AuthResponse
                {
                    IsSuccess = false,
                    Message = "Invalid request"
                });
            }

            var context = new LogoutAuthContext
            {
                UserId = model.UserId
            };

            var result = await _authSagaHandler.ExecuteLogoutAsync(context, cancellationToken);

            return result.Match<IActionResult>(
                Right: _ =>
                {
                    _logger.LogInformation("User {UserId} logged out successfully", model.UserId);
                    return Ok(new AuthResponse
                    {
                        IsSuccess = true,
                        Message = "Logout successful"
                    });
                },
                Left: ex =>
                {
                    _logger.LogError(ex, "Logout failed for user {UserId}", model.UserId);
                    return StatusCode(500, new AuthResponse
                    {
                        IsSuccess = false,
                        Message = "Logout failed",
                        ErrorMessage = ex.Message
                    });
                });
        }
        [HttpPost("refresh_token_v2")]
        public async Task<IActionResult> RefreshTokenV2([FromBody] RefreshTokenRequest model, CancellationToken cancellationToken = default)
        {

            if (!ModelState.IsValid || string.IsNullOrWhiteSpace(model.RefreshToken))
            {
                return BadRequest(new AuthResponse
                {
                    IsSuccess = false,
                    Message = "Invalid refresh token request"
                });
            }

            var context = new RefreshTokenAuthContext
            {
                RefreshTokenValue = model.RefreshToken
            };

            var result = await _authSagaHandler.ExecuteRefreshTokenAsync(context, cancellationToken);

            return result.Match<IActionResult>(
                Right: _ =>
                {
                    _logger.LogInformation("Token refreshed successfully");
                    return Ok(new
                    {
                        Token = context.GeneratedToken,
                        RefreshToken = context.RefreshToken,
                        IsValidToken = true
                    });
                },
                Left: ex =>
                {
                    _logger.LogError(ex, "Token refresh failed");
                    return Unauthorized(new AuthResponse
                    {
                        IsSuccess = false,
                        Message = "Token refresh failed",
                        ErrorMessage = ex.Message
                    });
                });
        }
        [HttpGet("confirm_email_v2")]
        public async Task<IActionResult> ConfirmEmailV2([FromQuery] string token, [FromQuery] string email, CancellationToken cancellationToken = default)
        {

            if (string.IsNullOrWhiteSpace(token) || string.IsNullOrWhiteSpace(email))
            {
                return BadRequest(new AuthResponse
                {
                    IsSuccess = false,
                    Message = "Invalid email confirmation parameters"
                });
            }

            var context = new ConfirmEmailAuthContext
            {
                Token = token,
                Email = email
            };

            var result = await _authSagaHandler.ExecuteConfirmEmailAsync(context, cancellationToken);

            return result.Match<IActionResult>(
                Right: _ =>
                {
                    _logger.LogInformation("Email confirmed successfully for {Email}", email);
                    return Ok(new AuthResponse
                    {
                        IsSuccess = true,
                        Message = "Email confirmed successfully"
                    });
                },
                Left: ex =>
                {
                    _logger.LogError(ex, "Email confirmation failed for {Email}", email);
                    return BadRequest(new AuthResponse
                    {
                        IsSuccess = false,
                        Message = "Email confirmation failed",
                        ErrorMessage = ex.Message
                    });
                });
        }
        [HttpPost("validate_token_v2")]
        public async Task<IActionResult> ValidateTokenV2([FromBody] ValidateTokenRequest model, CancellationToken cancellationToken = default)
        {
            if (string.IsNullOrWhiteSpace(model.Token))
            {
                return BadRequest(new TokenValidationResponse
                {
                    IsValidToken = false,
                    Error = "Token is required"
                });
            }

            try
            {
                var tokenHandler = new System.IdentityModel.Tokens.Jwt.JwtSecurityTokenHandler();
                var validationParameters = new Microsoft.IdentityModel.Tokens.TokenValidationParameters
                {
                    ValidateIssuerSigningKey = true,
                    IssuerSigningKey = new Microsoft.IdentityModel.Tokens.SymmetricSecurityKey(
                        System.Text.Encoding.UTF8.GetBytes(_configuration["JwtSettings:PCISHIELDAPP_SECRET_KEY"] ?? "")),
                    ValidateIssuer = true,
                    ValidIssuer = _configuration["JwtSettings:PCISHIELDAPP_ISSUER"],
                    ValidateAudience = true,
                    ValidAudience = _configuration["JwtSettings:PCISHIELDAPP_AUDIENCE"],
                    ValidateLifetime = true,
                    ClockSkew = TimeSpan.Zero
                };

                var principal = tokenHandler.ValidateToken(model.Token, validationParameters, out var validatedToken);
                var userIdClaim = principal.Claims.FirstOrDefault(c => c.Type == System.Security.Claims.ClaimTypes.NameIdentifier);
                var applicationUserIdClaim = principal.Claims.FirstOrDefault(c => c.Type == "ApplicationUserId");

                if (userIdClaim != null && applicationUserIdClaim != null)
                {
                    var user = await _userManager.FindByIdAsync(applicationUserIdClaim.Value);
                    if (user != null)
                    {
                        return Ok(new TokenValidationResponse
                        {
                            IsValidToken = true,
                            UserId = userIdClaim.Value,
                            ApplicationUserId = applicationUserIdClaim.Value
                        });
                    }
                }

                return Ok(new TokenValidationResponse
                {
                    IsValidToken = false,
                    Error = "Invalid token - user not found"
                });
            }
            catch (Microsoft.IdentityModel.Tokens.SecurityTokenExpiredException)
            {
                return Ok(new TokenValidationResponse
                {
                    IsValidToken = false,
                    Error = "Token is expired"
                });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Token validation failed");
                return Ok(new TokenValidationResponse
                {
                    IsValidToken = false,
                    Error = $"Invalid token: {ex.Message}"
                });
            }
        }
        [HttpGet("signin_google")]
        [AllowAnonymous]
        [ProducesResponseType(typeof(LoginResponse), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(AuthResponse), StatusCodes.Status400BadRequest)]
        [ProducesResponseType(typeof(AuthResponse), StatusCodes.Status401Unauthorized)]
        public async Task<IActionResult> SignInGoogle(
            [FromQuery] string? code,
            [FromQuery] string? state,
            CancellationToken cancellationToken = default)
        {
            _logger.LogInformation("Google OAuth callback received. Code present: {HasCode}", !string.IsNullOrWhiteSpace(code));

            if (string.IsNullOrWhiteSpace(code))
            {
                _logger.LogWarning("Google callback missing authorization code");
                return BadRequest(new AuthResponse
                {
                    IsSuccess = false,
                    Message = "Invalid Google authentication code",
                    ErrorMessage = "Authorization code is required"
                });
            }

            try
            {
                var googleCommand = HttpContext.RequestServices.GetRequiredService<GoogleLoginCallbackCommand>();
                var context = new GoogleLoginAuthContext
                {
                    Code = code,
                    State = state,
                    HttpContext = HttpContext
                };
                var result = await googleCommand.ExecuteAuthBusinessLogicAsync(context, cancellationToken);

                return result.Match<IActionResult>(
                    Right: _ =>
                    {
                        _logger.LogInformation("Google login saga completed successfully");
                        var roles = context.CurrentIdentityUser != null
                            ? (_userManager.GetRolesAsync(context.CurrentIdentityUser).Result).FirstOrDefault() ?? string.Empty
                            : string.Empty;
                        var response = new LoginResponse
                        {
                            ApplicationUser = context.CurrentApplicationUser,
                            TenantId = context.TenantId,
                            IsFullyRegistered = context.IsFullyRegistered,
                            Token = context.GeneratedToken ?? string.Empty,
                            RefreshToken = context.RefreshToken ?? string.Empty,
                            IsValidToken = !string.IsNullOrEmpty(context.GeneratedToken),
                            PCIShieldAppPowerUserId = context.PCIShieldAppPowerUserId ?? string.Empty,
                            Role = roles,
                            UserType = "Merchant"
                        };

                        _logger.LogInformation("Returning Google login response with token length: {TokenLength}",
                            response.Token?.Length ?? 0);

                        return Ok(response);
                    },
                    Left: ex =>
                    {
                        _logger.LogError(ex, "Google login saga failed");
                        return Unauthorized(new AuthResponse
                        {
                            IsSuccess = false,
                            Message = "Google authentication failed",
                            ErrorMessage = ex.Message
                        });
                    });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Unexpected error in Google signin callback");
                return StatusCode(500, new AuthResponse
                {
                    IsSuccess = false,
                    Message = "Internal server error during Google authentication",
                    ErrorMessage = ex.Message
                });
            }
        }
        [HttpPost("login_google")]
        public async Task<IActionResult> LoginGoogle([FromBody] GoogleLoginRequest model, CancellationToken cancellationToken = default)
        {
            if (string.IsNullOrWhiteSpace(model.TokenId))
            {
                return BadRequest(new AuthResponse
                {
                    IsSuccess = false,
                    Message = "Google token is required"
                });
            }

            var googleCommand = HttpContext.RequestServices.GetRequiredService<ValidateGoogleTokenCommand>();
            var context = new GoogleTokenAuthContext
            {
                TokenId = model.TokenId,
                HttpContext = HttpContext
            };

            var result = await googleCommand.ExecuteAuthBusinessLogicAsync(context, cancellationToken);

            return result.Match<IActionResult>(
                Right: _ =>
                {
                    _logger.LogInformation("Google token login successful");
                    
                    var response = new LoginResponse
                    {
                        ApplicationUser = context.CurrentApplicationUser,
                        Token = context.GeneratedToken ?? string.Empty,
                        RefreshToken = context.RefreshToken ?? string.Empty,
                        IsValidToken = !string.IsNullOrEmpty(context.GeneratedToken),
                        Role = context.CurrentIdentityUser != null 
                            ? (_userManager.GetRolesAsync(context.CurrentIdentityUser).Result).FirstOrDefault() ?? string.Empty
                            : string.Empty
                    };

                    return Ok(response);
                },
                Left: ex =>
                {
                    _logger.LogError(ex, "Google token validation failed");
                    return Unauthorized(new AuthResponse
                    {
                        IsSuccess = false,
                        Message = "Invalid Google authentication",
                        ErrorMessage = ex.Message
                    });
                });
        }
        private async Task<string> ExchangeCodeForToken(string code)
        {
            var clientId = _configuration["Authentication:Google:ClientId"];
            var clientSecret = _configuration["Authentication:Google:ClientSecret"];
            var tokenUri = "https://oauth2.googleapis.com/token";
            var redirectUri = $"{Request.Scheme}://{Request.Host}/api/auth/v2/signin_google";

            var requestBody = new FormUrlEncodedContent(new[]
            {
                new KeyValuePair<string, string>("code", code),
                new KeyValuePair<string, string>("client_id", clientId),
                new KeyValuePair<string, string>("client_secret", clientSecret),
                new KeyValuePair<string, string>("redirect_uri", redirectUri),
                new KeyValuePair<string, string>("grant_type", "authorization_code")
            });

            using var httpClient = new HttpClient();
            var response = await httpClient.PostAsync(tokenUri, requestBody);
            var responseString = await response.Content.ReadAsStringAsync();
            var json = Newtonsoft.Json.Linq.JObject.Parse(responseString);
            return json["access_token"]?.ToString() ?? string.Empty;
        }
        [HttpPost("signin_apple")]
        public async Task<IActionResult> SignInApple([FromForm] string code, [FromForm] string? state, [FromForm] string? user, CancellationToken cancellationToken = default)
        {
            if (string.IsNullOrWhiteSpace(code))
            {
                return BadRequest(new AuthResponse
                {
                    IsSuccess = false,
                    Message = "Invalid Apple authentication code"
                });
            }

            var appleCommand = HttpContext.RequestServices.GetRequiredService<AppleLoginCallbackCommand>();
            var context = new AppleLoginAuthContext
            {
                Code = code,
                State = state,
                User = user,
                HttpContext = HttpContext
            };

            var result = await appleCommand.ExecuteAuthBusinessLogicAsync(context, cancellationToken);

            return result.Match<IActionResult>(
                Right: _ =>
                {
                    _logger.LogInformation("Apple login successful");
                    
                    var response = new LoginResponse
                    {
                        ApplicationUser = context.CurrentApplicationUser,
                        TenantId = context.TenantId,
                        IsFullyRegistered = context.IsFullyRegistered,
                        Token = context.GeneratedToken ?? string.Empty,
                        RefreshToken = context.RefreshToken ?? string.Empty,
                        IsValidToken = !string.IsNullOrEmpty(context.GeneratedToken),
                        PCIShieldAppPowerUserId = context.PCIShieldAppPowerUserId ?? string.Empty,
                        Role = context.CurrentIdentityUser != null 
                            ? (_userManager.GetRolesAsync(context.CurrentIdentityUser).Result).FirstOrDefault() ?? string.Empty
                            : string.Empty
                    };

                    return Ok(response);
                },
                Left: ex =>
                {
                    _logger.LogError(ex, "Apple login failed");
                    return Unauthorized(new AuthResponse
                    {
                        IsSuccess = false,
                        Message = "Apple authentication failed",
                        ErrorMessage = ex.Message
                    });
                });
        }
        [HttpPost("login_apple")]
        public async Task<IActionResult> LoginApple([FromBody] AppleLoginRequest model, CancellationToken cancellationToken = default)
        {
            if (string.IsNullOrWhiteSpace(model.IdentityToken))
            {
                return BadRequest(new AuthResponse
                {
                    IsSuccess = false,
                    Message = "Apple identity token is required"
                });
            }

            var appleCommand = HttpContext.RequestServices.GetRequiredService<ValidateAppleTokenCommand>();
            var context = new AppleTokenAuthContext
            {
                IdentityToken = model.IdentityToken,
                AuthorizationCode = model.AuthorizationCode,
                User = model.User,
                HttpContext = HttpContext
            };

            var result = await appleCommand.ExecuteAuthBusinessLogicAsync(context, cancellationToken);

            return result.Match<IActionResult>(
                Right: _ =>
                {
                    _logger.LogInformation("Apple token login successful");
                    
                    var response = new LoginResponse
                    {
                        ApplicationUser = context.CurrentApplicationUser,
                        Token = context.GeneratedToken ?? string.Empty,
                        RefreshToken = context.RefreshToken ?? string.Empty,
                        IsValidToken = !string.IsNullOrEmpty(context.GeneratedToken),
                        Role = context.CurrentIdentityUser != null 
                            ? (_userManager.GetRolesAsync(context.CurrentIdentityUser).Result).FirstOrDefault() ?? string.Empty
                            : string.Empty
                    };

                    return Ok(response);
                },
                Left: ex =>
                {
                    _logger.LogError(ex, "Apple token validation failed");
                    return Unauthorized(new AuthResponse
                    {
                        IsSuccess = false,
                        Message = "Invalid Apple authentication",
                        ErrorMessage = ex.Message
                    });
                });
        }
        [HttpPost("forgot-password")]
        [AllowAnonymous]
        public async Task<IActionResult> ForgotPassword(
    [FromBody] ForgotPasswordRequest model,
    CancellationToken cancellationToken = default)
        {
            if (!ModelState.IsValid)
            {
                _logger.LogWarning("Invalid forgot password request");
                return BadRequest(new AuthResponse
                {
                    IsSuccess = false,
                    Message = "Invalid request",
                    ErrorMessage = string.Join(", ", ModelState.Values
                        .SelectMany(v => v.Errors)
                        .Select(e => e.ErrorMessage))
                });
            }
            var resetPasswordUrl = $"{Request.Scheme}://{Request.Host}/reset-password";

            var context = new ForgotPasswordAuthContext
            {
                Email = model.Email,
                ResetPasswordUrl = resetPasswordUrl
            };

            var result = await _authSagaHandler.ExecuteForgotPasswordAsync(context, cancellationToken);
            return result.Match<IActionResult>(
                Right: _ =>
                {
                    _logger.LogInformation("Forgot password request processed for {Email}", model.Email);
                    return Ok(new AuthResponse
                    {
                        IsSuccess = true,
                        Message = "If an account with this email exists, a password reset link has been sent."
                    });
                },
                Left: ex =>
                {
                    _logger.LogError(ex, "Forgot password request encountered an issue for {Email}", model.Email);
                    return Ok(new AuthResponse
                    {
                        IsSuccess = true,
                        Message = "If an account with this email exists, a password reset link has been sent."
                    });
                }
            );
        }

        [HttpPost("reset-password")]
        [AllowAnonymous]
        public async Task<IActionResult> ResetPassword(
            [FromBody] ResetPasswordRequest model,
            CancellationToken cancellationToken = default)
        {
            if (!ModelState.IsValid)
            {
                _logger.LogWarning("Invalid reset password request");
                return BadRequest(new AuthResponse
                {
                    IsSuccess = false,
                    Message = "Invalid request",
                    ErrorMessage = string.Join(", ", ModelState.Values
                        .SelectMany(v => v.Errors)
                        .Select(e => e.ErrorMessage))
                });
            }

            var context = new ResetPasswordAuthContext
            {
                Email = model.Email,
                Token = model.Token,
                NewPassword = model.Password
            };

            var result = await _authSagaHandler.ExecuteResetPasswordAsync(context, cancellationToken);

            return result.Match<IActionResult>(
                Right: _ =>
                {
                    _logger.LogInformation("Password successfully reset for {Email}", model.Email);
                    return Ok(new AuthResponse
                    {
                        IsSuccess = true,
                        Message = "Your password has been reset successfully. You can now log in with your new password."
                    });
                },
                Left: ex =>
                {
                    _logger.LogError(ex, "Password reset failed for {Email}", model.Email);
                    return BadRequest(new AuthResponse
                    {
                        IsSuccess = false,
                        Message = "Password reset failed",
                        ErrorMessage = ex.Message
                    });
                }
            );
        }
    }
}
