using System.Globalization;
using System.Security.Claims;
using System.Text.Json;
using System.Text.Json.Serialization;

using ApexCharts;

using FluentValidation;

using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Components.Authorization;
using Microsoft.AspNetCore.Components.Server;
using Microsoft.AspNetCore.Localization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.ResponseCompression;
using Microsoft.JSInterop;

using MudBlazor;
using MudBlazor.Services;

using PCIShield.BlazorAdmin.Client.Auth;
using PCIShield.BlazorAdmin.Client.Services;
using PCIShield.BlazorAdmin.Client.SignalR;
using PCIShield.BlazorAdmin.Components;
using PCIShield.BlazorAdmin.Endpoints;
using PCIShield.BlazorAdmin.Hubs;
using PCIShield.BlazorAdmin.Services;
using PCIShield.BlazorMauiShared.Models.Merchant;
using PCIShield.Client.Services.Assessment;
using PCIShield.Client.Services.Auth;
using PCIShield.Client.Services.Common;
using PCIShield.Client.Services.DashBoard;
using PCIShield.Client.Services.InvoiceSession;
using PCIShield.Client.Services.Merchant;
using PCIShield.Client.Services.Agui;
using PCIShield.Client.Services.PaymentChannel;

using Serilog;

using static PCIShield.BlazorAdmin.Auth.Login;

using ILogger = Microsoft.Extensions.Logging.ILogger;

namespace PCIShield.BlazorAdmin
{
    public class Program
    {
        public static void Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);
            builder.Services.AddApexCharts(e =>
            {
                e.GlobalOptions = new ApexChartBaseOptions
                {
                    Debug = true,
                    Theme = new Theme { Palette = PaletteType.Palette6 }
                };
            });
            builder.Services.AddDistributedMemoryCache();
            builder.Services.AddSession(options =>
            {
                options.IdleTimeout = TimeSpan.FromMinutes(10);
                options.Cookie.HttpOnly = true;
                options.Cookie.IsEssential = true;
                options.Cookie.SecurePolicy = CookieSecurePolicy.Always;
            });

            builder.Services.Configure<CircuitOptions>(options =>
            {
                options.DetailedErrors = true;
            });
            builder.Services.AddHttpContextAccessor();
            builder.Services.AddSmartSignalR(options =>
            {
                options.HubUrl = "/genericEventHub";
                options.MaxRetryAttempts = 3;
            });
            builder.Services.AddScoped<IDirtyStateService, ResilientDirtyStateService>();
            builder.Services.AddOutputCache(options =>
            {
                options.AddBasePolicy(builder => builder.Expire(TimeSpan.FromSeconds(10)));
                options.AddBasePolicy(builder => builder.With(c => c.HttpContext.Request.Path.StartsWithSegments("/api")));
            });
            builder.Services.AddRazorComponents().AddInteractiveServerComponents().AddInteractiveWebAssemblyComponents();
            builder.Services.Configure<JsonSerializerOptions>(options =>
            {
                options.PropertyNamingPolicy = JsonNamingPolicy.CamelCase;
                options.WriteIndented = true;
                options.ReferenceHandler = ReferenceHandler.IgnoreCycles;
                options.DefaultIgnoreCondition = JsonIgnoreCondition.WhenWritingNull;
                options.Converters.Add(new JsonStringEnumConverter());
            });
            builder.Services.AddLocalization(options => options.ResourcesPath = "Resources");
            builder.Services.AddResponseCompression(opts =>
            {
                opts.MimeTypes = ResponseCompressionDefaults.MimeTypes.Concat(new[] { "application/octet-stream" });
            });
            builder
                .Services.AddAuthentication(options =>
                {
                    options.DefaultScheme = "Cookies";
                    options.DefaultChallengeScheme = "Cookies";
                })
                .AddCookie(
                    "Cookies",
                    options =>
                    {
                        options.Cookie.Name = ".PCIShieldERP.BlazorAdmin.Auth";
                        options.LoginPath = "/Auth/Login";
                        options.LogoutPath = "/Auth/Logout";
                        options.AccessDeniedPath = "/Auth/AccessDenied";
                        options.ExpireTimeSpan = TimeSpan.FromMinutes(60);
                        options.SlidingExpiration = true;
                        options.Cookie.SameSite = SameSiteMode.None;
                        options.Cookie.SecurePolicy = CookieSecurePolicy.Always;
                    }
                );
            builder.Services.AddHttpClient(
                "PCIShieldAPI",
                client =>
                {
                    client.BaseAddress = new Uri("https://localhost:52509/");
                }
            );
            builder.Services.AddCors(options =>
            {
                options.AddPolicy("AllowAll", builder => builder.AllowAnyOrigin().AllowAnyMethod().AllowAnyHeader());
            });

