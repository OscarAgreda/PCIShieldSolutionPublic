using System;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using PCIShield.Domain.Entities;

namespace PCIShield.Infrastructure.Data.Config;

public  class AssessmentTypeConfiguration
    : IEntityTypeConfiguration<AssessmentType>
{
    public void Configure(EntityTypeBuilder<AssessmentType> builder)
    {
        builder.ToTable("AssessmentType", "dbo");
        builder.HasKey(t => t.AssessmentTypeId).IsClustered();
        builder.Property(t => t.AssessmentTypeId)
            .IsRequired()
            .HasColumnName("AssessmentTypeId")
            .HasColumnType("uniqueidentifier").ValueGeneratedNever();

        builder.Property(t => t.AssessmentTypeCode)
            .IsRequired()
            .HasColumnName("AssessmentTypeCode")
            .HasColumnType("nvarchar(30)")
            .HasMaxLength(30);

        builder.Property(t => t.AssessmentTypeName)
            .IsRequired()
            .HasColumnName("AssessmentTypeName")
            .HasColumnType("nvarchar(100)")
            .HasMaxLength(100);

        builder.Property(t => t.Description)
            .HasColumnName("Description")
            .HasColumnType("nvarchar(500)")
            .HasMaxLength(500);

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
