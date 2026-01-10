using System;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using PCIShield.Domain.Entities;

namespace PCIShield.Infrastructure.Data.Config;

public  class AssessmentControlConfiguration
    : IEntityTypeConfiguration<AssessmentControl>
{
    public void Configure(EntityTypeBuilder<AssessmentControl> builder)
    {
        builder.ToTable("AssessmentControl", "dbo");
        builder.HasKey(t => t.RowId).IsClustered();
        builder.Property(t => t.RowId)
            .IsRequired()
            .HasColumnName("RowId")
            .HasColumnType("int")
            .ValueGeneratedOnAdd();

        builder.Property(t => t.AssessmentId)
            .IsRequired()
            .HasColumnName("AssessmentId")
            .HasColumnType("uniqueidentifier");

        builder.Property(t => t.ControlId)
            .IsRequired()
            .HasColumnName("ControlId")
            .HasColumnType("uniqueidentifier");

        builder.Property(t => t.TenantId)
            .IsRequired()
            .HasColumnName("TenantId")
            .HasColumnType("uniqueidentifier");

        builder.Property(t => t.TestResult)
            .IsRequired()
            .HasColumnName("TestResult")
            .HasColumnType("int");

        builder.Property(t => t.TestDate)
            .HasColumnName("TestDate")
            .HasColumnType("datetime2");

        builder.Property(t => t.TestedBy)
            .HasColumnName("TestedBy")
            .HasColumnType("uniqueidentifier");

        builder.Property(t => t.Notes)
            .HasColumnName("Notes")
            .HasColumnType("nvarchar(max)");

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
            .WithMany(t => t.AssessmentControls)
            .HasForeignKey(d => d.AssessmentId)
            .HasConstraintName("FK_AssessmentControl_Assessment");

        builder.HasOne(t => t.Control)
            .WithMany(t => t.AssessmentControls)
            .HasForeignKey(d => d.ControlId)
            .HasConstraintName("FK_AssessmentControl_Control");

    }

}