            builder.Services.AddAuthorizationCore();
            builder.Services.AddScoped<IHttpAuthClientService, HostAuthService>();
            builder.Services.AddScoped<AuthenticationStateProvider, CookieAuthenticationStateProvider>();

            builder.Services.AddMemoryCache();

            builder.Services.AddScoped<IClientLocalizationService, ClientLocalizationService>();
            builder.Services.AddScoped<MudLocalizer, MudLocalizerService>();
            builder.Services.AddScoped<MudLocalizer, SpanishMudLocalizer>();
            builder.Services.AddScoped<ITokenService>(serviceProvider =>
            {
                var httpContextAccessor = serviceProvider.GetRequiredService<IHttpContextAccessor>();
                var logger = serviceProvider.GetRequiredService<ILogger<ServerSideTokenService>>();
                return new ServerSideTokenService(httpContextAccessor, logger);
            });
            builder.Services.AddScoped<IDashBoardStateService, DashBoardStateService>();
            builder.Services.AddScoped<IHttpAppLocalizationClientService, HttpAppLocalizationClientService>();

            builder.Services.AddScoped<IHttpAssessmentClientService, HttpAssessmentClientService>();
            builder.Services.AddScoped<IHttpPaymentChannelClientService, HttpPaymentChannelClientService>();

         

            // ... existing usings ...

            builder.Services.AddScoped<IHttpMerchantClientService, HttpMerchantClientService>();
            builder.Services.AddScoped<IHttpAguiClientService, HttpAguiClientService>();

            builder.Services.AddScoped<IHttpDashBoardClientService, HttpDashBoardClientService>();

            builder.Services.AddMudServices(config =>
            {
                config.SnackbarConfiguration.PositionClass = Defaults.Classes.Position.BottomRight;
                config.SnackbarConfiguration.PreventDuplicates = false;
                config.SnackbarConfiguration.NewestOnTop = false;
                config.SnackbarConfiguration.ShowCloseIcon = true;
                config.SnackbarConfiguration.VisibleStateDuration = 3000;
                config.SnackbarConfiguration.HideTransitionDuration = 500;
                config.SnackbarConfiguration.ShowTransitionDuration = 500;
            });
            Log.Logger = new LoggerConfiguration()
                .MinimumLevel.Debug()
                .MinimumLevel.Override("Microsoft", Serilog.Events.LogEventLevel.Information)
                .MinimumLevel.Override("Microsoft.AspNetCore", Serilog.Events.LogEventLevel.Warning)
                .MinimumLevel.Override("System", Serilog.Events.LogEventLevel.Information)
                .Enrich.FromLogContext()
                .WriteTo.Console(
                    outputTemplate: "[{Timestamp:HH:mm:ss} {Level:u3}] {SourceContext}: {Message:lj}{NewLine}{Exception}")
                .CreateLogger();
            builder.Host.UseSerilog();
            if (builder.Environment.IsDevelopment())
            {
                builder.Logging.AddConsole();
                builder.Logging.SetMinimumLevel(LogLevel.Debug);
            }
            builder
                .Services.AddSignalR()
                .AddHubOptions<GenericEventHub>(options =>
                {
                    options.MaximumReceiveMessageSize = 5 * 1024 * 1024;
                    options.EnableDetailedErrors = true;
                    options.StreamBufferCapacity = 20;
                    options.StatefulReconnectBufferSize = 100000;
                    options.EnableDetailedErrors = true;
                    options.ClientTimeoutInterval = TimeSpan.FromSeconds(30);
                });
            builder.Services.AddSingleton<Serilog.ILogger>(Log.Logger);
            builder.Services.AddSingleton(typeof(IAppLoggerService<>), typeof(AppLoggerService<>));
            builder.Services.AddValidatorsFromAssemblyContaining<UpdateMerchantValidator>();
            builder.Services.AddScoped<UpdateMerchantValidator>();
            var app = builder.Build();
            var supportedCultures = new[] { new CultureInfo("en-US"), new CultureInfo("es-ES") };
            app.UseRequestLocalization(
                new RequestLocalizationOptions
                {
                    DefaultRequestCulture = new RequestCulture("en-US"),
                    SupportedCultures = supportedCultures,
                    SupportedUICultures = supportedCultures,
                }
            );
            if (app.Environment.IsDevelopment())
            {
                app.UseWebAssemblyDebugging();
                app.UseDeveloperExceptionPage();
            }
            else
            {
                app.UseExceptionHandler("/Error");
                app.UseHsts();
            }
            app.UseHttpsRedirection();
            app.MapStaticAssets();
            app.UseStaticFiles();
            app.UseCors("AllowAll");
            app.UseResponseCompression();
            app.UseRouting();
            app.UseSession();
            app.UseAuthentication();
            app.UseAuthorization();

