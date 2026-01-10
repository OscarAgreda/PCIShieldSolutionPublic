using System;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using PCIShield.Domain.Entities;

namespace PCIShield.Infrastructure.Data.Config;

public  class ScanScheduleConfiguration
    : IEntityTypeConfiguration<ScanSchedule>
{
    public void Configure(EntityTypeBuilder<ScanSchedule> builder)
    {
        builder.ToTable("ScanSchedule", "dbo");
        builder.HasKey(t => t.ScanScheduleId).IsClustered();
        builder.Property(t => t.ScanScheduleId)
            .IsRequired()
            .HasColumnName("ScanScheduleId")
            .HasColumnType("uniqueidentifier").ValueGeneratedNever();

        builder.Property(t => t.TenantId)
            .IsRequired()
            .HasColumnName("TenantId")
            .HasColumnType("uniqueidentifier");

        builder.Property(t => t.AssetId)
            .IsRequired()
            .HasColumnName("AssetId")
            .HasColumnType("uniqueidentifier");

        builder.Property(t => t.ScanType)
            .IsRequired()
            .HasColumnName("ScanType")
            .HasColumnType("int");

        builder.Property(t => t.Frequency)
            .IsRequired()
            .HasColumnName("Frequency")
            .HasColumnType("nvarchar(50)")
            .HasMaxLength(50);

        builder.Property(t => t.NextScanDate)
            .IsRequired()
            .HasColumnName("NextScanDate")
            .HasColumnType("datetime2");

        builder.Property(t => t.BlackoutStart)
            .HasColumnName("BlackoutStart")
            .HasColumnType("time");

        builder.Property(t => t.BlackoutEnd)
            .HasColumnName("BlackoutEnd")
            .HasColumnType("time");

        builder.Property(t => t.IsEnabled)
            .IsRequired()
            .HasColumnName("IsEnabled")
            .HasColumnType("bit")
            .HasDefaultValue(true);

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
            .WithMany(t => t.ScanSchedules)
            .HasForeignKey(d => d.AssetId)
            .HasConstraintName("FK_ScanSchedule_Asset");

    }

}
