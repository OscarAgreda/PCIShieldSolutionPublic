/*
 *I'll provide a complete, optimized solution that consolidates both dependency registrations and includes all necessary components:
   1. **First, the Quartz Configuration:**
   ```csharp
   using static LanguageExt.Prelude;
   using LanguageExt;
   using System.Reactive.Linq;
   using System.Reactive.Subjects;
   using System.Collections.Specialized;
   using Microsoft.Data.SqlClient;
   using Quartz;
   using Quartz.Impl;
   public static class QuartzConfiguration
   {
       public record QuartzOptions(
           string ConnectionString,
           int ThreadCount = 10,
           string InstanceName = "PCIShieldERPScheduler",
           bool IsClustered = true,
           TimeSpan ConnectionTimeout = default)
       {
           public TimeSpan ConnectionTimeout { get; init; } = ConnectionTimeout == default ? TimeSpan.FromSeconds(30) : ConnectionTimeout;
       }
       public static TryAsync<IScheduler> ConfigureQuartz(QuartzOptions options) =>
           from validConn in ValidateConnection(options.ConnectionString, options.ConnectionTimeout)
           from props in CreateQuartzProperties(validConn, options)
           from scheduler in CreateScheduler(props)
           select scheduler;
       private static TryAsync<string> ValidateConnection(string connectionString, TimeSpan timeout) =>
           TryAsync(async () =>
           {
               ArgumentException.ThrowIfNullOrWhiteSpace(connectionString);
               var builder = new SqlConnectionStringBuilder(connectionString)
               {
                   TrustServerCertificate = true,
                   ConnectTimeout = (int)timeout.TotalSeconds
               };
               await using var conn = new SqlConnection(builder.ConnectionString);
               await conn.OpenAsync();
               return builder.ConnectionString;
           });
       private static Try<NameValueCollection> CreateQuartzProperties(string connectionString, QuartzOptions options) =>
           Try(() => new NameValueCollection
           {
               ["quartz.serializer.type"] = "json",
               ["quartz.jobStore.type"] = "Quartz.Impl.AdoJobStore.JobStoreTX, Quartz",
               ["quartz.jobStore.driverDelegateType"] = "Quartz.Impl.AdoJobStore.SqlServerDelegate, Quartz",
               ["quartz.jobStore.tablePrefix"] = "QRTZ_",
               ["quartz.jobStore.dataSource"] = "myDS",
               ["quartz.jobStore.useProperties"] = "true",
               ["quartz.jobStore.performSchemaValidation"] = "false",
               ["quartz.dataSource.myDS.connectionString"] = connectionString,
               ["quartz.dataSource.myDS.provider"] = "SqlServer",
               ["quartz.scheduler.instanceId"] = "AUTO",
               ["quartz.jobStore.clustered"] = options.IsClustered.ToString().ToLower(),
               ["quartz.scheduler.instanceName"] = options.InstanceName,
               ["quartz.threadPool.type"] = "Quartz.Simpl.SimpleThreadPool, Quartz",
               ["quartz.threadPool.threadCount"] = options.ThreadCount.ToString()
           });
       private static TryAsync<IScheduler> CreateScheduler(NameValueCollection props) =>
           TryAsync(async () =>
           {
               var factory = new StdSchedulerFactory(props);
               return await factory.GetScheduler();
           });
   }
   public record HealthCheckResult(bool IsHealthy, string Status, Exception? Exception = null);
   public sealed class QuartzHealthMonitor : IDisposable
   {
       private readonly Subject<HealthCheckResult> _healthStatus = new();
       private readonly CompositeDisposable _disposables = new();
       private readonly ILogger<QuartzHealthMonitor> _logger;
       private readonly IScheduler _scheduler;
       public IObservable<HealthCheckResult> Status => _healthStatus.AsObservable();
       public QuartzHealthMonitor(IScheduler scheduler, ILogger<QuartzHealthMonitor> logger)
       {
           _scheduler = scheduler;
           _logger = logger;
           InitializeMonitoring();
       }
       private void InitializeMonitoring()
       {
           _disposables.Add(
               Observable
                   .Interval(TimeSpan.FromMinutes(1))
                   .SelectMany(_ => CheckHealth())
                   .Subscribe(
                       status => {
                           _healthStatus.OnNext(status);
                           if (!status.IsHealthy)
                               _logger.LogWarning("Quartz scheduler unhealthy: {Status}", status.Status);
                       },
                       ex => _logger.LogError(ex, "Error monitoring Quartz scheduler health")
                   )
           );
           _disposables.Add(
               Observable
                   .FromEventPattern<JobExecutionException>(
                       h => _scheduler.ListenerManager.JobListeners.Add(new JobFailureListener(h)),
                       h => _scheduler.ListenerManager.JobListeners.Clear()
                   )
                   .Subscribe(ex => 
                       _logger.LogError(ex.EventArgs, "Job execution failed: {JobKey}", 
                           ex.EventArgs.JobExecutionContext.JobDetail.Key)
                   )
           );
       }
       private async Task<HealthCheckResult> CheckHealth()
       {
           try
           {
               var isStarted = await _scheduler.IsStarted();
               var inStandbyMode = await _scheduler.InStandbyMode();
               var isShutdown = await _scheduler.IsShutdown();
               if (!isStarted || inStandbyMode || isShutdown)
                   return new HealthCheckResult(false, 
                       $"Scheduler state: Started={isStarted}, Standby={inStandbyMode}, Shutdown={isShutdown}");
               return new HealthCheckResult(true, "Scheduler running normally");
           }
           catch (Exception ex)
           {
               return new HealthCheckResult(false, "Failed to check scheduler health", ex);
           }
       }
       public void Dispose()
       {
           _disposables.Dispose();
           _healthStatus.Dispose();
       }
       private class JobFailureListener : IJobListener
       {
           private readonly Action<JobExecutionException> _onFailure;
           public string Name => "HealthMonitorJobListener";
           public JobFailureListener(Action<JobExecutionException> onFailure)
           {
               _onFailure = onFailure;
           }
           public Task JobExecutionVetoed(IJobExecutionContext context) => Task.CompletedTask;
           public Task JobToBeExecuted(IJobExecutionContext context) => Task.CompletedTask;
           public Task JobWasExecuted(IJobExecutionContext context, JobExecutionException? jobException)
           {
               if (jobException != null)
                   _onFailure(jobException);
               return Task.CompletedTask;
           }
       }
   }
   public static class QuartzModule 
   {
       public static IServiceCollection AddQuartzConfiguration(
           this IServiceCollection services,
           QuartzConfiguration.QuartzOptions options)
       {
           services.AddSingleton(sp => 
               QuartzConfiguration.ConfigureQuartz(options)
                   .Match(
                       Succ: scheduler => scheduler,
                       Fail: ex => throw new ApplicationException("Failed to configure Quartz", ex)
                   ).Result);
           services.AddSingleton<QuartzHealthMonitor>();
           services.AddHostedService<QuartzHostedService>();
           return services;
       }
       public static ContainerBuilder AddQuartzConfiguration(
           this ContainerBuilder builder,
           QuartzConfiguration.QuartzOptions options)
       {
           builder.Register(c => 
               QuartzConfiguration.ConfigureQuartz(options)
                   .Match(
                       Succ: scheduler => scheduler,
                       Fail: ex => throw new ApplicationException("Failed to configure Quartz", ex)
                   ).Result)
               .As<IScheduler>()
               .SingleInstance();
           builder.RegisterType<QuartzHealthMonitor>()
               .AsSelf()
               .SingleInstance();
           return builder;
       }
   }
   public class QuartzHostedService : IHostedService
   {
       private readonly IScheduler _scheduler;
       private readonly QuartzHealthMonitor _monitor;
       private readonly ILogger<QuartzHostedService> _logger;
       public QuartzHostedService(
           IScheduler scheduler,
           QuartzHealthMonitor monitor,
           ILogger<QuartzHostedService> logger)
       {
           _scheduler = scheduler;
           _monitor = monitor;
           _logger = logger;
           _monitor.Status.Subscribe(status =>
           {
               if (!status.IsHealthy)
                   _logger.LogWarning("Quartz health check failed: {Status}", status.Status);
           });
       }
       public async Task StartAsync(CancellationToken cancellationToken)
       {
           try
           {
               await _scheduler.Start(cancellationToken);
               _logger.LogInformation("Quartz scheduler started successfully");
           }
           catch (Exception ex)
           {
               _logger.LogError(ex, "Failed to start Quartz scheduler");
               throw;
           }
       }
       public async Task StopAsync(CancellationToken cancellationToken)
       {
           try
           {
               await _scheduler.Shutdown(true, cancellationToken);
               _logger.LogInformation("Quartz scheduler shut down successfully");
           }
           catch (Exception ex)
           {
               _logger.LogError(ex, "Error shutting down Quartz scheduler");
               throw;
           }
       }
   }
   ```
   Usage in your Startup.cs:
   ```csharp
   public void ConfigureServices(IServiceCollection services)
   {
       var quartzOptions = new QuartzConfiguration.QuartzOptions(
           ConnectionString: Configuration.GetConnectionString("DefaultConnection"),
           ThreadCount: 20,
           InstanceName: "PCIShieldERPScheduler",
           IsClustered: true
       );
       services.AddQuartzConfiguration(quartzOptions);
   }
   ```
   And in your DefaultInfrastructureModule:
   ```csharp
   protected override void Load(ContainerBuilder builder)
   {
       var quartzOptions = new QuartzConfiguration.QuartzOptions(
           ConnectionString: _sqlConnectionString,
           ThreadCount: 20,
           InstanceName: "PCIShieldERPScheduler",
           IsClustered: true
       );
       builder.AddQuartzConfiguration(quartzOptions);
   }
   ```
   Key benefits:
   1. **Unified Configuration**: Single source of configuration for both dependency injection systems
   2. **Health Monitoring**: Built-in health checks with reactive updates
   3. **Error Handling**: Comprehensive error handling using Language-Ext
   4. **Resource Management**: Proper disposal of resources
   5. **Extensibility**: Easy to add new features or monitoring capabilities
   6. **Performance**: Efficient async operations and connection handling
   The code is:
   - More powerful (adds health monitoring, better error handling)
   - More concise (removes duplication)
   - More maintainable (single source of truth)
   - More reliable (proper resource management)
   - More extensible (easy to add new features)
   we could :
   1. Add more monitoring capabilities
   2. Implement specific retry policies
   3. Add additional configuration options
 */