            // Debugging middleware BEFORE anti-forgery to avoid form access issues
            app.Use(async (context, next) =>
            {
                if (context.Request.Path.StartsWithSegments("/auth/get-client-tokens"))
                {
                    var logger = context.RequestServices.GetRequiredService<ILogger<Program>>();
                    logger.LogInformation($"Request to {context.Request.Path}:");
                    logger.LogInformation($"  - User.Identity.IsAuthenticated: {context.User.Identity?.IsAuthenticated}");
                    logger.LogInformation($"  - User.Identity.Name: {context.User.Identity?.Name}");
                    logger.LogInformation($"  - Has Cookie header: {context.Request.Headers.ContainsKey("Cookie")}");
                    if (context.Request.Headers.ContainsKey("Cookie"))
                    {
                        logger.LogInformation($"  - Cookie header value: {context.Request.Headers["Cookie"].ToString().Substring(0, Math.Min(50, context.Request.Headers["Cookie"].ToString().Length))}...");
                    }
                    logger.LogInformation($"  - Claims count: {context.User.Claims.Count()}");
                    foreach (var claim in context.User.Claims.Take(5))
                    {
                        logger.LogInformation($"  - Claim: {claim.Type} = {claim.Value.Substring(0, Math.Min(20, claim.Value.Length))}...");
                    }
                }
                await next();
            });

            app.UseAntiforgery();

