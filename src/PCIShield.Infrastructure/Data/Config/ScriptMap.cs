using System;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using PCIShield.Domain.Entities;

namespace PCIShield.Infrastructure.Data.Config;

public  class ScriptConfiguration
    : IEntityTypeConfiguration<Script>
{
    public void Configure(EntityTypeBuilder<Script> builder)
    {
        builder.ToTable("Script", "dbo");
        builder.HasKey(t => t.ScriptId).IsClustered();
        builder.Property(t => t.ScriptId)
            .IsRequired()
            .HasColumnName("ScriptId")
            .HasColumnType("uniqueidentifier").ValueGeneratedNever();

        builder.Property(t => t.TenantId)
            .IsRequired()
            .HasColumnName("TenantId")
            .HasColumnType("uniqueidentifier");

        builder.Property(t => t.PaymentPageId)
            .IsRequired()
            .HasColumnName("PaymentPageId")
            .HasColumnType("uniqueidentifier");

        builder.Property(t => t.ScriptUrl)
            .IsRequired()
            .HasColumnName("ScriptUrl")
            .HasColumnType("nvarchar(500)")
            .HasMaxLength(500);

        builder.Property(t => t.ScriptHash)
            .IsRequired()
            .HasColumnName("ScriptHash")
            .HasColumnType("nvarchar(128)")
            .HasMaxLength(128);

        builder.Property(t => t.ScriptType)
            .IsRequired()
            .HasColumnName("ScriptType")
            .HasColumnType("nvarchar(50)")
            .HasMaxLength(50);

        builder.Property(t => t.IsAuthorized)
            .IsRequired()
            .HasColumnName("IsAuthorized")
            .HasColumnType("bit")
            .HasDefaultValue(false);

        builder.Property(t => t.FirstSeen)
            .IsRequired()
            .HasColumnName("FirstSeen")
            .HasColumnType("datetime2");

        builder.Property(t => t.LastSeen)
            .IsRequired()
            .HasColumnName("LastSeen")
            .HasColumnType("datetime2");

        builder.Property(t => t.CreatedAt)
            .IsRequired()
            .HasColumnName("CreatedAt")
            .HasColumnType("datetime2")
            .HasDefaultValueSql("(sysutcdatetime())");

        builder.Property(t => t.CreatedBy)
            .IsRequired()
            .HasColumnName("CreatedBy")
            .HasColumnType("uniqueidentifier");

        builder.Property(t => t.UpdatedAt)
            .HasColumnName("UpdatedAt")
            .HasColumnType("datetime2");

        builder.Property(t => t.UpdatedBy)
            .HasColumnName("UpdatedBy")
            .HasColumnType("uniqueidentifier");

        builder.Property(t => t.IsDeleted)
            .IsRequired()
            .HasColumnName("IsDeleted")
            .HasColumnType("bit")
            .HasDefaultValue(false);
        builder.HasOne(t => t.PaymentPage)
            .WithMany(t => t.Scripts)
            .HasForeignKey(d => d.PaymentPageId)
            .HasConstraintName("FK_Script_PaymentPage");

    }

}
