using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Reactive.Linq;
using System.Reflection;
using System.Security.Claims;
using System.Text;
using System.Text.Json;
using System.Text.Json.Serialization;
using System.Threading;
using System.Threading;
using System.Threading.Tasks;
using System.Threading.Tasks;

using Autofac;

using Elastic.Clients.Elasticsearch.MachineLearning;
using Elastic.Clients.Elasticsearch.Xpack;

using FastEndpoints;
using FastEndpoints.Swagger;

using FluentValidation;

using Hangfire;
using Hangfire.SqlServer;

using LanguageExt.ClassInstances.Const;

using MediatR;
using MediatR.NotificationPublishers;
using MediatR.Pipeline;

using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Authorization;

using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Diagnostics;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Http.Connections;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Localization;
using Microsoft.AspNetCore.Mvc.Formatters;
using Microsoft.AspNetCore.Mvc.ModelBinding;
using Microsoft.AspNetCore.ResponseCompression;
using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Caching.Hybrid;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Diagnostics.HealthChecks;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Localization;
using Microsoft.Extensions.Logging;
using Microsoft.IdentityModel.Tokens;

using NetTopologySuite.Geometries;

using Newtonsoft.Json;

using PCIShield.Api.Auth.ApplicationUserOnly;
using PCIShield.Api.Auth.AuthServices;
using PCIShield.Api.Common;

using PCIShield.Api.Saga.SignalR;
using PCIShield.BlazorMauiShared;
using PCIShield.BlazorMauiShared.Models;
using PCIShield.BlazorMauiShared.Models.Merchant;
using PCIShield.Domain.Entities;
using PCIShield.Domain.Interfaces;
using PCIShield.Domain.ModelsDto;
using PCIShield.Infrastructure;
using PCIShield.Infrastructure.Data;
using PCIShield.Infrastructure.Helpers;
using PCIShield.Infrastructure.SeedData;
using PCIShield.Infrastructure.Services;
using PCIShield.Infrastructure.Services.OracleAi;

using PCIShieldLib.SharedKernel.Interfaces;

using Quartz;

using Serilog;
using Serilog.Core;
using Serilog.Events;
using Serilog.Sinks.MSSqlServer;

using Utf8Json;
using Utf8Json.AspNetCoreMvcFormatter;
using Utf8Json.Resolvers;

using static PCIShield.Api.Auth.ApplicationUserOnly.AuthenticationStrategyServiceCollectionExtensions;

// A2UI: OpenAI IChatClient and AG-UI Agents
using Microsoft.Extensions.AI;
using OpenAI;
using PCIShield.Api.Agents;

using ILogger = Serilog.ILogger;
using JsonSerializer = Utf8Json.JsonSerializer;
using JsonSerializerOptions = System.Text.Json.JsonSerializerOptions;

namespace PCIShield.Api
{

    public class Startup
    {
        public const string CORS_POLICY = "CorsPolicy";
        private readonly IWebHostEnvironment _env;
        public Startup(IConfiguration configuration, IWebHostEnvironment env)
        {
            Configuration = configuration;
            _env = env;
            System.Diagnostics.Debug.WriteLine($"Environment: {_env.EnvironmentName}");
        }
        public IConfiguration Configuration { get; }
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env, IHeartbeatService heartbeatService,
            IHostApplicationLifetime appLifetime, CleanupService cleanupService)
        {

            if (env.IsDevelopment())
            {
                app.Use(async (context, next) =>
                {
                    try
                    {
                        await next();
                    }
                    catch (Exception ex)
                    {
                        System.Diagnostics.Debugger.Break();
                        throw;
                    }
                });

                app.UseDeveloperExceptionPage();
            }

            if (!env.IsDevelopment())
            {
                app.UseMiddleware<ExceptionMiddleware>();
                app.UseHsts();
            }
            else
            {
                app.UseHttpLogging();
                app.UseDeveloperExceptionPage();
            }
            appLifetime.ApplicationStarted.Register(() => cleanupService.Start());
            appLifetime.ApplicationStopping.Register(() => cleanupService.Stop());
            heartbeatService.Start();
            appLifetime.ApplicationStopping.Register(() => heartbeatService.Stop());

            app.UseHttpsRedirection();

            app.UseHttpsRedirection();

            app.UseWhen(context => !context.Request.Path.StartsWithSegments("/agui"), appBuilder =>
            {
                appBuilder.UseResponseCompression();
            });
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }
            else
            {
            }

