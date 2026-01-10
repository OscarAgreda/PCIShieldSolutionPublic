using System;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using PCIShield.Domain.Entities;

namespace PCIShield.Infrastructure.Data.Config;

public  class ControlEvidenceConfiguration
    : IEntityTypeConfiguration<ControlEvidence>
{
    public void Configure(EntityTypeBuilder<ControlEvidence> builder)
    {
        builder.ToTable("ControlEvidence", "dbo");
        builder.HasKey(t => t.RowId).IsClustered();
        builder.Property(t => t.RowId)
            .IsRequired()
            .HasColumnName("RowId")
            .HasColumnType("int")
            .ValueGeneratedOnAdd();

        builder.Property(t => t.ControlId)
            .IsRequired()
            .HasColumnName("ControlId")
            .HasColumnType("uniqueidentifier");

        builder.Property(t => t.EvidenceId)
            .IsRequired()
            .HasColumnName("EvidenceId")
            .HasColumnType("uniqueidentifier");

        builder.Property(t => t.AssessmentId)
            .IsRequired()
            .HasColumnName("AssessmentId")
            .HasColumnType("uniqueidentifier");

        builder.Property(t => t.TenantId)
            .IsRequired()
            .HasColumnName("TenantId")
            .HasColumnType("uniqueidentifier");

        builder.Property(t => t.IsPrimary)
            .IsRequired()
            .HasColumnName("IsPrimary")
            .HasColumnType("bit")
            .HasDefaultValue(false);

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
            .WithMany(t => t.ControlEvidences)
            .HasForeignKey(d => d.AssessmentId)
            .HasConstraintName("FK_ControlEvidence_Assessment");

        builder.HasOne(t => t.Control)
            .WithMany(t => t.ControlEvidences)
            .HasForeignKey(d => d.ControlId)
            .HasConstraintName("FK_ControlEvidence_Control");

        builder.HasOne(t => t.Evidence)
            .WithMany(t => t.ControlEvidences)
            .HasForeignKey(d => d.EvidenceId)
            .HasConstraintName("FK_ControlEvidence_Evidence");

    }

}
