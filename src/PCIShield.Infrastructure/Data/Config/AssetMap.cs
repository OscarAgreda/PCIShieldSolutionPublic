using System;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using PCIShield.Domain.Entities;

namespace PCIShield.Infrastructure.Data.Config;

public  class AssetConfiguration
    : IEntityTypeConfiguration<Asset>
{
    public void Configure(EntityTypeBuilder<Asset> builder)
    {
        builder.ToTable("Asset", "dbo");
        builder.HasKey(t => t.AssetId).IsClustered();
        builder.Property(t => t.AssetId)
            .IsRequired()
            .HasColumnName("AssetId")
            .HasColumnType("uniqueidentifier").ValueGeneratedNever();

        builder.Property(t => t.TenantId)
            .IsRequired()
            .HasColumnName("TenantId")
            .HasColumnType("uniqueidentifier");

        builder.Property(t => t.MerchantId)
            .IsRequired()
            .HasColumnName("MerchantId")
            .HasColumnType("uniqueidentifier");

        builder.Property(t => t.AssetCode)
            .IsRequired()
            .HasColumnName("AssetCode")
            .HasColumnType("nvarchar(50)")
            .HasMaxLength(50);

        builder.Property(t => t.AssetName)
            .IsRequired()
            .HasColumnName("AssetName")
            .HasColumnType("nvarchar(200)")
            .HasMaxLength(200);

        builder.Property(t => t.AssetType)
            .IsRequired()
            .HasColumnName("AssetType")
            .HasColumnType("int");

        builder.Property(t => t.IPAddress)
            .HasColumnName("IPAddress")
            .HasColumnType("nvarchar(45)")
            .HasMaxLength(45);

        builder.Property(t => t.Hostname)
            .HasColumnName("Hostname")
            .HasColumnType("nvarchar(255)")
            .HasMaxLength(255);

        builder.Property(t => t.IsInCDE)
            .IsRequired()
            .HasColumnName("IsInCDE")
            .HasColumnType("bit")
            .HasDefaultValue(true);

        builder.Property(t => t.NetworkZone)
            .HasColumnName("NetworkZone")
            .HasColumnType("nvarchar(100)")
            .HasMaxLength(100);

        builder.Property(t => t.LastScanDate)
            .HasColumnName("LastScanDate")
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
            .WithMany(t => t.Assets)
            .HasForeignKey(d => d.MerchantId)
            .HasConstraintName("FK_Asset_Merchant");

    }

}
