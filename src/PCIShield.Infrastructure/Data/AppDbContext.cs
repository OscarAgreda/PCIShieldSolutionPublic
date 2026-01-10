using MediatR;
using Microsoft.AspNetCore.SignalR;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.ChangeTracking;
using Microsoft.EntityFrameworkCore.Diagnostics;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using PCIShield.Domain.Entities;
using System;
using System.Data.Common;
using System.Diagnostics;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using PCIShield.Infrastructure.Services;
using PCIShieldLib.SharedKernel;
using System.Collections.Generic;
using PCIShield.Domain.Events;
/*
* make sure package Microsoft.EntityFrameworkCore.Design
   * --RUN THIS FROM PCIShield.Api project folder   << ---------
   * -c	The DbContext class to use. Class name only or fully qualified with namespaces. If this option is omitted, EF Core will find the context class. If there are multiple context classes, this option is required.
   * -p	Relative path to the project folder of the target project. Default value is the current folder.
   * -s	Relative path to the project folder of the startup project. Default value is the current folder.
   * -o	The directory to put files in. Paths are relative to the project directory.
   * to drop the database
   * dotnet ef database drop -c appdbcontext -p ../PCIShield.Infrastructure/PCIShield.Infrastructure.csproj -f -v
   * dotnet ef migrations add initialPCIShieldAppMigration -c appdbcontext -p ../PCIShield.Infrastructure/PCIShield.Infrastructure.csproj -s PCIShield.Api.csproj -o Data/Migrations
   * dotnet ef database update -c appdbcontext --project ../PCIShield.Infrastructure/PCIShield.Infrastructure.csproj -s PCIShield.Api.csproj
   * then look at AppDbContextSeed
   * 
   *  also optimize  the model with :
   *  dotnet ef dbcontext optimize -c appdbcontext -p ../PCIShield.Infrastructure/PCIShield.Infrastructure.csproj
   * 
 */
namespace PCIShield.Infrastructure.Data;

public class AppDbContext : DbContext
{
    private readonly string _sqlConnectionString;
    private readonly IMediator _mediator;
    private readonly IAppLoggerService<AppDbContext> _logger;

    public AppDbContext(DbContextOptions<AppDbContext> options, IConfiguration configuration, IMediator mediator, IAppLoggerService<AppDbContext> logger)
        : base(options)
    {
        _sqlConnectionString = configuration.GetConnectionString("DefaultConnection");
        SavedChanges += PublishEvent;
        _mediator = mediator;
        _logger = logger;

    }
    public DbSet<ApplicationUser> ApplicationUsers { get; set; }
    public DbSet<Assessment> Assessments { get; set; }
    public DbSet<AssessmentControl> AssessmentControls { get; set; }
    public DbSet<AssessmentType> AssessmentTypes { get; set; }
    public DbSet<Asset> Assets { get; set; }
    public DbSet<AssetControl> AssetControls { get; set; }
    public DbSet<AuditLog> AuditLogs { get; set; }
    public DbSet<CompensatingControl> CompensatingControls { get; set; }
    public DbSet<ComplianceOfficer> ComplianceOfficers { get; set; }
    public DbSet<Control> Controls { get; set; }
    public DbSet<ControlCategory> ControlCategories { get; set; }
    public DbSet<ControlEvidence> ControlEvidences { get; set; }
    public DbSet<CryptographicInventory> CryptographicInventories { get; set; }
    public DbSet<Evidence> Evidences { get; set; }
    public DbSet<EvidenceType> EvidenceTypes { get; set; }
    public DbSet<Merchant> Merchants { get; set; }
    public DbSet<NetworkSegmentation> NetworkSegmentations { get; set; }
    public DbSet<PaymentChannel> PaymentChannels { get; set; }
    public DbSet<PaymentPage> PaymentPages { get; set; }
    public DbSet<ROCPackage> ROCPackages { get; set; }
    public DbSet<ScanSchedule> ScanSchedules { get; set; }
    public DbSet<Script> Scripts { get; set; }
    public DbSet<ServiceProvider> ServiceProviders { get; set; }
    public DbSet<Vulnerability> Vulnerabilities { get; set; }
    public DbSet<VulnerabilityRank> VulnerabilityRanks { get; set; }




