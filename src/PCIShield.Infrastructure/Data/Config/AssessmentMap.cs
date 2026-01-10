using System;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using PCIShield.Domain.Entities;

namespace PCIShield.Infrastructure.Data.Config;

public  class AssessmentConfiguration
    : IEntityTypeConfiguration<Assessment>
{
    public void Configure(EntityTypeBuilder<Assessment> builder)
    {
        builder.ToTable("Assessment", "dbo");
        builder.HasKey(t => t.AssessmentId).IsClustered();
        builder.Property(t => t.AssessmentId)
            .IsRequired()
            .HasColumnName("AssessmentId")
            .HasColumnType("uniqueidentifier").ValueGeneratedNever();

        builder.Property(t => t.TenantId)
            .IsRequired()
            .HasColumnName("TenantId")
            .HasColumnType("uniqueidentifier");

        builder.Property(t => t.MerchantId)
            .IsRequired()
            .HasColumnName("MerchantId")
            .HasColumnType("uniqueidentifier");

        builder.Property(t => t.AssessmentCode)
            .IsRequired()
            .HasColumnName("AssessmentCode")
            .HasColumnType("nvarchar(30)")
            .HasMaxLength(30);

        builder.Property(t => t.AssessmentType)
            .IsRequired()
            .HasColumnName("AssessmentType")
            .HasColumnType("int");

        builder.Property(t => t.AssessmentPeriod)
            .IsRequired()
            .HasColumnName("AssessmentPeriod")
            .HasColumnType("nvarchar(20)")
            .HasMaxLength(20);

        builder.Property(t => t.StartDate)
            .IsRequired()
            .HasColumnName("StartDate")
            .HasColumnType("date");

        builder.Property(t => t.EndDate)
            .IsRequired()
            .HasColumnName("EndDate")
            .HasColumnType("date");

        builder.Property(t => t.CompletionDate)
            .HasColumnName("CompletionDate")
            .HasColumnType("date");

        builder.Property(t => t.Rank)
            .IsRequired()
            .HasColumnName("Rank")
            .HasColumnType("int");

        builder.Property(t => t.ComplianceScore)
            .HasColumnName("ComplianceScore")
            .HasColumnType("decimal(5,2)");

        builder.Property(t => t.QSAReviewRequired)
            .IsRequired()
            .HasColumnName("QSAReviewRequired")
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
        builder.HasOne(t => t.Merchant)
            .WithMany(t => t.Assessments)
            .HasForeignKey(d => d.MerchantId)
            .HasConstraintName("FK_Assessment_Merchant");

    }

}
