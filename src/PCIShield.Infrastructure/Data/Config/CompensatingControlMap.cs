using System;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using PCIShield.Domain.Entities;

namespace PCIShield.Infrastructure.Data.Config;

public  class CompensatingControlConfiguration
    : IEntityTypeConfiguration<CompensatingControl>
{
    public void Configure(EntityTypeBuilder<CompensatingControl> builder)
    {
        builder.ToTable("CompensatingControl", "dbo");
        builder.HasKey(t => t.CompensatingControlId).IsClustered();
        builder.Property(t => t.CompensatingControlId)
            .IsRequired()
            .HasColumnName("CompensatingControlId")
            .HasColumnType("uniqueidentifier").ValueGeneratedNever();

        builder.Property(t => t.TenantId)
            .IsRequired()
            .HasColumnName("TenantId")
            .HasColumnType("uniqueidentifier");

        builder.Property(t => t.ControlId)
            .IsRequired()
            .HasColumnName("ControlId")
            .HasColumnType("uniqueidentifier");

        builder.Property(t => t.MerchantId)
            .IsRequired()
            .HasColumnName("MerchantId")
            .HasColumnType("uniqueidentifier");

        builder.Property(t => t.Justification)
            .IsRequired()
            .HasColumnName("Justification")
            .HasColumnType("nvarchar(max)");

        builder.Property(t => t.ImplementationDetails)
            .IsRequired()
            .HasColumnName("ImplementationDetails")
            .HasColumnType("nvarchar(max)");

        builder.Property(t => t.ApprovedBy)
            .HasColumnName("ApprovedBy")
            .HasColumnType("uniqueidentifier");

        builder.Property(t => t.ApprovalDate)
            .HasColumnName("ApprovalDate")
            .HasColumnType("datetime2");

        builder.Property(t => t.ExpiryDate)
            .IsRequired()
            .HasColumnName("ExpiryDate")
            .HasColumnType("date");

        builder.Property(t => t.Rank)
            .IsRequired()
            .HasColumnName("Rank")
            .HasColumnType("int");

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
        builder.HasOne(t => t.Control)
            .WithMany(t => t.CompensatingControls)
            .HasForeignKey(d => d.ControlId)
            .HasConstraintName("FK_CompensatingControl_Control");

        builder.HasOne(t => t.Merchant)
            .WithMany(t => t.CompensatingControls)
            .HasForeignKey(d => d.MerchantId)
            .HasConstraintName("FK_CompensatingControl_Merchant");

    }

}