            app.MapPost(
                    "/auth/perform-host-login",
                    async ([FromForm] LoginInputModel loginForm, HttpContext httpContext, [FromServices] IHttpAuthClientService backendApiAuthService, [FromServices] LinkGenerator linkGenerator, [FromServices] ILogger<Program> logger) =>
                    {
                        logger.LogInformation("Host login endpoint hit for user: {Username}", loginForm.Username);
                        var apiLoginRequest = new PCIShield.BlazorMauiShared.Models.Auth.LoginRequest { Username = loginForm.Username, Password = loginForm.Password };

                        var apiLoginResult = await backendApiAuthService.LoginMerchantAsync(apiLoginRequest);

                        return await apiLoginResult.MatchAsync<IResult>(
                            RightAsync: async apiResponse =>
                            {
                                if (apiResponse.IsValidToken && !string.IsNullOrEmpty(apiResponse.Token))
                                {
                                    logger.LogInformation("API login successful for {Username}. Setting host cookie.", loginForm.Username);
                                    var claims = new List<Claim> { new Claim(ClaimTypes.NameIdentifier, apiResponse.ApplicationUser?.ApplicationUserId.ToString() ?? apiLoginRequest.Username), new Claim(ClaimTypes.Name, apiLoginRequest.Username) };
                                    if (!string.IsNullOrEmpty(apiResponse.Role))
                                    {
                                        claims.Add(new Claim(ClaimTypes.Role, apiResponse.Role));
                                    }
                                    claims.Add(new Claim("api_jwt_token", apiResponse.Token));
                                    if (!string.IsNullOrEmpty(apiResponse.RefreshToken))
                                    {
                                        claims.Add(new Claim("api_refresh_token", apiResponse.RefreshToken));
                                    }

                                    var claimsIdentity = new ClaimsIdentity(claims, CookieAuthenticationDefaults.AuthenticationScheme);
                                    var authProperties = new AuthenticationProperties { IsPersistent = loginForm.RememberMe };

                                    await httpContext.SignInAsync(CookieAuthenticationDefaults.AuthenticationScheme, new ClaimsPrincipal(claimsIdentity), authProperties);
                                    logger.LogInformation("Host cookie set successfully.");

                                    var returnUrl = loginForm.ReturnUrl ?? "/";
                                    if (!string.IsNullOrEmpty(returnUrl) && returnUrl.StartsWith("/") && !returnUrl.StartsWith("//") && !returnUrl.Contains(":/"))
                                    {
                                        return Results.Redirect(returnUrl);
                                    }
                                    return Results.Redirect("/");
                                }
                                else
                                {
                                    logger.LogWarning("API login failed for {Username}: {ErrorMessage}", loginForm.Username, apiResponse.ErrorMessage ?? "Invalid credentials");
                                    var loginPageUrl = linkGenerator.GetPathByPage(httpContext, "/Auth/Login", values: new { error = apiResponse.ErrorMessage ?? "Invalid credentials" });
                                    return Results.Redirect(loginPageUrl ?? "/Auth/Login?error=LoginFailed");
                                }
                            },
                            LeftAsync: async error =>
                            {
                                Console.Error.WriteLine($"Backend API service returned error for login by {loginForm.Username}: {error}");
                                var loginPageUrl = linkGenerator.GetPathByPage(httpContext, "/Auth/Login", values: new { error = error });
                                return Results.Redirect(loginPageUrl ?? "/Auth/Login?error=ApiError");
                            }
                        );
                    }
                )
                .WithName("PerformHostLogin");
            app.MapGet(
                    "/auth/google-challenge",
                    (HttpContext httpContext, IConfiguration configuration, ILogger<Program> logger, [FromQuery] string? returnUrl) =>
                    {
                        logger.LogInformation("Google OAuth challenge initiated");
                        var clientId = configuration["Authentication:Google:ClientId"];
                        var redirectUri = $"{httpContext.Request.Scheme}://{httpContext.Request.Host}/auth/google-callback";

                        if (string.IsNullOrEmpty(clientId))
                        {
                            logger.LogError("Google ClientId not configured");
                            return Results.Redirect("/Auth/Login?error=GoogleNotConfigured");
                        }
                        var state = Convert.ToBase64String(Guid.NewGuid().ToByteArray());
                        var scope = Uri.EscapeDataString("openid profile email");

                        var googleAuthUrl = "https://accounts.google.com/o/oauth2/v2/auth?" +
                            $"client_id={Uri.EscapeDataString(clientId)}&" +
                            $"redirect_uri={Uri.EscapeDataString(redirectUri)}&" +
                            $"response_type=code&" +
                            $"scope={scope}&" +
                            $"state={state}&" +
                            $"access_type=offline&" +
                            $"prompt=consent";
                        httpContext.Session.SetString("GoogleOAuthState", state);
                        httpContext.Session.SetString("GoogleOAuthReturnUrl", returnUrl ?? "/");

                        logger.LogInformation("Redirecting to Google OAuth: {GoogleAuthUrl}", googleAuthUrl.Substring(0, Math.Min(100, googleAuthUrl.Length)));
                        return Results.Redirect(googleAuthUrl);
                    }
                )
                .WithName("GoogleChallenge")
                .AllowAnonymous();
            app.MapGet(
                    "/auth/google-callback",
                    async (
                        HttpContext httpContext,
                        [FromServices] IHttpAuthClientService backendApiAuthService,
                        [FromServices] IConfiguration configuration,
                        [FromServices] ILogger<Program> logger,
                        [FromQuery] string? code,
                        [FromQuery] string? state,
                        [FromQuery] string? error) =>
                    {
                        logger.LogInformation("Google OAuth callback received. Code present: {HasCode}, State present: {HasState}, Error: {Error}",
                            !string.IsNullOrEmpty(code),
                            !string.IsNullOrEmpty(state),
                            error);
                        if (!string.IsNullOrEmpty(error))
                        {
                            logger.LogWarning("Google OAuth returned error: {Error}", error);
                            return Results.Redirect($"/Auth/Login?error=GoogleAuth_{error}");
                        }
                        if (string.IsNullOrEmpty(code) || string.IsNullOrEmpty(state))
                        {
                            logger.LogError("Google callback missing required parameters");
                            return Results.Redirect("/Auth/Login?error=InvalidGoogleCallback");
                        }
                        var storedState = httpContext.Session.GetString("GoogleOAuthState");
                        if (string.IsNullOrEmpty(storedState) || storedState != state)
                        {
                            logger.LogError("Google OAuth state validation failed. Expected: {Expected}, Got: {Got}",
                                storedState, state);
                            return Results.Redirect("/Auth/Login?error=InvalidState");
                        }
                        var returnUrl = httpContext.Session.GetString("GoogleOAuthReturnUrl") ?? "/";
                        httpContext.Session.Remove("GoogleOAuthState");
                        httpContext.Session.Remove("GoogleOAuthReturnUrl");

                        try
                        {
                            logger.LogInformation("Calling backend API to process Google authorization code");
                            var backendResult = await backendApiAuthService.ProcessGoogleCallbackAsync(code, state);

                            return await backendResult.MatchAsync<IResult>(
                                RightAsync: async apiResponse =>
                                {
                                    if (apiResponse.IsValidToken && !string.IsNullOrEmpty(apiResponse.Token))
                                    {
                                        logger.LogInformation("Google login successful via backend API. Setting host cookie.");
                                        var claims = new List<Claim>
                                        {
                                new Claim(ClaimTypes.NameIdentifier, apiResponse.ApplicationUser?.ApplicationUserId.ToString() ?? Guid.NewGuid().ToString()),
                                new Claim(ClaimTypes.Name, apiResponse.ApplicationUser?.UserName ?? "GoogleUser"),
                                new Claim(ClaimTypes.Email, apiResponse.ApplicationUser?.Email ?? "")
                                        };

                                        if (!string.IsNullOrEmpty(apiResponse.Role))
                                        {
                                            claims.Add(new Claim(ClaimTypes.Role, apiResponse.Role));
                                        }
                                        claims.Add(new Claim("api_jwt_token", apiResponse.Token));
                                        if (!string.IsNullOrEmpty(apiResponse.RefreshToken))
                                        {
                                            claims.Add(new Claim("api_refresh_token", apiResponse.RefreshToken));
                                        }
                                        claims.Add(new Claim("AuthenticationMethod", "Google"));

                                        var claimsIdentity = new ClaimsIdentity(claims, CookieAuthenticationDefaults.AuthenticationScheme);
                                        var authProperties = new AuthenticationProperties
                                        {
                                            IsPersistent = true,
                                            ExpiresUtc = DateTimeOffset.UtcNow.AddHours(24)
                                        };

                                        await httpContext.SignInAsync(
                                            CookieAuthenticationDefaults.AuthenticationScheme,
                                            new ClaimsPrincipal(claimsIdentity),
                                            authProperties);

                                        logger.LogInformation("Host cookie set successfully for Google user. Redirecting to: {ReturnUrl}", returnUrl);
                                        if (!string.IsNullOrEmpty(returnUrl) &&
                                            returnUrl.StartsWith("/") &&
                                            !returnUrl.StartsWith("//") &&
                                            !returnUrl.Contains("://"))
                                        {
                                            return Results.Redirect(returnUrl);
                                        }

                                        return Results.Redirect("/");
                                    }
                                    else
                                    {
                                        logger.LogWarning("Google login via backend returned invalid token");
                                        return Results.Redirect("/Auth/Login?error=InvalidGoogleToken");
                                    }
                                },
                                LeftAsync: async error =>
                                {
                                    logger.LogError("Backend API returned error for Google login: {Error}", error);
                                    return Results.Redirect($"/Auth/Login?error={Uri.EscapeDataString(error)}");
                                }
                            );
                        }
                        catch (Exception ex)
                        {
                            logger.LogError(ex, "Unexpected error processing Google callback");
                            return Results.Redirect("/Auth/Login?error=GoogleCallbackError");
                        }
                    }
                )
                .WithName("GoogleCallback")
                .AllowAnonymous();
            app.MapGet(
                    "/auth/get-client-tokens",
                    (HttpContext httpContext, ILogger<Program> logger) =>
                    {
                        logger.LogInformation($"/auth/get-client-tokens: Called. User authenticated: {httpContext.User.Identity?.IsAuthenticated}");

                        if (httpContext.User.Identity?.IsAuthenticated ?? false)
                        {
                            var jwtClaim = httpContext.User.FindFirst("api_jwt_token");
                            var refreshTokenClaim = httpContext.User.FindFirst("api_refresh_token");

                            logger.LogInformation($"/auth/get-client-tokens: JWT claim found: {jwtClaim != null}, Refresh token claim found: {refreshTokenClaim != null}");

                            if (jwtClaim != null)
                            {
                                logger.LogInformation($"/auth/get-client-tokens: Returning tokens. JWT length: {jwtClaim.Value.Length}");
                                return Results.Ok(new { accessToken = jwtClaim.Value, refreshToken = refreshTokenClaim?.Value });
                            }
                            logger.LogWarning("/auth/get-client-tokens: User authenticated but no API token found in claims");
                            return Results.Problem("API token not found in user session.", statusCode: 404);
                        }
                        logger.LogWarning("/auth/get-client-tokens: User not authenticated, returning 401");
                        return Results.Unauthorized();
                    }
                )
                .RequireAuthorization();
            app.MapGet("/force-logout", async (HttpContext context) =>
            {
                await context.SignOutAsync(CookieAuthenticationDefaults.AuthenticationScheme);
                return Results.Redirect("/Auth/Login?message=Please login again");
            });
            app.MapLogoutEndpoint();

