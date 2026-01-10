using System;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using PCIShield.Domain.Entities;

namespace PCIShield.Infrastructure.Data.Config;

public  class ComplianceOfficerConfiguration
    : IEntityTypeConfiguration<ComplianceOfficer>
{
    public void Configure(EntityTypeBuilder<ComplianceOfficer> builder)
    {
        builder.ToTable("ComplianceOfficer", "dbo");
        builder.HasKey(t => t.ComplianceOfficerId).IsClustered();
        builder.Property(t => t.ComplianceOfficerId)
            .IsRequired()
            .HasColumnName("ComplianceOfficerId")
            .HasColumnType("uniqueidentifier").ValueGeneratedNever();

        builder.Property(t => t.TenantId)
            .IsRequired()
            .HasColumnName("TenantId")
            .HasColumnType("uniqueidentifier");

        builder.Property(t => t.MerchantId)
            .IsRequired()
            .HasColumnName("MerchantId")
            .HasColumnType("uniqueidentifier");

        builder.Property(t => t.OfficerCode)
            .IsRequired()
            .HasColumnName("OfficerCode")
            .HasColumnType("nvarchar(30)")
            .HasMaxLength(30);

        builder.Property(t => t.FirstName)
            .IsRequired()
            .HasColumnName("FirstName")
            .HasColumnType("nvarchar(100)")
            .HasMaxLength(100);

        builder.Property(t => t.LastName)
            .IsRequired()
            .HasColumnName("LastName")
            .HasColumnType("nvarchar(100)")
            .HasMaxLength(100);

        builder.Property(t => t.Email)
            .IsRequired()
            .HasColumnName("Email")
            .HasColumnType("nvarchar(320)")
            .HasMaxLength(320);

        builder.Property(t => t.Phone)
            .HasColumnName("Phone")
            .HasColumnType("nvarchar(32)")
            .HasMaxLength(32);

        builder.Property(t => t.CertificationLevel)
            .HasColumnName("CertificationLevel")
            .HasColumnType("nvarchar(50)")
            .HasMaxLength(50);

        builder.Property(t => t.IsActive)
            .IsRequired()
            .HasColumnName("IsActive")
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
            .WithMany(t => t.ComplianceOfficers)
            .HasForeignKey(d => d.MerchantId)
            .HasConstraintName("FK_ComplianceOfficer_Merchant");

    }

}
