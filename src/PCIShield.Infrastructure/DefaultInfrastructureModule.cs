using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Reflection;

using Autofac;
using Autofac.Core;
using Autofac.Extensions.DependencyInjection;

using Elastic.Clients.Elasticsearch;
using Elastic.Transport;

using MediatR;
using MediatR.Pipeline;

using Microsoft.Data.SqlClient;
using Microsoft.Extensions.Caching.Hybrid;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Diagnostics.HealthChecks;
using Microsoft.Extensions.Options;

using MongoDB.Driver;

using Org.BouncyCastle.Bcpg;

using PCIShield.Domain.Entities;
using PCIShield.Domain.Events;
using PCIShield.Domain.Interfaces;
using PCIShield.Infrastructure.Behaviors;
using PCIShield.Infrastructure.Data;
using PCIShield.Infrastructure.Helpers;
using PCIShield.Infrastructure.Services;
using PCIShield.Infrastructure.Services.Audit;
using PCIShield.Infrastructure.Services.Elasticsearch;
using PCIShield.Infrastructure.Services.Email;
using PCIShield.Infrastructure.Services.HealthCheck;
using PCIShield.Infrastructure.Services.LockControl;
using PCIShield.Infrastructure.Services.Mongo;
using PCIShield.Infrastructure.Services.OracleAi;
using PCIShield.Infrastructure.Services.Redis;
using PCIShield.Infrastructure.TurnServices;

using PCIShieldLib.SharedKernel.Interfaces;

using Quartz;

using Serilog;
using Serilog.Core;
using Serilog.Sinks.MSSqlServer;

using StackExchange.Redis;

using Module = Autofac.Module;
namespace PCIShield.Infrastructure;



//Noting potential issues in dependency registration
//ConnectionMultiplexer uses a sync method for connection, while async would be a better option. RedisCacheWithCircuitBreaker and RedisCacheRepository registrations may result in conflicts, as Autofac resolves the last registration. Dapper’s SqlConnection should not be registered as SingleInstance due to thread-safety concerns. Quartz scheduler’s synchronous creation in the load could lead to potential deadlocks. Additionally, while MongoClient is suitable as a singleton, the approach of creating it during load seems fine.
//Seen. This infrastructure wiring confirms you’re running a fairly “enterprise ERP” stack: Autofac + MediatR pipelines, EF/UoW/transactions, Redis/Mongo cache layers, Dapper for messaging, Quartz for scheduling, Serilog-to-SQL, Elasticsearch, plus Oracle 23ai vector/CQN.
//Key observations (high-signal):
//* **You are using MediatR** (assembly scan + open generic registrations + pipeline behaviors). If you truly want “your own mediator,” this module is effectively committing you to MediatR semantics unless you swap the core registrations.
//* **Pipeline ordering is doing the heavy lifting** (Exception/Logging/Validation/Transaction/Caching). For sagas + shared UoW, the critical invariant is “transaction commit precedes publish + cache invalidation.” Your `TransactionBehavior<,` is the right anchor point.
//* **You have multiple registrations for the same service**: e.g., `IReadRedisRepository<` is registered as both `RedisCacheWithCircuitBreaker<` and later `RedisCacheRepository<`. In Autofac, the last registration typically wins for single service resolution (unless you intentionally resolve `IEnumerable<`). This is easy to get wrong and leads to “why is my decorator not used?” bugs.
//* **`IDbConnection` as `SingleInstance` is risky**: `SqlConnection` is not intended to be a singleton shared across threads/requests. Prefer a factory or per-scope lifetime.
//* **Building a separate `ServiceProvider` inside the module** for `HybridCache` is a DI smell: you can end up with duplicate singletons and lifetimes that don’t match Autofac. Better to register those services into the same container rather than `BuildServiceProvider()` inside `Load`.
//* **Hard-coded Redis password / endpoints in compile directives**: operationally brittle and a security concern. Configuration/secret store is the right place (even if you keep “pure code” elsewhere).
//* **Quartz scheduler constructed synchronously during container build** (`GetAwaiter().GetResult()`): startup fragility and failure coupling. In production ERP, you generally want scheduler initialization isolated so a transient DB issue doesn’t prevent the app from booting.

