using System;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using PCIShield.Domain.Entities;

namespace PCIShield.Infrastructure.Data.Config;

public  class ControlConfiguration
    : IEntityTypeConfiguration<Control>
{
    public void Configure(EntityTypeBuilder<Control> builder)
    {
        builder.ToTable("Control", "dbo");
        builder.HasKey(t => t.ControlId).IsClustered();
        builder.Property(t => t.ControlId)
            .IsRequired()
            .HasColumnName("ControlId")
            .HasColumnType("uniqueidentifier").ValueGeneratedNever();

        builder.Property(t => t.TenantId)
            .IsRequired()
            .HasColumnName("TenantId")
            .HasColumnType("uniqueidentifier");

        builder.Property(t => t.ControlCode)
            .IsRequired()
            .HasColumnName("ControlCode")
            .HasColumnType("nvarchar(30)")
            .HasMaxLength(30);

        builder.Property(t => t.RequirementNumber)
            .IsRequired()
            .HasColumnName("RequirementNumber")
            .HasColumnType("nvarchar(20)")
            .HasMaxLength(20);

        builder.Property(t => t.ControlTitle)
            .IsRequired()
            .HasColumnName("ControlTitle")
            .HasColumnType("nvarchar(500)")
            .HasMaxLength(500);

        builder.Property(t => t.ControlDescription)
            .IsRequired()
            .HasColumnName("ControlDescription")
            .HasColumnType("nvarchar(max)");

        builder.Property(t => t.TestingGuidance)
            .HasColumnName("TestingGuidance")
            .HasColumnType("nvarchar(max)");

        builder.Property(t => t.FrequencyDays)
            .IsRequired()
            .HasColumnName("FrequencyDays")
            .HasColumnType("int");

        builder.Property(t => t.IsMandatory)
            .IsRequired()
            .HasColumnName("IsMandatory")
            .HasColumnType("bit")
            .HasDefaultValue(true);

        builder.Property(t => t.EffectiveDate)
            .IsRequired()
            .HasColumnName("EffectiveDate")
            .HasColumnType("date");

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