            app.UseRequestLocalization();
            app.UseHangfireDashboard();
            if (env.IsDevelopment())
            {
                app.UseCors("AllowBlazorAdminLocalhost");
            }
            else
            {
                app.UseCors("AllowBlazorAdminRemote");
            }
            app.UseResponseCompression();
            app.UseRouting();
            if (env.IsDevelopment())
            {
            }
            app.UseAuthentication();
            app.UseAuthorization();
            var routes = new Dictionary<string, string>();
            var routesMetadata = new Dictionary<string, RouteMetadata>();
            try
            {
                app.UseResponseCaching()
                .UseEndpoints(endpoints =>
                {
                    endpoints.MapControllers();
                    endpoints.MapHub<PCIShieldAppApiSignalrHub>("/pciShieldappApiSignalrHub", options =>
                    {
                        options.Transports =
                            HttpTransportType.WebSockets |
                            HttpTransportType.LongPolling;
                        options.LongPolling.PollTimeout = TimeSpan.FromSeconds(30);
                        options.WebSockets.CloseTimeout = TimeSpan.FromSeconds(30);
                        options.AllowStatefulReconnects = true;
                    });
                    endpoints.MapFastEndpoints(config =>
                    {

                        config.Endpoints.RoutePrefix = "api";
                        config.Endpoints.Configurator = ep =>
                        {
                            foreach (var route in ep.Routes)
                            {
                                var key = $"{string.Join(",", ep.Verbs)}:{route}";
                                if (routes.ContainsKey(key))
                                {
                                    var error = $"Duplicate route found: {key}\nExisting: {routes[key]}\nDuplicate: {ep.EndpointType.FullName}";
                                    System.Diagnostics.Debugger.Break();
                                    throw new InvalidOperationException(error);
                                }
                                routes[key] = ep.EndpointType.FullName;
                            }
                        };
                    });

                    // A2UI: Map AG-UI Merchant Copilot endpoint
                    endpoints.MapPCIShieldAgents();
                });
            }
            catch (Exception e)
            {
                throw;
            }
            if (env.IsDevelopment() && Environment.UserName == "Oscar")
            {
            }

        }
        public void ConfigureContainer(ContainerBuilder builder)
        {
            bool isDevelopment = _env.EnvironmentName == "Development";
            builder.RegisterType<MerchantComplianceOfficerMessagePublisherService>().As<IMerchantComplianceOfficerMessagePublisherService>()
                .InstancePerLifetimeScope();
            string SqlConnectionString = Configuration.GetConnectionString("DefaultConnection");
            string MongConnectionString = Configuration.GetConnectionString("MongoDbConnection");
            string RedisConnectionString = Configuration.GetConnectionString("RedisConnection");
            string currentUserName = Environment.UserName;
            if (currentUserName.Equals("macpciShieldapp", StringComparison.InvariantCultureIgnoreCase))
            {
                SqlConnectionString = Configuration.GetSection("ConnectionStrings:KarlaConnection").Value;
            }
            builder.RegisterModule(new DefaultInfrastructureModule(isDevelopment, SqlConnectionString, MongConnectionString, RedisConnectionString,
                Assembly.GetExecutingAssembly()));
        }
        public void ConfigureDevelopmentServices(IServiceCollection services)
        {
            ConfigureProductionServices(services);
        }
        public void ConfigureDockerServices(IServiceCollection services)
        {
            System.Diagnostics.Debug.WriteLine($"Environment: {_env.EnvironmentName}");
            ConfigureDevelopmentServices(services);
        }
        public void ConfigureProductionServices(IServiceCollection services)
        {

            services.AddAuthenticationStrategy(Configuration);

            services.AddOutputCache(options =>
            {
                options.AddBasePolicy(builder =>
                    builder.Expire(TimeSpan.FromSeconds(10)));
                options.AddBasePolicy(builder =>
                    builder.With(c => c.HttpContext.Request.Path.StartsWithSegments("/api")));
            });
            string? pciShieldappIssuer = Configuration["JwtSettings:PCISHIELDAPP_ISSUER"];
            string? pciShieldappAudience = Configuration["JwtSettings:PCISHIELDAPP_AUDIENCE"];
            string? pciShieldappSecretKey = Configuration["JwtSettings:PCISHIELDAPP_SECRET_KEY"];
            string? redirect_uri = $"https://oauth-redirect.googleusercontent.com/r/pciShieldappnewapp";

            services.AddAuthentication(options =>
            {
                options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
                options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
                options.DefaultScheme = JwtBearerDefaults.AuthenticationScheme;
            })
   .AddGoogle(options =>
   {
       options.ClientId = Configuration["EmailSettings:client_id"];
       options.ClientSecret = Configuration["EmailSettings:client_secret"];
       options.CallbackPath = new PathString("/api/accountv2/signin_google");
   })
   .AddJwtBearer(options =>
   {
       options.TokenValidationParameters = new TokenValidationParameters
       {
           ValidateIssuer = true,
           ValidateAudience = true,
           ValidateLifetime = true,
           ValidateIssuerSigningKey = true,
           ValidIssuer = pciShieldappIssuer,
           ValidAudience = pciShieldappAudience,
           IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(pciShieldappSecretKey)),
           ClockSkew = TimeSpan.FromMinutes(5)
       };

       options.Events = new JwtBearerEvents
       {
           OnTokenValidated = async context =>
           {
               var logger = context.HttpContext.RequestServices
                   .GetRequiredService<ILogger<Program>>();
               logger.LogInformation("=== TOKEN VALIDATION DEBUG ===");
               logger.LogInformation("All claims in token:");
               foreach (var claim in context.Principal.Claims)
               {
                   logger.LogInformation("  Claim: {Type} = {Value}",
                       claim.Type,
                       claim.Value.Length > 50 ? claim.Value.Substring(0, 50) + "..." : claim.Value);
               }
               logger.LogInformation("=== END TOKEN VALIDATION DEBUG ===");

               try
               {
                   var userManager = context.HttpContext.RequestServices
                       .GetRequiredService<UserManager<CustomPCIShieldUser>>();
                   var usernameClaim = context.Principal?.FindFirst(ClaimTypes.NameIdentifier)
                                    ?? context.Principal?.FindFirst(ClaimTypes.Name)
                                    ?? context.Principal?.FindFirst("sub");

                   if (usernameClaim == null)
                   {
                       logger.LogWarning("Token validation: No username claim found in token");
                       context.Fail("Invalid token: missing user identifier");
                       return;
                   }

                   var username = usernameClaim.Value;
                   logger.LogInformation("Token validation: Looking up user by username: {Username}", username);
                   var tokenSecurityStamp = context.Principal?.FindFirst("AspNet.Identity.SecurityStamp")?.Value;

                   if (string.IsNullOrEmpty(tokenSecurityStamp))
                   {
                       logger.LogDebug("Token for user {Username} has no security stamp, skipping validation", username);
                       return;
                   }
                   var user = await userManager.FindByNameAsync(username);

                   if (user == null)
                   {
                       logger.LogWarning("Token validation: User {Username} not found in database", username);
                       context.Fail("User not found");
                       return;
                   }

                   logger.LogInformation("Token validation: Found user {Username} with ID {UserId}", username, user.Id);
                   if (user.SecurityStamp != tokenSecurityStamp)
                   {
                       logger.LogWarning(
                           "Security stamp validation failed for user {Username}. Token stamp: {TokenStamp}, Current stamp: {CurrentStamp}",
                           username,
                           tokenSecurityStamp?.Substring(0, Math.Min(8, tokenSecurityStamp.Length)),
                           user.SecurityStamp?.Substring(0, Math.Min(8, user.SecurityStamp?.Length ?? 0)));

                       context.Fail("Invalid security stamp. Please log in again.");
                       return;
                   }

                   logger.LogInformation("Security stamp validated successfully for user {Username}", username);
               }
               catch (Exception ex)
               {
                   logger.LogError(ex, "Error during security stamp validation");
                   context.Fail("Security validation failed");
               }
           },

           OnChallenge = context =>
           {
               context.HandleResponse();
               context.Response.StatusCode = StatusCodes.Status401Unauthorized;
               context.Response.ContentType = "application/json";

               var result = System.Text.Json.JsonSerializer.Serialize(new
               {
                   error = "Unauthorized",
                   message = "You are not authorized to access this resource. Please log in.",
                   status = 401
               });

               return context.Response.WriteAsync(result);
           },

           OnForbidden = context =>
           {
               context.Response.StatusCode = StatusCodes.Status403Forbidden;
               context.Response.ContentType = "application/json";

               var result = System.Text.Json.JsonSerializer.Serialize(new
               {
                   error = "Forbidden",
                   message = "You do not have permission to access this resource.",
                   status = 403
               });

               return context.Response.WriteAsync(result);
           },

           OnAuthenticationFailed = context =>
           {
               var logger = context.HttpContext.RequestServices
                   .GetRequiredService<ILogger<Program>>();

               logger.LogError("=== JWT AUTHENTICATION FAILED ===");
               logger.LogError("Exception: {Exception}", context.Exception.Message);
               logger.LogError("Path: {Path}", context.Request.Path);

               if (context.Request.Headers.ContainsKey("Authorization"))
               {
                   var authHeader = context.Request.Headers["Authorization"].ToString();
                   logger.LogError("Auth header present: {Header}",
                       authHeader.Length > 50 ? authHeader.Substring(0, 50) + "..." : authHeader);
               }
               else
               {
                   logger.LogError("NO Authorization header present");
               }

               if (context.Exception is SecurityTokenExpiredException)
               {
                   logger.LogInformation("Token expired for request to {Path}", context.Request.Path);
                   context.Response.Headers.Add("Token-Expired", "true");
               }
               else
               {
                   logger.LogWarning(context.Exception, "Authentication failed for request to {Path}", context.Request.Path);
               }

               return Task.CompletedTask;
           }
       };
   });
            services.AddCors(options =>
            {

                options.AddPolicy("AllowAll",
                    builder =>
                    {
                        builder
                            .AllowAnyOrigin()
                            .AllowAnyMethod()
                            .AllowAnyHeader();
                    });
                options.AddPolicy("AllowBlazorAdminLocalhost",
                    builder =>
                    {
                        builder.WithOrigins("https://localhost:7234")
                            .AllowAnyMethod()
                            .AllowAnyHeader()
                            .AllowCredentials();
                    });
                options.AddPolicy("AllowBlazorAdminRemote", builder =>
                {
                    builder
                        .WithOrigins("https://localhost:7234")
                        .AllowAnyMethod()
                        .AllowAnyHeader()
                        .AllowCredentials();
                });
            });
            services.AddAuthorization(options =>
            {
                options.AddPolicy("CustomerOrEmployee", policy =>
                    policy.RequireAssertion(context =>
                        context.User.HasClaim(c =>
                            (c.Type == "SpecialCustomerPermission" ||
                             c.Type == "SpecialEmployeePermission"))));
                options.AddPolicy("PCIShieldAppCustomerPolicyRead", policy =>
                    policy.RequireClaim("SpecialCustomerPermission", "PCIShieldAppCustomerSpecialAccess")
                        .RequireAssertion(context =>
                            context.User.HasClaim(c => c.Type == "scope" && c.Value.Contains("customer.read"))));
                options.AddPolicy("PCIShieldAppCustomerPolicyWrite", policy =>
                    policy.RequireClaim("SpecialCustomerPermission", "PCIShieldAppCustomerSpecialAccess")
                        .RequireAssertion(context =>
                            context.User.HasClaim(c => c.Type == "scope" && c.Value.Contains("customer.write"))));
                options.AddPolicy("PCIShieldAppEmployeePolicyRead", policy =>
                    policy.RequireClaim("SpecialEmployeePermission", "PCIShieldAppEmployeeSpecialAccess")
                        .RequireAssertion(context =>
                            context.User.HasClaim(c => c.Type == "scope" && c.Value.Contains("employee.read"))));
                options.AddPolicy("PCIShieldAppEmployeePolicyWrite", policy =>
                    policy.RequireClaim("SpecialEmployeePermission", "PCIShieldAppEmployeeSpecialAccess")
                        .RequireAssertion(context =>
                            context.User.HasClaim(c => c.Type == "scope" && c.Value.Contains("employee.write"))));
            });
            services.AddIdentity<CustomPCIShieldUser, IdentityRole>(options =>
            {
                options.SignIn.RequireConfirmedAccount = true;
                options.Password.RequiredLength = 8;
                options.Password.RequireDigit = true;
                options.Password.RequireLowercase = true;
                options.Password.RequireUppercase = true;
                options.Password.RequireNonAlphanumeric = true;
                options.Password.RequiredUniqueChars = 4;
                options.Lockout.DefaultLockoutTimeSpan = TimeSpan.FromMinutes(15);
                options.Lockout.MaxFailedAccessAttempts = 5;
                options.Lockout.AllowedForNewUsers = true;
                options.User.RequireUniqueEmail = true;
                options.Tokens.PasswordResetTokenProvider = TokenOptions.DefaultProvider;
                options.Tokens.EmailConfirmationTokenProvider = TokenOptions.DefaultProvider;
            })
          .AddEntityFrameworkStores<AuthorizationDbContext>()
          .AddDefaultTokenProviders();
            services.Configure<SecurityStampValidatorOptions>(options =>
            {
                options.ValidationInterval = TimeSpan.FromMinutes(5);
            });
            services.Configure<DataProtectionTokenProviderOptions>(options =>
            {
                options.TokenLifespan = TimeSpan.FromHours(24);
            });

            services.ConfigureApplicationCookie(options =>
            {
                options.LoginPath = "/Account/Login";
                options.AccessDeniedPath = "/Account/AccessDenied";
                options.ExpireTimeSpan = TimeSpan.FromHours(24);
                options.SlidingExpiration = true;
                options.Events.OnRedirectToLogin = context =>
                {
                    if (context.Request.Path.StartsWithSegments("/api"))
                    {
                        context.Response.StatusCode = StatusCodes.Status401Unauthorized;
                        context.Response.ContentType = "application/json";
                        var result = System.Text.Json.JsonSerializer.Serialize(new
                        {
                            error = "unauthorized",
                            message = "Authentication required"
                        });
                        return context.Response.WriteAsync(result);
                    }
                    context.Response.Redirect(context.RedirectUri);
                    return Task.CompletedTask;
                };
                options.Events.OnRedirectToAccessDenied = context =>
                {
                    if (context.Request.Path.StartsWithSegments("/api"))
                    {
                        context.Response.StatusCode = StatusCodes.Status403Forbidden;
                        context.Response.ContentType = "application/json";
                        var result = System.Text.Json.JsonSerializer.Serialize(new
                        {
                            error = "forbidden",
                            message = "Access denied"
                        });
                        return context.Response.WriteAsync(result);
                    }
                    context.Response.Redirect(context.RedirectUri);
                    return Task.CompletedTask;
                };
            });
            services.AddDbContext<AuthorizationDbContext>(options =>
                options.UseSqlServer(Configuration.GetConnectionString("AuthorizationConnection")));
            services.AddHttpContextAccessor();
            string currentUserName = Environment.UserName;
            string connectionString = Configuration.GetConnectionString("DefaultConnection");
            if (currentUserName.Equals("macpciShieldapp", StringComparison.InvariantCultureIgnoreCase))
            {
                connectionString = Configuration.GetSection("ConnectionStrings:KarlaConnection").Value;
            }
            services.AddDbContext<AppDbContext>(c => c.UseSqlServer(connectionString));
            ConfigureServices(services);
            var sqlServerStorageOptions = new SqlServerStorageOptions
            {
                PrepareSchemaIfNecessary = true,
                CommandBatchMaxTimeout = TimeSpan.FromMinutes(5),
                SlidingInvisibilityTimeout = TimeSpan.FromMinutes(5),
                QueuePollInterval = TimeSpan.Zero,
                UseRecommendedIsolationLevel = true,
                DisableGlobalLocks = true
            };
            services.AddHangfire(config => config.UseSqlServerStorage(connectionString, sqlServerStorageOptions));
            services.AddHangfireServer();
            try
            {
                string? quartzConnectionString = connectionString;
                if (!string.IsNullOrWhiteSpace(connectionString))
                {
                    quartzConnectionString = connectionString
                        .Replace("PCIShield_Core_Db", "pciShieldAppBackgroundJobs")
                        .Replace("pciShieldAppBackgroundMessages", "pciShieldAppBackgroundJobs");
                    var builder = new SqlConnectionStringBuilder(quartzConnectionString)
                    {
                        TrustServerCertificate = true,
                        ConnectTimeout = 30
                    };
                    quartzConnectionString = builder.ToString();
                    using (var connection = new SqlConnection(quartzConnectionString.Replace("Database=pciShieldAppBackgroundJobs;", "Database=master;")))
                    {
                        connection.Open();
                        var command = new SqlCommand(
                            "IF NOT EXISTS (SELECT * FROM sys.databases WHERE name = 'pciShieldAppBackgroundJobs') CREATE DATABASE pciShieldAppBackgroundJobs;",
                            connection);
                        command.ExecuteNonQuery();
                    }
                    var scheduler = QuartzConfiguration.ConfigureQuartz(quartzConnectionString).GetAwaiter().GetResult();
                    services.AddSingleton(scheduler);
                }
            }
            catch (Exception e)
            {
                throw new ApplicationException("Failed to initialize Quartz scheduler", e);
            }
            string? serilogConnectionString = connectionString;
            if (!string.IsNullOrWhiteSpace(connectionString))
            {
                serilogConnectionString = connectionString.Replace("PCIShield_Core_Db", "PCIShieldAppLogs");
                serilogConnectionString = serilogConnectionString.Replace("pciShieldAppBackgroundMessages", "PCIShieldAppLogs");
                serilogConnectionString = serilogConnectionString.Replace("pciShieldAppBackgroundJobs", "PCIShieldAppLogs");
            }
            Logger? logger = new LoggerConfiguration()
                .MinimumLevel.Error()
                .MinimumLevel.Override("Microsoft", LogEventLevel.Error)
                .Enrich.FromLogContext()
                .WriteTo.Console()
                .WriteTo.MSSqlServer(serilogConnectionString,
                    new MSSqlServerSinkOptions { TableName = "Logs", SchemaName = "dbo", AutoCreateSqlTable = true })
                .CreateLogger();
            services.AddSingleton<ILogger>(logger);
            services.AddSingleton(new SqlConnection(connectionString));
            services.AddProblemDetails(options =>
            {
                options.CustomizeProblemDetails = context =>
                {
                    var traceId = Activity.Current?.Id ?? context.HttpContext.TraceIdentifier;
                    context.ProblemDetails.Extensions["traceId"] = traceId;
                };
            });

            services.AddOracle23AiServices(Configuration);
            var conn = Configuration.GetConnectionString("OracleConnection");
            var name = Configuration["Oracle23Ai:ModelName"] ?? "ALL_MINILM_L12_V2";
            var dims = int.Parse(Configuration["Oracle23Ai:Dimensions"] ?? "384");

            // ═══════════════════════════════════════════════════════════════════
            // A2UI: OpenAI IChatClient + AG-UI Agent Services
            // ═══════════════════════════════════════════════════════════════════
            var openAiApiKey = Configuration["OPENAI_API_KEY"]
                ?? Environment.GetEnvironmentVariable("OPENAI_API_KEY")
                ?? throw new InvalidOperationException("OPENAI_API_KEY is not configured. Add it to .env file.");

            var openAiModel = Configuration["OPENAI_MODEL"]
                ?? Environment.GetEnvironmentVariable("OPENAI_MODEL")
                ?? "gpt-5-mini"; // Default to GPT-5 mini

            // Register IChatClient using OpenAI
            services.AddSingleton<IChatClient>(sp =>
            {
                var openAiClient = new OpenAIClient(openAiApiKey);
                var chatClient = openAiClient.GetChatClient(openAiModel);
                return chatClient.AsIChatClient();
            });

            // Register AG-UI agent services
            services.AddPCIShieldAgents();

        }
        public void ConfigureServices(IServiceCollection services)
        {
            services.AddSingleton<EnhancedRouteAnalyzer>();
            using var scope = services.BuildServiceProvider().CreateScope();
            var analyzer = scope.ServiceProvider.GetRequiredService<EnhancedRouteAnalyzer>();
            var endpointTypes = AppDomain.CurrentDomain.GetAssemblies()
                .SelectMany(a => a.GetTypes())
                .Where(t => !t.IsAbstract && !t.IsInterface &&
                            typeof(Endpoint).IsAssignableFrom(t));
            var endpoints = endpointTypes.Select(t =>
            {
                var baseEndpointType = t.BaseType;
                while (baseEndpointType != null && !baseEndpointType.IsGenericType)
                {
                    baseEndpointType = baseEndpointType.BaseType;
                }
                var args = baseEndpointType?.GetGenericArguments() ?? Type.EmptyTypes;
                return new EndpointDefinition(
                    t,
                    args.Length > 0 ? args[0] : typeof(object),
                    args.Length > 1 ? args[1] : typeof(object)
                );
            });
            var duplicates = analyzer.AnalyzeEndpoints(endpoints);
            if (duplicates.Any())
            {
                var message = string.Join("\n", duplicates.Select(d =>
                    $"Route: {d.Key}\nExisting: {d.ExistingType}\nDuplicate: {d.DuplicateType}"));
                if (_env.IsDevelopment())
                {
                    analyzer.WriteAnalysisToFile(Path.Combine(_env.ContentRootPath, "duplicate-routes.txt"));
                }
                else
                {
                    throw new InvalidOperationException($"Duplicate routes found:\n{message}");
                }
            }
            services.AddResponseCompression(options =>
            {
                options.EnableForHttps = true;
                options.Providers.Add<BrotliCompressionProvider>();
                options.Providers.Add<GzipCompressionProvider>();
            });
            services.AddFastEndpoints(c =>
            {
                c.DisableAutoDiscovery = false;
            }).AddResponseCaching();
            services.
                AddValidatorsFromAssemblyContaining<UpdateMerchantValidator>();
            services.AddTransient<DuplicateRouteCheckerMiddleware>();
            services.SwaggerDocument(o =>
            {
                o.ShortSchemaNames = true;
                o.DocumentSettings = s =>
                {
                    s.Title = "PCIShield API";
                    s.Version = "v1";
                };
            });
            if (_env.IsDevelopment())
            {
                services.AddHttpLogging(logging =>
                {
                    logging.LoggingFields = Microsoft.AspNetCore.HttpLogging.HttpLoggingFields.RequestProperties |
                                            Microsoft.AspNetCore.HttpLogging.HttpLoggingFields.ResponsePropertiesAndHeaders;
                    logging.RequestHeaders.Add("User-Agent");
                    logging.ResponseHeaders.Add("Content-Type");
                    logging.RequestBodyLogLimit = 0;
                    logging.ResponseBodyLogLimit = 0;
                });
            }
            services.AddLogging(loggingBuilder =>
            {
                loggingBuilder.AddSerilog();
            });
            services.AddCqrs();

            services.AddSignalR().AddHubOptions<PCIShieldAppApiSignalrHub>(o =>
            {
                o.StatefulReconnectBufferSize = 1000;
                o.ClientTimeoutInterval = TimeSpan.FromMinutes(2);
                o.KeepAliveInterval = TimeSpan.FromSeconds(30);
            });

            services.AddOptions();
#pragma warning disable EXTEXP0018
            services.AddCustomHybridCache();
#pragma warning restore EXTEXP0018
            string currentUserName = Environment.UserName;
            string connectionString = Configuration.GetConnectionString("DefaultConnection");
            if (currentUserName.Equals("macpciShieldapp", StringComparison.InvariantCultureIgnoreCase))
            {
                connectionString = Configuration.GetSection("ConnectionStrings:KarlaConnection").Value;
            }
            string? dapperConnectionString = connectionString;
            if (!string.IsNullOrWhiteSpace(connectionString))
            {
                dapperConnectionString = connectionString.Replace("PCIShield_Core_Db", "pciShieldAppBackgroundMessages");
            }
            var sqlServerStorageOptions = new SqlServerStorageOptions
            {
                PrepareSchemaIfNecessary = true,
                CommandBatchMaxTimeout = TimeSpan.FromMinutes(5),
                SlidingInvisibilityTimeout = TimeSpan.FromMinutes(5),
                QueuePollInterval = TimeSpan.Zero,
                UseRecommendedIsolationLevel = true,
                DisableGlobalLocks = true
            };
            services.AddHangfire(config => config.UseSqlServerStorage(connectionString, sqlServerStorageOptions));
            services.AddSingleton(typeof(IApplicationSettings), typeof(OfficeSettings));
            BaseUrlConfiguration? baseUrlConfig = new();
            Configuration.Bind(BaseUrlConfiguration.CONFIG_NAME, baseUrlConfig);
            services.AddCors(options =>
                options.AddDefaultPolicy(builder => builder.AllowAnyOrigin().AllowAnyMethod().AllowAnyHeader()));
            services.AddControllers(options =>
                {
                })
                .AddJsonOptions(options =>
                {
                    options.JsonSerializerOptions.PropertyNamingPolicy = System.Text.Json.JsonNamingPolicy.CamelCase;
                    options.JsonSerializerOptions.WriteIndented = true;
                    options.JsonSerializerOptions.ReferenceHandler = System.Text.Json.Serialization.ReferenceHandler.IgnoreCycles;
                    options.JsonSerializerOptions.DefaultIgnoreCondition = System.Text.Json.Serialization.JsonIgnoreCondition.WhenWritingNull;
                    options.JsonSerializerOptions.Converters.Add(new System.Text.Json.Serialization.JsonStringEnumConverter());
                });
            Assembly[]? assemblies =
                new[] { typeof(Startup).Assembly, typeof(AppDbContext).Assembly, typeof(Merchant).Assembly };
            services.AddMediatR(cfg =>
            {
                cfg.RegisterServicesFromAssemblies(assemblies);
                cfg.AddOpenBehavior(typeof(RequestExceptionProcessorBehavior<,>));
                cfg.AddOpenBehavior(typeof(RequestExceptionActionProcessorBehavior<,>));
                cfg.AddOpenBehavior(typeof(RequestPreProcessorBehavior<,>));
                cfg.AddOpenBehavior(typeof(RequestPostProcessorBehavior<,>));
                cfg.NotificationPublisher = new TaskWhenAllPublisher();
                cfg.MaxGenericTypeParameters = 10;
                cfg.MaxTypesClosing = 100;
                cfg.MaxGenericTypeRegistrations = 125000;
                cfg.RegistrationTimeout = 15000;
            });
            services.AddResponseCompression(opts =>
                opts.MimeTypes = ResponseCompressionDefaults.MimeTypes.Concat(new[] { "application/octet-stream" }));
            services.AddSwaggerGenCustom();
            services.AddScoped<IDbLocalizationService, DbLocalizationService>();
            services.AddScoped<IStringLocalizer, DbStringLocalizer>();
            services.AddScoped<IStringLocalizerFactory, DbStringLocalizerFactory>();

            services.AddLocalization(options => options.ResourcesPath = "Resources");

            var supportedCultures = new[] { "en-US", "es-ES" };
            services.Configure<RequestLocalizationOptions>(options =>
            {
                options.DefaultRequestCulture = new RequestCulture("en-US");
                options.SupportedCultures = supportedCultures.Select(c => new CultureInfo(c)).ToList();
                options.SupportedUICultures = supportedCultures.Select(c => new CultureInfo(c)).ToList();
            });
            services.AddSingleton<ComplianceOfficerMerchantConnections>();
            services.AddSingleton<IHeartbeatService, HeartbeatService>();
            services.AddSingleton<ChatEventProcessor>();
            services.AddSingleton<ProcessedMessagesStore>();
            services.AddSingleton<CleanupService>();

            services.AddTransient<PCIShieldAppApiSignalrHub>();

            services.AddTransient<PaymentChannelService>();
            services.AddTransient<AssessmentService>();

            services.AddTransient<MerchantService>();
        }
        public void ConfigureTestingServices(IServiceCollection services)
        {
            ConfigureInMemoryDatabases(services);
        }
        private void ConfigureInMemoryDatabases(IServiceCollection services)
        {
            services.AddDbContext<AppDbContext>(c => c.UseInMemoryDatabase("AppDb"));
            ConfigureServices(services);
        }
        private static void ConfigureEventStoreDbService(IServiceCollection services)
        {
            services.AddMediatR(
                cfg =>
                {
                    cfg.RegisterServicesFromAssembly(typeof(Program).Assembly);
                });
        }
    }
    public sealed class Utf8JsonResolver : IJsonFormatterResolver
    {
        public static readonly Utf8JsonResolver Instance = new();
        private readonly IJsonFormatterResolver[] _resolvers;
        private Utf8JsonResolver()
        {
            _resolvers = new[]
            {
                StandardResolver.Default,
                StandardResolver.AllowPrivate,
                StandardResolver.ExcludeNull,
                EnumResolver.Default,
            };
        }
        public IJsonFormatter<T> GetFormatter<T>() =>
            CompositeResolver.Create(_resolvers).GetFormatter<T>();
    }
    public class JwtSettings
    {
        public string PCISHIELDAPP_ISSUER { get; set; } = "";
        public string PCISHIELDAPP_AUDIENCE { get; set; } = "";
        public string PCISHIELDAPP_SECRET_KEY { get; set; } = "";
        public int TokenExpirationMinutes { get; set; }
        public int RefreshTokenExpirationDays { get; set; }
        public int ClockSkewMinutes { get; set; }
        public bool ValidateIssuer { get; set; }
        public bool ValidateAudience { get; set; }
        public bool ValidateLifetime { get; set; }
        public bool ValidateIssuerSigningKey { get; set; }
        public bool RequireExpirationTime { get; set; }
        public bool RequireSignedTokens { get; set; }
    }

}
