using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
namespace PCIShield.Infrastructure.Data;
/*
* to drop the database
* 
* 
   * dotnet ef database drop -c AuthorizationDbContext -p ../PCIShield.Infrastructure/PCIShield.Infrastructure.csproj -f -v
   
   * dotnet ef migrations remove -c AuthorizationDbContext -p ../PCIShield.Infrastructure/PCIShield.Infrastructure.csproj -s PCIShield.Api.csproj
   * 
   * 
   * dotnet ef migrations add initialPCIShieldAppAuthMigration -c AuthorizationDbContext -p ../PCIShield.Infrastructure/PCIShield.Infrastructure.csproj -s PCIShield.Api.csproj -o Data/Migrations
   * 
   * 
   * dotnet ef database update -c AuthorizationDbContext --project ../PCIShield.Infrastructure/PCIShield.Infrastructure.csproj -s PCIShield.Api.csproj
 */
public class AuthorizationDbContext : IdentityDbContext<CustomPCIShieldUser>
{
    public AuthorizationDbContext(DbContextOptions<AuthorizationDbContext> options)
        : base(options)
    {
    }

    public DbSet<AppLocalization> AppLocalizations { get; set; }

    public DbSet<Permission> Permissions { get; set; } = null!;
    public DbSet<RolePermission> RolePermissions { get; set; } = null!;
    public DbSet<UserPermission> UserPermissions { get; set; } = null!;
    public DbSet<LoginHistory> LoginHistories { get; set; } = null!;
    public DbSet<AuthAuditLog> AuthAuditLogs { get; set; } = null!;

