// PCIShield.BlazorMauiShared/Models/Auth/LoginRequest.cs

using PCIShield.BlazorMauiShared.Auth;
using PCIShield.Domain.ModelsDto;

namespace PCIShield.BlazorMauiShared.Models.Auth
{
    public class LoginRequest
    {
        public string Username { get; set; }
        public string Password { get; set; }
        public string? TwoFactorCode { get; set; }
        public string? TwoFactorRecoveryCode { get; set; }
        public bool RememberMe { get; set; }
        public string ReturnUrl { get; set; }
    }
}

namespace PCIShield.BlazorMauiShared.Models.Auth
{
    public class LoginResponse
    {
        public ApplicationUserDto ApplicationUser { get; set; }
        public LoginUserDto User { get; set; }
        public string MerchantId { get; set; }
        public string ComplianceOfficerId { get; set; }
        public string TenantId { get; set; }
        public bool? IsFullyRegistered { get; set; }
        public string Token { get; set; }
        public string RefreshToken { get; set; }
        public bool IsValidToken { get; set; }
        public string PCIShieldAppPowerUserId { get; set; }
        public string ErrorMessage { get; set; }
        public string? UserType { get; set; }
        public string? Role { get; set; }
       
    }
}

namespace PCIShield.BlazorMauiShared.Models.Auth
{
    public class RegisterRequest
    {
        public string Username { get; set; }
        public string Email { get; set; }
        public string Password { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string Phone { get; set; }
    }
}

namespace PCIShield.BlazorMauiShared.Models.Auth
{
    public class RegisterResponse
    {
        public string Message { get; set; }
        public string ErrorMessage { get; set; }
    }
}

namespace PCIShield.BlazorMauiShared.Models.Auth
{
    public class LogoutRequest
    {
        public string UserId { get; set; }
    }
}

namespace PCIShield.BlazorMauiShared.Models.Auth
{
    public class LogoutResponse
    {
        public string Message { get; set; }
        public bool IsSuccess { get; set; }
    }
}

namespace PCIShield.BlazorMauiShared.Models.Auth
{
    public class ValidateTokenRequest
    {
        public string Token { get; set; }
    }
}

namespace PCIShield.BlazorMauiShared.Models.Auth
{
    public class ValidateTokenResponse
    {
        public bool IsValidToken { get; set; }
        public string Error { get; set; }
    }
}

namespace PCIShield.BlazorMauiShared.Models.Auth
{
    public class RefreshTokenRequest
    {
        public string RefreshToken { get; set; }
    }
}

namespace PCIShield.BlazorMauiShared.Models.Auth
{
    public class RefreshTokenResponse
    {
        public string AccessToken { get; set; }
        public string RefreshToken { get; set; }
        public string TokenType { get; set; }
        public int ExpiresIn { get; set; }
        public string Error { get; set; }
    }
}