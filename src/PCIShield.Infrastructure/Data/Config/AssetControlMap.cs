using System;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using PCIShield.Domain.Entities;

namespace PCIShield.Infrastructure.Data.Config;

public  class AssetControlConfiguration
    : IEntityTypeConfiguration<AssetControl>
{
    public void Configure(EntityTypeBuilder<AssetControl> builder)
    {
        builder.ToTable("AssetControl", "dbo");
        builder.HasKey(t => t.RowId).IsClustered();
        builder.Property(t => t.RowId)
            .IsRequired()
            .HasColumnName("RowId")
            .HasColumnType("int")
            .ValueGeneratedOnAdd();

        builder.Property(t => t.AssetId)
            .IsRequired()
            .HasColumnName("AssetId")
            .HasColumnType("uniqueidentifier");

        builder.Property(t => t.ControlId)
            .IsRequired()
            .HasColumnName("ControlId")
            .HasColumnType("uniqueidentifier");

        builder.Property(t => t.TenantId)
            .IsRequired()
            .HasColumnName("TenantId")
            .HasColumnType("uniqueidentifier");

        builder.Property(t => t.IsApplicable)
            .IsRequired()
            .HasColumnName("IsApplicable")
            .HasColumnType("bit")
            .HasDefaultValue(true);

        builder.Property(t => t.CustomizedApproach)
            .HasColumnName("CustomizedApproach")
            .HasColumnType("nvarchar(max)");

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
        builder.HasOne(t => t.Asset)
            .WithMany(t => t.AssetControls)
            .HasForeignKey(d => d.AssetId)
            .HasConstraintName("FK_AssetControl_Asset");

        builder.HasOne(t => t.Control)
            .WithMany(t => t.AssetControls)
            .HasForeignKey(d => d.ControlId)
            .HasConstraintName("FK_AssetControl_Control");

    }

}
