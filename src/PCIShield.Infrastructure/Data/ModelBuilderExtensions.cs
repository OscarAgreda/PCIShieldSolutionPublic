using Microsoft.EntityFrameworkCore;
using PCIShield.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
namespace PCIShield.Infrastructure.Data
{
    public static class ModelBuilderExtensions
    {
        public static void ApplyGlobalStandards(this ModelBuilder modelBuilder)
        {
            foreach (var entityType in modelBuilder.Model.GetEntityTypes())
            {
                entityType.SetTableName(entityType.DisplayName());
            }
            var cascadeFKs = modelBuilder.Model.GetEntityTypes().SelectMany(t => t.GetForeignKeys()).Where(fk => !fk.IsOwnership && fk.DeleteBehavior == DeleteBehavior.Cascade);
            foreach (var fk in cascadeFKs)
            {
                fk.DeleteBehavior = DeleteBehavior.Cascade;
            }
            foreach (var relationship in modelBuilder.Model.GetEntityTypes().SelectMany(e => e.GetForeignKeys()))
            {
                relationship.DeleteBehavior = DeleteBehavior.Restrict;
            }
            foreach (var entityType in modelBuilder.Model.GetEntityTypes())
            {
                if (entityType.FindProperty("IsDeleted") != null)
                {
                }
            }
            foreach (var entityType in modelBuilder.Model.GetEntityTypes())
            {
                foreach (var property in entityType.GetProperties())
                {
                    if (property.ClrType == typeof(decimal) || property.ClrType == typeof(decimal?))
                    {
                        property.SetColumnType("decimal(18, 2)");
                    }
                }
            }
            foreach (var entityType in modelBuilder.Model.GetEntityTypes())
            {
            }
            ApplyCustomConfigurations(modelBuilder);
        }
        private static void ApplyCustomConfigurations(ModelBuilder modelBuilder)
        {
        }
        public static void HasQueryFilter<TEntity>(ModelBuilder modelBuilder, Expression<Func<TEntity, bool>> filter) where TEntity : class
        {
            modelBuilder.Entity<TEntity>().HasQueryFilter(filter);
        }
        public static void ApplySqlServerRowVersionConvention(this ModelBuilder modelBuilder, Action<RowVersionConventionOptions> configure)
        {
            var options = new RowVersionConventionOptions();
            configure(options);

            foreach (var et in modelBuilder.Model.GetEntityTypes())
            {
                var clr = et.ClrType;
                if (clr is null) continue;

                if (!options.RowVersionedEntities.Contains(clr))
                    continue;
                var builder = modelBuilder.Entity(clr);
                builder.Property<byte[]>(options.ColumnName)
                       .IsRowVersion()
                       .HasColumnType(options.ColumnType);
            }
        }
        public static void ApplySqlServerAuditTimestampConvention(this ModelBuilder modelBuilder)
        {
            foreach (var et in modelBuilder.Model.GetEntityTypes())
            {
                var builder = modelBuilder.Entity(et.ClrType);
                var timestampProp = et.GetProperties()
                    .FirstOrDefault(p =>
                        p.Name == "Timestamp" &&
                        (p.ClrType == typeof(DateTime) || p.ClrType == typeof(DateTime?)
                         || p.ClrType == typeof(DateTimeOffset) || p.ClrType == typeof(DateTimeOffset?)));

                if (timestampProp is null) continue;
                builder.Property(timestampProp.Name)
                       .HasColumnType("datetime2")
                       .HasDefaultValueSql("sysutcdatetime()")
                       .ValueGeneratedOnAdd();
            }
        }

    }

    public sealed class RowVersionConventionOptions
    {
        public HashSet<Type> RowVersionedEntities { get; set; } = new();
        public string ColumnName { get; set; } = "RowVersion";
        public string ColumnType { get; set; } = "rowversion";
    }
}
