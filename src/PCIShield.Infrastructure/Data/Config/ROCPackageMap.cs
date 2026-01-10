using System;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using PCIShield.Domain.Entities;

namespace PCIShield.Infrastructure.Data.Config;

public  class ROCPackageConfiguration
    : IEntityTypeConfiguration<ROCPackage>
{
    public void Configure(EntityTypeBuilder<ROCPackage> builder)
    {
        builder.ToTable("ROCPackage", "dbo");
        builder.HasKey(t => t.ROCPackageId).IsClustered();
        builder.Property(t => t.ROCPackageId)
            .IsRequired()
            .HasColumnName("ROCPackageId")
            .HasColumnType("uniqueidentifier").ValueGeneratedNever();

        builder.Property(t => t.TenantId)
            .IsRequired()
            .HasColumnName("TenantId")
            .HasColumnType("uniqueidentifier");

        builder.Property(t => t.AssessmentId)
            .IsRequired()
            .HasColumnName("AssessmentId")
            .HasColumnType("uniqueidentifier");

        builder.Property(t => t.PackageVersion)
            .IsRequired()
            .HasColumnName("PackageVersion")
            .HasColumnType("nvarchar(20)")
            .HasMaxLength(20);

        builder.Property(t => t.GeneratedDate)
            .IsRequired()
            .HasColumnName("GeneratedDate")
            .HasColumnType("datetime2");

        builder.Property(t => t.QSAName)
            .HasColumnName("QSAName")
            .HasColumnType("nvarchar(200)")
            .HasMaxLength(200);

        builder.Property(t => t.QSACompany)
            .HasColumnName("QSACompany")
            .HasColumnType("nvarchar(200)")
            .HasMaxLength(200);

        builder.Property(t => t.SignatureDate)
            .HasColumnName("SignatureDate")
            .HasColumnType("datetime2");

        builder.Property(t => t.AOCNumber)
            .HasColumnName("AOCNumber")
            .HasColumnType("nvarchar(50)")
            .HasMaxLength(50);

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
        builder.HasOne(t => t.Assessment)
            .WithMany(t => t.ROCPackages)
            .HasForeignKey(d => d.AssessmentId)
            .HasConstraintName("FK_ROCPackage_Assessment");

    }

}