    public override EntityEntry<TEntity> Add<TEntity>(TEntity entity)
    {
        return base.Add(entity);
    }
    public override ValueTask<EntityEntry> AddAsync(object entity, CancellationToken cancellationToken = new())
    {
        return base.AddAsync(entity, cancellationToken);
    }
    public override EntityEntry<TEntity> Attach<TEntity>(TEntity entity)
    {
        return base.Attach(entity);
    }
    public override EntityEntry Attach(object entity)
    {
        return base.Attach(entity);
    }
    public override EntityEntry<TEntity> Remove<TEntity>(TEntity entity)
    {
        return base.Remove(entity);
    }
    public override int SaveChanges()
    {
        return SaveChangesAsync().GetAwaiter().GetResult();
    }
    //private void LogChanges(string methodType)
    //{
    //    var stackTrace = new StackTrace();
    //    var frame = stackTrace.GetFrame(3); // Adjust the frame index based on your call stack
    //    var method = frame.GetMethod();
    //    var declaringType = method.DeclaringType;
    //    foreach (var entry in ChangeTracker.Entries<Message>())
    //    {
    //        if (entry.Entity is Message)
    //        {
    //            _logger.LogInformation($"Changes initiated by {declaringType?.FullName}.{method.Name} - {methodType}");
    //            _logger.LogInformation($"Message change: {entry.Entity.GetType().Name}, ID: {entry.Entity.MessageId}, State: {entry.State}");
    //        }
    //    }
    //}
    public override async Task<int> SaveChangesAsync(CancellationToken cancellationToken = new())
    {
        try
        {
            // int result = await base.SaveChangesAsync(cancellationToken).ConfigureAwait(false);
            // Capture the events before they are cleared
            // var events = GetEvents().ToList();
            //If you want fine-grained control and different handling for different event types:
            // Publish the events individually using Hangfire
            // Approach 1: Individual handling
            //foreach (INotification domainEvent in events)
            //{
            //    if (domainEvent is EfDomainEvent message)
            //    {
            //        switch (message)
            //        {
            //            case EfMessageCreatedEvent createdEvent:
            //                _hangFireService.PublishEfMessageCreatedEvent(createdEvent);
            //                break;
            //            case EfMessageUpdatedEvent updatedEvent:
            //                _hangFireService.PublishEfMessageUpdatedEvent(updatedEvent);
            //                break;
            //            case CustomerCacheRetrievedEvent customerCacheRetrievedEvent:
            //                _hangFireService.PublishCustomerCacheRetrievedEvent(customerCacheRetrievedEvent);
            //                break;
            //            case EfConversationUpdatedCreateReconnEvent updatedEvent:
            //                _hangFireService.ProcessConversationUpdatedCreateReconEvent(updatedEvent);
            //                break;
            //            default:
            //                _logger.LogWarning($"Unhandled domain event type: {message.GetType().Name}");
            //                break;
            //        }
            //    }
            //}
            // Approach 2: Batch handling
            // Publish the events in the background using Hangfire
            //  _hangFireService.PublishEventsInBackground(events);
            // Temporarily disable automatic detection for large change sets
            bool originalAutoDetectChanges = ChangeTracker.AutoDetectChangesEnabled;
            int entryCount = ChangeTracker.Entries().Count();
            if (entryCount > 100)
            {
                ChangeTracker.AutoDetectChangesEnabled = false;
            }
            int result = await base.SaveChangesAsync(cancellationToken).ConfigureAwait(false);
            // Restore original setting
            ChangeTracker.AutoDetectChangesEnabled = originalAutoDetectChanges;
            // Capture the events before they are cleared
            var events = GetEvents().ToList();
            return result;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "An error occurred while saving changes.");
            throw;
        }
    }
    private IEnumerable<INotification> GetEvents()
    {
        var entitiesWithEvents = ChangeTracker
            .Entries()
            .Select(e => e.Entity as BaseEntityEv<Guid>)
            .Where(e => e?.Events != null && e.Events.Any())
            .ToList();
        foreach (var entity in entitiesWithEvents)
        {
            if (entity != null)
            {
                var events = entity.Events.ToArray();
                entity.Events.Clear();
                foreach (var domainEvent in events)
                {
                    yield return domainEvent;
                }
            }
        }
    }
    public override EntityEntry<TEntity> Update<TEntity>(TEntity entity)
    {
        return base.Update(entity);
    }
    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.ApplyGlobalStandards();
        // Carefull, Cautiously use TPT, only if essential for your model. for use in pciShield is not necesary due to the way i use projections on the specificaion queries
        // modelBuilder.Entity<Customer>().UseTptMappingStrategy();
        // --- SQL Server conventions: RowVersion & audit Timestamp ---
        modelBuilder.ApplySqlServerRowVersionConvention(options =>
        {
            // Opt-in: only entities listed here will map a shadow RowVersion byte[] column.
            // Add types that ALREADY have a SQL Server 'rowversion' column.
            options.RowVersionedEntities = new HashSet<Type>
            {
                typeof(Assessment),
                typeof(Asset),
                typeof(Control),
                typeof(Evidence),
                typeof(Merchant),
                // add more as needed
            };
            options.ColumnName = "RowVersion";    // matches your DB
            options.ColumnType = "rowversion";    // SQL Server type
        });

        modelBuilder.ApplySqlServerAuditTimestampConvention();

        base.OnModelCreating(modelBuilder);
    }
    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
    {
        if (!optionsBuilder.IsConfigured)
        {
            optionsBuilder.UseSqlServer(
                    _sqlConnectionString,
                    sqlOptions =>
                    {
                        //sqlOptions.MigrationsAssembly("PCIShield.Infrastructure");
                        sqlOptions.EnableRetryOnFailure(
                            maxRetryCount: 3,
                            maxRetryDelay: TimeSpan.FromSeconds(3),
                            errorNumbersToAdd: null);
                        sqlOptions.UseQuerySplittingBehavior(QuerySplittingBehavior.SplitQuery);
                    })
                .UseQueryTrackingBehavior(QueryTrackingBehavior.NoTrackingWithIdentityResolution); // Greatly improves performance for read-heavy scenarios
                                                                                                   //.LogTo(Console.WriteLine, new[] { DbLoggerCategory.Database.Command.Name }, LogLevel.Information)
                                                                                                   //;
                                                                                                   // Enable sensitive data logging only in development environment
            if (Environment.GetEnvironmentVariable("ASPNETCORE_ENVIRONMENT") == "Development")
            {
                optionsBuilder.EnableSensitiveDataLogging();
                optionsBuilder.LogTo(Console.WriteLine, LogLevel.Information);
            }
        }
    }
    private static void PublishEvent(object sender, SavedChangesEventArgs e)
    {
        SavedChangesEventArgs? aa = e;
    }
    //public override void Dispose()
    //{
    //    // Ensure connections are properly released
    //    Database.CloseConnection();
    //    base.Dispose();
    //}
    //public override ValueTask DisposeAsync()
    //{
    //    // Ensure connections are properly released asynchronously
    //    Database.CloseConnection();
    //    return base.DisposeAsync();
    //}
}
