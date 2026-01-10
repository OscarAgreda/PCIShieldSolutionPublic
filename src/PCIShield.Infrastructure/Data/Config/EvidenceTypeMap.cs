using System;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using PCIShield.Domain.Entities;

namespace PCIShield.Infrastructure.Data.Config;

public  class EvidenceTypeConfiguration
    : IEntityTypeConfiguration<EvidenceType>
{
    public void Configure(EntityTypeBuilder<EvidenceType> builder)
    {
        builder.ToTable("EvidenceType", "dbo");
        builder.HasKey(t => t.EvidenceTypeId).IsClustered();
        builder.Property(t => t.EvidenceTypeId)
            .IsRequired()
            .HasColumnName("EvidenceTypeId")
            .HasColumnType("uniqueidentifier").ValueGeneratedNever();

        builder.Property(t => t.EvidenceTypeCode)
            .IsRequired()
            .HasColumnName("EvidenceTypeCode")
            .HasColumnType("nvarchar(30)")
            .HasMaxLength(30);

        builder.Property(t => t.EvidenceTypeName)
            .IsRequired()
            .HasColumnName("EvidenceTypeName")
            .HasColumnType("nvarchar(100)")
            .HasMaxLength(100);

        builder.Property(t => t.FileExtensions)
            .HasColumnName("FileExtensions")
            .HasColumnType("nvarchar(200)")
            .HasMaxLength(200);

        builder.Property(t => t.MaxSizeMB)
            .HasColumnName("MaxSizeMB")
            .HasColumnType("int");

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
    }

}
