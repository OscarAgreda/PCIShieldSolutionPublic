using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Mvc;

namespace PCIShield.BlazorAdmin.Endpoints
{
    public static class LogoutEndpoint
    {
        public static void MapLogoutEndpoint(this WebApplication app)
        {
            app.MapPost("/api/auth/logout", async (HttpContext httpContext, ILogger<Program> logger) =>
            {
                logger.LogInformation($"/api/auth/logout: User {httpContext.User.Identity?.Name} logging out");
                await httpContext.SignOutAsync(CookieAuthenticationDefaults.AuthenticationScheme);
                httpContext.Session?.Clear();
                httpContext.Response.Cookies.Delete(".PCIShieldERP.BlazorAdmin.Auth", new CookieOptions
                {
                    Path = "/",
                    SameSite = SameSiteMode.None,
                    Secure = true
                });
                
                logger.LogInformation("/api/auth/logout: Cookie and session cleared");
                return Results.Ok(new { message = "Logged out successfully" });
            })
            .AllowAnonymous();
        }
    }
}