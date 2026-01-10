using System;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using PCIShield.Domain.Entities;

namespace PCIShield.Infrastructure.Data.Config;

public  class ControlCategoryConfiguration
    : IEntityTypeConfiguration<ControlCategory>
{
    public void Configure(EntityTypeBuilder<ControlCategory> builder)
    {
        builder.ToTable("ControlCategory", "dbo");
        builder.HasKey(t => t.ControlCategoryId).IsClustered();
        builder.Property(t => t.ControlCategoryId)
            .IsRequired()
            .HasColumnName("ControlCategoryId")
            .HasColumnType("uniqueidentifier").ValueGeneratedNever();

        builder.Property(t => t.ControlCategoryCode)
            .IsRequired()
            .HasColumnName("ControlCategoryCode")
            .HasColumnType("nvarchar(30)")
            .HasMaxLength(30);

        builder.Property(t => t.ControlCategoryName)
            .IsRequired()
            .HasColumnName("ControlCategoryName")
            .HasColumnType("nvarchar(100)")
            .HasMaxLength(100);

        builder.Property(t => t.RequirementSection)
            .IsRequired()
            .HasColumnName("RequirementSection")
            .HasColumnType("nvarchar(20)")
            .HasMaxLength(20);

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