public class BehaviorModule : Module
{
    private readonly List<Assembly> _assemblies;
    public BehaviorModule(List<Assembly> assemblies)
    {
        _assemblies = assemblies;
    }

    private void RegisterOracle23Ai(ContainerBuilder builder)
    {
        builder.Register(ctx =>
        {
            var cfg = ctx.Resolve<IConfiguration>();
            var cs = cfg.GetConnectionString("OracleConnection")
                          ?? throw new InvalidOperationException("ConnectionStrings:OracleConnection is missing.");
            var logger = ctx.Resolve<Microsoft.Extensions.Logging.ILogger<Oracle23AiBootstrapper>>();
            return new Oracle23AiBootstrapper(cs, logger);
        })
        .AsSelf()
        .SingleInstance();
        builder.Register(ctx =>
        {
            var cfg = ctx.Resolve<IConfiguration>();
            var cs = cfg.GetConnectionString("OracleConnection")
                          ?? throw new InvalidOperationException("ConnectionStrings:OracleConnection is missing.");
            var logger = ctx.Resolve<Microsoft.Extensions.Logging.ILogger<Oracle23AiVectorService>>();
            return new Oracle23AiVectorService(cs, logger);
        })
        .As<IOracle23AiVectorService>()
        .InstancePerLifetimeScope();
        builder.RegisterType<OracleHealthChecksOptionsSetup>()
               .As<IConfigureOptions<HealthCheckServiceOptions>>()
               .SingleInstance();
        builder.Register(ctx =>
        {
            var cfg = ctx.Resolve<IConfiguration>();
            var cs = cfg.GetConnectionString("OracleConnection")
                          ?? throw new InvalidOperationException("ConnectionStrings:OracleConnection is missing.");
            var schema = cfg["Oracle23Ai:CqnSchema"] ?? throw new InvalidOperationException("Oracle23Ai:CqnSchema is missing.");
            var table = cfg["Oracle23Ai:CqnTable"] ?? throw new InvalidOperationException("Oracle23Ai:CqnTable is missing.");
            var idCol = cfg["Oracle23Ai:CqnIdColumn"] ?? throw new InvalidOperationException("Oracle23Ai:CqnIdColumn is missing.");
            var port = cfg.GetValue<int?>("Oracle23Ai:CqnPort");
            var logger = ctx.Resolve<Microsoft.Extensions.Logging.ILogger<CqnObservable>>();

            return new CqnObservable(cs, schema, table, idCol, logger, port);
        })
        .AsSelf()
        .SingleInstance();
        builder.RegisterBuildCallback(sp =>
        {
            var cfg = sp.Resolve<IConfiguration>();
            if (cfg.GetValue<bool>("Oracle23Ai:EnableCqn"))
            {
                _ = sp.Resolve<CqnObservable>();
            }
        });
        builder.RegisterType<VectorMetricsService>()
       .As<IVectorMetricsService>()
       .InstancePerLifetimeScope();
    }