            app.MapHub<GenericEventHub>("/genericEventHub");
            app.MapRazorComponents<App>().AddInteractiveServerRenderMode().AddInteractiveWebAssemblyRenderMode().AddAdditionalAssemblies(typeof(Client._Imports).Assembly);

            try
            {
                Log.Information("Starting Blazor Host application");
                app.Run();
            }
            catch (Exception ex)
            {
                Log.Fatal(ex, "Application terminated unexpectedly");
                throw;
            }
            finally
            {
                Log.CloseAndFlush();
            }
        }
    }

    public class BrowserConsoleLogger : ILogger
    {
        private readonly IJSRuntime _jsRuntime;
        private readonly string _categoryName;

        public BrowserConsoleLogger(IJSRuntime jsRuntime, string categoryName)
        {
            _jsRuntime = jsRuntime;
            _categoryName = categoryName;
        }

        public IDisposable? BeginScope<TState>(TState state)
            where TState : notnull
        {
            return NullScope.Instance;
        }

        public bool IsEnabled(LogLevel logLevel)
        {
            return logLevel != LogLevel.None;
        }

        private class NullScope : IDisposable
        {
            public static readonly NullScope Instance = new NullScope();

            private NullScope() { }

            public void Dispose() { }
        }

        public void Log<TState>(LogLevel logLevel, EventId eventId, TState state, Exception exception, Func<TState, Exception, string> formatter)
        {
            string message = formatter(state, exception);
            _jsRuntime.InvokeVoidAsync("console.log", $"{_categoryName}: {message}");
            if (exception != null)
            {
                _jsRuntime.InvokeVoidAsync("console.error", exception.ToString());
            }
        }
    }
}
