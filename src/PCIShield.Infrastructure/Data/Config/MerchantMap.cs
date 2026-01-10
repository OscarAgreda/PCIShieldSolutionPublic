using System;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using PCIShield.Domain.Entities;

namespace PCIShield.Infrastructure.Data.Config;

public  class MerchantConfiguration
    : IEntityTypeConfiguration<Merchant>
{
    public void Configure(EntityTypeBuilder<Merchant> builder)
    {
        builder.ToTable("Merchant", "dbo");
        builder.HasKey(t => t.MerchantId).IsClustered();
        builder.Property(t => t.MerchantId)
            .IsRequired()
            .HasColumnName("MerchantId")
            .HasColumnType("uniqueidentifier").ValueGeneratedNever();

        builder.Property(t => t.TenantId)
            .IsRequired()
            .HasColumnName("TenantId")
            .HasColumnType("uniqueidentifier");

        builder.Property(t => t.MerchantCode)
            .IsRequired()
            .HasColumnName("MerchantCode")
            .HasColumnType("nvarchar(30)")
            .HasMaxLength(30);

        builder.Property(t => t.MerchantName)
            .IsRequired()
            .HasColumnName("MerchantName")
            .HasColumnType("nvarchar(200)")
            .HasMaxLength(200);

        builder.Property(t => t.MerchantLevel)
            .IsRequired()
            .HasColumnName("MerchantLevel")
            .HasColumnType("int");

        builder.Property(t => t.AcquirerName)
            .IsRequired()
            .HasColumnName("AcquirerName")
            .HasColumnType("nvarchar(200)")
            .HasMaxLength(200);

        builder.Property(t => t.ProcessorMID)
            .IsRequired()
            .HasColumnName("ProcessorMID")
            .HasColumnType("nvarchar(50)")
            .HasMaxLength(50);

        builder.Property(t => t.AnnualCardVolume)
            .IsRequired()
            .HasColumnName("AnnualCardVolume")
            .HasColumnType("decimal(18,2)");

        builder.Property(t => t.LastAssessmentDate)
            .HasColumnName("LastAssessmentDate")
            .HasColumnType("date");

        builder.Property(t => t.NextAssessmentDue)
            .IsRequired()
            .HasColumnName("NextAssessmentDue")
            .HasColumnType("date");

        builder.Property(t => t.ComplianceRank)
            .IsRequired()
            .HasColumnName("ComplianceRank")
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
    }

}
