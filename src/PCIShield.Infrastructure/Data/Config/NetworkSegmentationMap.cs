using System;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using PCIShield.Domain.Entities;

namespace PCIShield.Infrastructure.Data.Config;

public  class NetworkSegmentationConfiguration
    : IEntityTypeConfiguration<NetworkSegmentation>
{
    public void Configure(EntityTypeBuilder<NetworkSegmentation> builder)
    {
        builder.ToTable("NetworkSegmentation", "dbo");
        builder.HasKey(t => t.NetworkSegmentationId).IsClustered();
        builder.Property(t => t.NetworkSegmentationId)
            .IsRequired()
            .HasColumnName("NetworkSegmentationId")
            .HasColumnType("uniqueidentifier").ValueGeneratedNever();

        builder.Property(t => t.TenantId)
            .IsRequired()
            .HasColumnName("TenantId")
            .HasColumnType("uniqueidentifier");

        builder.Property(t => t.MerchantId)
            .IsRequired()
            .HasColumnName("MerchantId")
            .HasColumnType("uniqueidentifier");

        builder.Property(t => t.SegmentName)
            .IsRequired()
            .HasColumnName("SegmentName")
            .HasColumnType("nvarchar(100)")
            .HasMaxLength(100);

        builder.Property(t => t.VLANId)
            .HasColumnName("VLANId")
            .HasColumnType("int");

        builder.Property(t => t.IPRange)
            .IsRequired()
            .HasColumnName("IPRange")
            .HasColumnType("nvarchar(50)")
            .HasMaxLength(50);

        builder.Property(t => t.FirewallRules)
            .HasColumnName("FirewallRules")
            .HasColumnType("nvarchar(max)");

        builder.Property(t => t.IsInCDE)
            .IsRequired()
            .HasColumnName("IsInCDE")
            .HasColumnType("bit")
            .HasDefaultValue(false);

        builder.Property(t => t.LastValidated)
            .HasColumnName("LastValidated")
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
        builder.HasOne(t => t.Merchant)
            .WithMany(t => t.NetworkSegmentations)
            .HasForeignKey(d => d.MerchantId)
            .HasConstraintName("FK_NetworkSegmentation_Merchant");

    }

}