using Microsoft.Data.SqlClient;
using Quartz.Impl;
using System.Collections.Specialized;
using System.Threading.Tasks;
using System;
using Quartz;
public static class QuartzConfiguration
{
    public static async Task<IScheduler> ConfigureQuartz(string connectionString)
    {
        if (string.IsNullOrWhiteSpace(connectionString))
        {
            throw new ArgumentException("Connection string cannot be null or whitespace.", nameof(connectionString));
        }
        var builder = new SqlConnectionStringBuilder(connectionString);
        if (string.IsNullOrWhiteSpace(builder.InitialCatalog))
        {
            throw new ArgumentException("Database name must be specified in connection string");
        }
        var props = new NameValueCollection
        {
            { "quartz.serializer.type", "json" },
            { "quartz.jobStore.type", "Quartz.Impl.AdoJobStore.JobStoreTX, Quartz" },
            { "quartz.jobStore.driverDelegateType", "Quartz.Impl.AdoJobStore.SqlServerDelegate, Quartz" },
            { "quartz.jobStore.tablePrefix", "QRTZ_" },
            { "quartz.jobStore.dataSource", "myDS" },
            { "quartz.jobStore.useProperties", "true" },
            { "quartz.jobStore.performSchemaValidation", "false" },
            { "quartz.dataSource.myDS.connectionString", connectionString },
            { "quartz.dataSource.myDS.provider", "SqlServer" },
            { "quartz.scheduler.instanceId", "AUTO" },
            { "quartz.jobStore.clustered", "true" },
            { "quartz.scheduler.instanceName", "PCIShieldERPScheduler" },
            { "quartz.threadPool.type", "Quartz.Simpl.SimpleThreadPool, Quartz" },
            { "quartz.threadPool.threadCount", "10" }
        };
        try
        {
            using (var conn = new SqlConnection(connectionString))
            {
                await conn.OpenAsync();
            }
            var factory = new StdSchedulerFactory(props);
            var scheduler = await factory.GetScheduler();
            return scheduler;
        }
        catch (Exception ex)
        {
            throw new Exception($"Failed to configure Quartz scheduler: {ex.Message}", ex);
        }
    }
}