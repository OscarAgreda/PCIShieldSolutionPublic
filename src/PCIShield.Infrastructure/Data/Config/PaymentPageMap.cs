using System;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using PCIShield.Domain.Entities;

namespace PCIShield.Infrastructure.Data.Config;

public  class PaymentPageConfiguration
    : IEntityTypeConfiguration<PaymentPage>
{
    public void Configure(EntityTypeBuilder<PaymentPage> builder)
    {
        builder.ToTable("PaymentPage", "dbo");
        builder.HasKey(t => t.PaymentPageId).IsClustered();
        builder.Property(t => t.PaymentPageId)
            .IsRequired()
            .HasColumnName("PaymentPageId")
            .HasColumnType("uniqueidentifier").ValueGeneratedNever();

        builder.Property(t => t.TenantId)
            .IsRequired()
            .HasColumnName("TenantId")
            .HasColumnType("uniqueidentifier");

        builder.Property(t => t.PaymentChannelId)
            .IsRequired()
            .HasColumnName("PaymentChannelId")
            .HasColumnType("uniqueidentifier");

        builder.Property(t => t.PageUrl)
            .IsRequired()
            .HasColumnName("PageUrl")
            .HasColumnType("nvarchar(500)")
            .HasMaxLength(500);

        builder.Property(t => t.PageName)
            .IsRequired()
            .HasColumnName("PageName")
            .HasColumnType("nvarchar(200)")
            .HasMaxLength(200);

        builder.Property(t => t.IsActive)
            .IsRequired()
            .HasColumnName("IsActive")
            .HasColumnType("bit")
            .HasDefaultValue(true);

        builder.Property(t => t.LastScriptInventory)
            .HasColumnName("LastScriptInventory")
            .HasColumnType("datetime2");

        builder.Property(t => t.ScriptIntegrityHash)
            .HasColumnName("ScriptIntegrityHash")
            .HasColumnType("nvarchar(128)")
            .HasMaxLength(128);

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
        builder.HasOne(t => t.PaymentChannel)
            .WithMany(t => t.PaymentPages)
            .HasForeignKey(d => d.PaymentChannelId)
            .HasConstraintName("FK_PaymentPage_PaymentChannel");

    }

}