    protected override void OnModelCreating(ModelBuilder builder)
    {
        base.OnModelCreating(builder);

        // Create a unique index on Key + Culture + TenantId
        builder.Entity<AppLocalization>()
            .HasIndex(l => new { l.Key, l.Culture, l.TenantId })
            .IsUnique()
            .HasFilter("[IsDeleted] = 0");

        // Company indexes
        builder.Entity<Company>()
            .HasIndex(c => c.CompanyCode)
            .IsUnique();

        builder.Entity<Company>()
            .HasIndex(c => c.TaxIdentificationNumber)
            .IsUnique();

        // CompanyUserRole index
        builder.Entity<CompanyUserRole>()
            .HasIndex(r => new { r.CompanyId, r.UserId, r.IsActive })
            .IsUnique()
            .HasFilter("[IsDeleted] = 0 AND [IsActive] = 1");


        // ===========================
        // Permission / RolePermission / UserPermission
        // ===========================

        // Permission
        builder.Entity<Permission>(entity =>
        {
            entity.HasKey(e => e.Id);
            entity.Property(e => e.Id).ValueGeneratedNever(); // matches DatabaseGenerated.None

            entity.HasIndex(e => e.Key).IsUnique();
            entity.HasIndex(e => e.Category);
            entity.HasIndex(e => e.IsSystemPermission);

            // Match your attribute style (keep Fluent as the single source of truth)
            entity.Property(e => e.Key)
                .HasMaxLength(255)
                .IsRequired();

            entity.Property(e => e.DisplayName)
                .HasMaxLength(255)
                .IsRequired();

            entity.Property(e => e.Category)
                .HasMaxLength(100)
                .IsRequired();

            entity.Property(e => e.Description)
                .HasMaxLength(2048);

            entity.Property(e => e.IsSystemPermission)
                .HasDefaultValue(false)
                .IsRequired();

            // Optional: default timestamps (provider-agnostic defaults are safer set in code)
            // entity.Property(e => e.CreatedAt).HasDefaultValueSql("GETUTCDATE()");
        });

        // RolePermission
        builder.Entity<RolePermission>(entity =>
        {
            entity.HasKey(e => e.Id);
            entity.Property(e => e.Id).ValueGeneratedNever();

            // Uniqueness: one row per (RoleId, PermissionId)
            entity.HasIndex(e => new { e.RoleId, e.PermissionId }).IsUnique();

            // FK -> Permission
            entity.HasOne(e => e.Permission)
                .WithMany(p => p.RolePermissions)
                .HasForeignKey(e => e.PermissionId)
                .OnDelete(DeleteBehavior.Cascade);

            // Sizes (RoleId/GrantedBy follow IdentityUser's nvarchar(450))
            entity.Property(e => e.RoleId)
                .HasMaxLength(450)
                .IsRequired();

            entity.Property(e => e.GrantedBy)
                .HasMaxLength(450);

            entity.Property(e => e.GrantedAt)
                .IsRequired();
        });

        // UserPermission
        builder.Entity<UserPermission>(entity =>
        {
            entity.HasKey(e => e.Id);
            entity.Property(e => e.Id).ValueGeneratedNever();

            // Uniqueness: one row per (UserId, PermissionId)
            entity.HasIndex(e => new { e.UserId, e.PermissionId }).IsUnique();

            // FK -> Permission
            entity.HasOne(e => e.Permission)
                .WithMany(p => p.UserPermissions)
                .HasForeignKey(e => e.PermissionId)
                .OnDelete(DeleteBehavior.Cascade);

            // Sizes (UserId/GrantedBy follow IdentityUser's nvarchar(450))
            entity.Property(e => e.UserId)
                .HasMaxLength(450)
                .IsRequired();

            entity.Property(e => e.GrantedBy)
                .HasMaxLength(450);

            entity.Property(e => e.IsGranted)
                .IsRequired();

            entity.Property(e => e.GrantedAt)
                .IsRequired();
        });
        // LoginHistory
        builder.Entity<LoginHistory>(entity =>
        {
            entity.HasKey(e => e.Id);
            entity.Property(e => e.Id).ValueGeneratedNever();

            entity.Property(e => e.UserId)
                  .HasMaxLength(450)
                  .IsRequired();

            entity.Property(e => e.LoginDate)
                  .IsRequired();

            entity.Property(e => e.IpAddress)
                  .HasMaxLength(64);

            entity.Property(e => e.UserAgent)
                  .HasMaxLength(1024);

            entity.Property(e => e.Success)
                  .IsRequired()
                  .HasDefaultValue(false);

            entity.Property(e => e.FailureReason)
                  .HasMaxLength(1024);

            // Indexes for common lookups
            entity.HasIndex(e => e.UserId);
            entity.HasIndex(e => e.LoginDate);
            entity.HasIndex(e => new { e.UserId, e.LoginDate }); // efficient timeline per user
            entity.HasIndex(e => e.Success);                      // quick failure filtering

            // FK to Identity user
            entity.HasOne(e => e.User)
                  .WithMany() // no back-collection on CustomPCIShieldUser
                  .HasForeignKey(e => e.UserId)
                  .HasPrincipalKey(u => u.Id)
                  .OnDelete(DeleteBehavior.Cascade);
        });
        // AuditLog
        builder.Entity<AuthAuditLog>(entity =>
        {
            entity.HasKey(e => e.Id);
            entity.Property(e => e.Id).ValueGeneratedNever();

            // Indexes for common queries
            entity.HasIndex(e => e.UserId);
            entity.HasIndex(e => e.Timestamp);
            entity.HasIndex(e => new { e.Category, e.Action });
            entity.HasIndex(e => new { e.UserId, e.Timestamp });
            entity.HasIndex(e => new { e.Category, e.Severity, e.Timestamp });

            // Column lengths / requirements (kept aligned with attributes)
            entity.Property(e => e.Action).HasMaxLength(100).IsRequired();
            entity.Property(e => e.Category).HasMaxLength(50).IsRequired();
            entity.Property(e => e.Severity).HasMaxLength(20).IsRequired();
            entity.Property(e => e.IpAddress).HasMaxLength(50);
            entity.Property(e => e.UserAgent).HasMaxLength(1024);
            entity.Property(e => e.ErrorMessage).HasMaxLength(1024);
            entity.Property(e => e.Description).HasMaxLength(1024);
            entity.Property(e => e.TargetEntityId).HasMaxLength(450);
            entity.Property(e => e.TargetEntityType).HasMaxLength(100);

            entity.Property(e => e.Timestamp).IsRequired();
            entity.Property(e => e.Success).IsRequired();

            // Optional: default timestamp at DB level (SQL Server)
            // entity.Property(e => e.Timestamp).HasDefaultValueSql("GETUTCDATE()");

            // FK to Identity user; keep logs if the user is removed
            entity.HasOne(e => e.User)
                  .WithMany()
                  .HasForeignKey(e => e.UserId)
                  .HasPrincipalKey(u => u.Id)
                  .OnDelete(DeleteBehavior.SetNull);
        });

    }

}