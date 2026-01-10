using System;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using PCIShield.Domain.Entities;

namespace PCIShield.Infrastructure.Data.Config;

public  class EvidenceConfiguration
    : IEntityTypeConfiguration<Evidence>
{
    public void Configure(EntityTypeBuilder<Evidence> builder)
    {
        builder.ToTable("Evidence", "dbo");
        builder.HasKey(t => t.EvidenceId).IsClustered();
        builder.Property(t => t.EvidenceId)
            .IsRequired()
            .HasColumnName("EvidenceId")
            .HasColumnType("uniqueidentifier").ValueGeneratedNever();

        builder.Property(t => t.TenantId)
            .IsRequired()
            .HasColumnName("TenantId")
            .HasColumnType("uniqueidentifier");

        builder.Property(t => t.MerchantId)
            .IsRequired()
            .HasColumnName("MerchantId")
            .HasColumnType("uniqueidentifier");

        builder.Property(t => t.EvidenceCode)
            .IsRequired()
            .HasColumnName("EvidenceCode")
            .HasColumnType("nvarchar(50)")
            .HasMaxLength(50);

        builder.Property(t => t.EvidenceTitle)
            .IsRequired()
            .HasColumnName("EvidenceTitle")
            .HasColumnType("nvarchar(200)")
            .HasMaxLength(200);

        builder.Property(t => t.EvidenceType)
            .IsRequired()
            .HasColumnName("EvidenceType")
            .HasColumnType("int");

        builder.Property(t => t.CollectedDate)
            .IsRequired()
            .HasColumnName("CollectedDate")
            .HasColumnType("datetime2");

        builder.Property(t => t.FileHash)
            .HasColumnName("FileHash")
            .HasColumnType("nvarchar(128)")
            .HasMaxLength(128);

        builder.Property(t => t.StorageUri)
            .HasColumnName("StorageUri")
            .HasColumnType("nvarchar(500)")
            .HasMaxLength(500);

        builder.Property(t => t.IsValid)
            .IsRequired()
            .HasColumnName("IsValid")
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
        builder.HasOne(t => t.Merchant)
            .WithMany(t => t.Evidences)
            .HasForeignKey(d => d.MerchantId)
            .HasConstraintName("FK_Evidence_Merchant");

    }

}
