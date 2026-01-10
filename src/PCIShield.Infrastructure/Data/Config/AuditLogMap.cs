using System;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using PCIShield.Domain.Entities;

namespace PCIShield.Infrastructure.Data.Config;

public  class AuditLogConfiguration
    : IEntityTypeConfiguration<AuditLog>
{
    public void Configure(EntityTypeBuilder<AuditLog> builder)
    {
        builder.ToTable("AuditLog", "dbo");
        builder.HasKey(t => t.AuditLogId).IsClustered();
        builder.Property(t => t.AuditLogId)
            .IsRequired()
            .HasColumnName("AuditLogId")
            .HasColumnType("uniqueidentifier").ValueGeneratedNever();

        builder.Property(t => t.TenantId)
            .IsRequired()
            .HasColumnName("TenantId")
            .HasColumnType("uniqueidentifier");

        builder.Property(t => t.EntityType)
            .IsRequired()
            .HasColumnName("EntityType")
            .HasColumnType("nvarchar(100)")
            .HasMaxLength(100);

        builder.Property(t => t.EntityId)
            .IsRequired()
            .HasColumnName("EntityId")
            .HasColumnType("uniqueidentifier");

        builder.Property(t => t.Action)
            .IsRequired()
            .HasColumnName("Action")
            .HasColumnType("nvarchar(50)")
            .HasMaxLength(50);

        builder.Property(t => t.OldValues)
            .HasColumnName("OldValues")
            .HasColumnType("nvarchar(max)");

        builder.Property(t => t.NewValues)
            .HasColumnName("NewValues")
            .HasColumnType("nvarchar(max)");

        builder.Property(t => t.UserId)
            .IsRequired()
            .HasColumnName("UserId")
            .HasColumnType("uniqueidentifier");

        builder.Property(t => t.IPAddress)
            .HasColumnName("IPAddress")
            .HasColumnType("nvarchar(45)")
            .HasMaxLength(45);
    }

}