    protected override void Load(ContainerBuilder builder)
    {
        builder
            .RegisterAssemblyTypes(typeof(IMediator).GetTypeInfo().Assembly)
            .AsImplementedInterfaces();
        var mediatrOpenTypes = new[]
        {
            typeof(IRequestHandler<,>),
            typeof(IRequestHandler<>),
            typeof(IRequestExceptionHandler<,,>),
            typeof(IRequestExceptionAction<,>),
            typeof(INotificationHandler<>),
            typeof(IStreamRequestHandler<,>),
        };
        foreach (var mediatrOpenType in mediatrOpenTypes)
        {
            builder
                .RegisterAssemblyTypes(_assemblies.ToArray())
                .AsClosedTypesOf(mediatrOpenType)
                .AsImplementedInterfaces();
        }
        builder
            .RegisterGeneric(typeof(ExceptionHandlingBehavior<,>))
            .As(typeof(IPipelineBehavior<,>))
            .InstancePerLifetimeScope();
        builder
            .RegisterGeneric(typeof(LoggingBehavior<,>))
            .As(typeof(IPipelineBehavior<,>))
            .InstancePerLifetimeScope();
        builder
            .RegisterGeneric(typeof(ValidationBehavior<,>))
            .As(typeof(IPipelineBehavior<,>))
            .InstancePerLifetimeScope();
        builder
            .RegisterGeneric(typeof(TransactionBehavior<,>))
            .As(typeof(IPipelineBehavior<,>))
            .InstancePerLifetimeScope();
        builder
            .RegisterGeneric(typeof(CachingBehavior<,>))
            .As(typeof(IPipelineBehavior<,>))
            .InstancePerDependency();
        builder
            .RegisterGeneric(typeof(RequestPostProcessorBehavior<,>))
            .As(typeof(IPipelineBehavior<,>));
        builder
            .RegisterGeneric(typeof(RequestPreProcessorBehavior<,>))
            .As(typeof(IPipelineBehavior<,>));
        builder
            .RegisterGeneric(typeof(RequestExceptionActionProcessorBehavior<,>))
            .As(typeof(IPipelineBehavior<,>));
        builder
            .RegisterGeneric(typeof(RequestExceptionProcessorBehavior<,>))
            .As(typeof(IPipelineBehavior<,>));

        RegisterOracle23Ai(builder);
    }
}
public class DefaultInfrastructureModule : Module
{
    private readonly List<Assembly> _assemblies = new();
    private readonly bool _isDevelopment;
    private readonly string _mongoConnectionString;
    private readonly string _redisConnectionString;
    private readonly string _sqlConnectionString;
    public DefaultInfrastructureModule(
        bool isDevelopment,
        string sqlConnectionString,
        string mongoConnectionString,
        string redisConnectionString,
        Assembly callingAssembly = null
    )
    {
        _isDevelopment = isDevelopment;
        _sqlConnectionString = sqlConnectionString;
        _mongoConnectionString = mongoConnectionString;
        _redisConnectionString = redisConnectionString;
        Assembly? domainAssembly = Assembly.GetAssembly(typeof(ApplicationUser));
        if (domainAssembly != null && !_assemblies.Contains(domainAssembly))
        {
            _assemblies.Add(domainAssembly);
        }
        Assembly? infrastructureAssembly = Assembly.GetAssembly(typeof(AppDbContext));
        if (infrastructureAssembly != null && !_assemblies.Contains(infrastructureAssembly))
        {
            _assemblies.Add(infrastructureAssembly);
        }
        if (callingAssembly != null)
        {
            _assemblies.Add(callingAssembly);
        }
    }
    protected override void Load(ContainerBuilder builder)
    {
        RegisterCommonDependencies(builder);
        RegisterElasticsearchDependencies(builder);
    }
    private void RegisterElasticsearchDependencies(ContainerBuilder builder)
    {
        builder.RegisterGeneric(typeof(ElasticsearchService<>))
               .As(typeof(IElasticsearchService<>))
               .InstancePerLifetimeScope();
        builder.Register(context =>
        {
            var configuration = context.Resolve<IConfiguration>();
            var settings = CreateElasticsearchSettings(configuration);
            return new ElasticsearchClient(settings);
        })
        .As<ElasticsearchClient>()
        .SingleInstance();
    }
    private ElasticsearchClientSettings CreateElasticsearchSettings(IConfiguration configuration)
    {
        var cloudId = configuration["Elasticsearch:CloudId"];
        var apiKey = configuration["Elasticsearch:ApiKey"];
        ElasticsearchClientSettings settings;
        if (!string.IsNullOrEmpty(cloudId) && !string.IsNullOrEmpty(apiKey))
        {
            settings = new ElasticsearchClientSettings(cloudId, new ApiKey(apiKey));
        }
        else
        {
            var url = configuration["Elasticsearch:Url"] ?? "http://localhost:9200";
            settings = new ElasticsearchClientSettings(new Uri(url))
                .Authentication(new BasicAuthentication(
                    configuration["Elasticsearch:Username"] ?? "elastic",
                    configuration["Elasticsearch:Password"] ?? ""));
        }
        return settings
            .DefaultIndex(configuration["Elasticsearch:DefaultIndex"])
            .RequestTimeout(TimeSpan.FromMinutes(2))
            .ThrowExceptions()
            .EnableDebugMode(details =>
            {
            });
    }
    private void RegisterCommonDependencies(ContainerBuilder builder)
    {
        builder.RegisterType<RepositoryFactory>()
            .As<IRepositoryFactory>()
            .InstancePerLifetimeScope();
        builder.RegisterType<ReadRedisRepositoryFactory>()
            .As<IReadRedisRepositoryFactory>()
            .InstancePerLifetimeScope();
        builder
            .RegisterGeneric(typeof(EfRepository<>))
            .As(typeof(IRepository<>))
            .InstancePerLifetimeScope();
        builder.RegisterGeneric(typeof(EfRepository<>)).InstancePerLifetimeScope();
		     builder
            .RegisterGeneric(typeof(EfAuthRepository<>))
            .As(typeof(IAuthRepository<>))
            .InstancePerLifetimeScope();
        builder.RegisterGeneric(typeof(EfAuthRepository<>)).InstancePerLifetimeScope();
        builder
            .RegisterType<CacheKeyListService>()
            .As<ICacheKeyListService>()
            .InstancePerLifetimeScope();
        builder.RegisterType<KeyCloakTenantService>().As<IKeyCloakTenantService>().SingleInstance();
        builder
            .RegisterType<UserManagementService>()
            .As<IUserManagementService>()
            .InstancePerLifetimeScope();
#pragma warning disable EXTEXP0018
        var services = new ServiceCollection();
        services.AddHybridCache();
        services.Configure<HybridCacheOptions>(options =>
        {
            options.DefaultEntryOptions = new HybridCacheEntryOptions
            {
                Expiration = TimeSpan.FromSeconds(10),
                LocalCacheExpiration = TimeSpan.FromSeconds(10),
                Flags = HybridCacheEntryFlags.None,
            };
            options.DisableCompression = true;
            options.MaximumPayloadBytes = 2048 * 2048;
            options.MaximumKeyLength = 256;
            options.ReportTagMetrics = false;
        });
        var serviceProvider = services.BuildServiceProvider();
        builder
            .Register(c => serviceProvider.GetRequiredService<HybridCache>())
            .AsSelf()
            .SingleInstance();
        builder
            .Register(context => new HybridCacheOptions
            {
                DefaultEntryOptions = new HybridCacheEntryOptions
                {
                    Expiration = TimeSpan.FromSeconds(10),
                    LocalCacheExpiration = TimeSpan.FromSeconds(10),
                    Flags = HybridCacheEntryFlags.None,
                },
                DisableCompression = true,
                MaximumKeyLength = 256,
                MaximumPayloadBytes = 2048 * 2048,
                ReportTagMetrics = false,
            })
            .AsSelf()
            .SingleInstance();
        builder
            .RegisterGeneric(typeof(InMemoryCacheRepository<>))
            .As(typeof(IReadInMemoryRepository<>))
            .InstancePerLifetimeScope();
#pragma warning restore EXTEXP0018
        builder
            .RegisterType<CircuitBreaker>()
            .As<ICircuitBreaker>()
            .WithParameters(
                new[]
                {
                    new NamedParameter("threshold", 5),
                    new NamedParameter("resetTimeoutSeconds", 30),
                }
            )
            .InstancePerLifetimeScope();
        builder.RegisterType<RedisHealthCheck>().As<IRedisHealthCheck>().InstancePerLifetimeScope();
        builder.RegisterType<DynamicCacheStrategy>().As<ICacheStrategy>().SingleInstance();
        builder
            .RegisterGeneric(typeof(ObservableRedisRepository<>))
            .As(typeof(IObservableRedisRepository<>))
            .InstancePerLifetimeScope();
        builder
            .RegisterGeneric(typeof(RedisCacheWithCircuitBreaker<>))
            .As(typeof(IReadRedisRepository<>))
            .InstancePerLifetimeScope();
#if DEBUG
        builder
            .Register(context =>
            {
                var configuration = new ConfigurationOptions
                {
                    EndPoints = { "localhost:6379" },
                    AbortOnConnectFail = false,
                };
                return ConnectionMultiplexer.Connect(configuration);
            })
            .As<IConnectionMultiplexer>()
            .SingleInstance();

#endif
#if !DEBUG

   builder
            .Register(context =>
            {
                var config = context.Resolve<IConfiguration>();
                var redisHost = config["REDIS_HOST"] ?? "redis";
                var redisPort = config["REDIS_PORT"] ?? "6379";
                var redisPassword = config["REDIS_PASSWORD"] ?? throw new InvalidOperationException("REDIS_PASSWORD environment variable is required");
                
                var configuration = new ConfigurationOptions
                {
                    EndPoints = { $"{redisHost}:{redisPort}" },
                    Password = redisPassword,
                    Ssl = bool.TryParse(config["REDIS_SSL"], out var useSsl) && useSsl,
                    AbortOnConnectFail = false,
                };
                return ConnectionMultiplexer.Connect(configuration);
            })
            .As<IConnectionMultiplexer>()
            .SingleInstance();

#endif
        builder
            .RegisterType<RedisCacheService>()
            .As<IRedisCacheService>()
            .InstancePerLifetimeScope();
        builder
            .RegisterGeneric(typeof(RedisCacheRepository<>))
            .As(typeof(IReadRedisRepository<>))
            .InstancePerLifetimeScope();
        builder
            .RegisterType<MongoCacheService>()
            .As<IMongoCacheService>()
            .InstancePerLifetimeScope();
        builder
            .RegisterGeneric(typeof(MongoCacheRepository<>))
            .As(typeof(IReadMongoRepository<>))
            .InstancePerLifetimeScope();
        builder
            .RegisterType<MerchantComplianceOfficerMessagePublisherService>()
            .As<IMerchantComplianceOfficerMessagePublisherService>()
            .InstancePerLifetimeScope();
        builder.RegisterModule(new BehaviorModule(_assemblies));
        builder.RegisterType<PollyService>().As<IPollyService>().InstancePerLifetimeScope();
        string? dapperConnectionString = _sqlConnectionString;
        if (!string.IsNullOrWhiteSpace(_sqlConnectionString))
        {
            dapperConnectionString = _sqlConnectionString.Replace(
                "PCIShield",
                "PCIShieldBackgroundMessages"
            );
        }
        builder
            .Register(c =>
            {
                IConfiguration? configuration = c.Resolve<IConfiguration>();
                return new SqlConnection(dapperConnectionString);
            })
            .As<IDbConnection>()
            .SingleInstance();
        builder
            .RegisterType<DapperMessagingService>()
            .As<IDapperMessagingService>()
            .InstancePerLifetimeScope();
        string? quartzConnectionString = _sqlConnectionString;
        if (!string.IsNullOrWhiteSpace(_sqlConnectionString))
        {
            quartzConnectionString = _sqlConnectionString.Replace(
                " PCIShield_Core_Db",
                "pciShieldAppBackgroundJobs"
            );
            quartzConnectionString = quartzConnectionString.Replace(
                "pciShieldAppBackgroundMessages",
                "pciShieldAppBackgroundJobs"
            );
        }
        IScheduler? scheduler = QuartzConfiguration
            .ConfigureQuartz(quartzConnectionString)
            .GetAwaiter()
            .GetResult();
        builder.RegisterInstance(scheduler).As<IScheduler>().SingleInstance();
        string? serilogConnectionString = _sqlConnectionString;
        if (!string.IsNullOrWhiteSpace(_sqlConnectionString))
        {
            serilogConnectionString = _sqlConnectionString.Replace(
                "PCIShield_Core_Db",
                "PCIShieldAppLogs"
            );
            serilogConnectionString = serilogConnectionString.Replace(
                "pciShieldAppBackgroundMessages",
                "PCIShieldAppLogs"
            );
            serilogConnectionString = serilogConnectionString.Replace(
                "pciShieldAppBackgroundJobs",
                "PCIShieldAppLogs"
            );
        }
        Logger? logger = new LoggerConfiguration()
            .Enrich.FromLogContext()
            .WriteTo.Console()
            .WriteTo.MSSqlServer(
                serilogConnectionString,
                new MSSqlServerSinkOptions
                {
                    TableName = "Logs",
                    SchemaName = "dbo",
                    AutoCreateSqlTable = true,
                }
            )
            .CreateLogger();
        builder.RegisterInstance(logger).As<ILogger>().SingleInstance();
        builder
            .RegisterGeneric(typeof(AppLoggerService<>))
            .As(typeof(IAppLoggerService<>))
            .SingleInstance();
        builder
            .RegisterType<MailKitEmailServicePlus>()
            .As<IMailKitEmailServicePlus>()
            .InstancePerLifetimeScope();
        MongoClient? mongoClient = new MongoClient(_mongoConnectionString);
        IMongoDatabase? database = mongoClient.GetDatabase("PCIShieldMongoDb");
        builder
            .RegisterType<GridFsBlobStorageService>()
            .As<IGridFsBlobStorageService>()
            .WithParameter("database", database)
            .InstancePerLifetimeScope();
        builder
            .RegisterType<MongoDbContext>()
            .AsSelf()
            .WithParameter("connectionString", _mongoConnectionString)
            .WithParameter("databaseName", "PCIShieldMongoDb")
            .SingleInstance();
        builder
            .RegisterGeneric(typeof(MongoRepository<>))
            .As(typeof(IMongoRepository<>))
            .InstancePerLifetimeScope();
        builder
            .RegisterGeneric(typeof(ResilientHangFireMongoMessageSavingService<>))
            .As(typeof(IResilientHangFireMongoMessageSavingService<>))
            .InstancePerLifetimeScope();
        builder
            .RegisterType<JsonDifferenceFinderService>()
            .As<IJsonDifferenceFinderService>()
            .InstancePerLifetimeScope();
        builder
            .RegisterType<MimeKitEmailSender.MimeKitEmailConfigService>()
            .As<MimeKitEmailSender.IMimeKitEmailConfigService>()
            .InstancePerLifetimeScope();
        builder
            .RegisterType<MimeKitEmailSender.MimeKitEmailSmtpClientFactory>()
            .As<MimeKitEmailSender.IMimeKitSmtpClientFactory>()
            .InstancePerLifetimeScope();
        builder.RegisterType<MimeKitEmailSender>().As<IMimeKitEmailSender>().InstancePerLifetimeScope();
        builder.RegisterType<SmtpEmailStrategy>().As<IEmailStrategy>().InstancePerLifetimeScope();
        builder.RegisterType<EmailService>().As<IEmailService>().InstancePerLifetimeScope();
        builder.RegisterType<TurnService>().As<ITurnService>().InstancePerLifetimeScope();
        builder
            .RegisterType<SynchronizationService>()
            .As<ISynchronizationService>()
            .InstancePerLifetimeScope();
        builder.RegisterType<PCIShield.Infrastructure.Services.HealthCheck.HealthCheckService>().AsSelf().InstancePerLifetimeScope();
        builder.Populate(services);
        builder.RegisterType<AuthAuditService>().As<IAuthAuditService>().InstancePerLifetimeScope();

    }
}
