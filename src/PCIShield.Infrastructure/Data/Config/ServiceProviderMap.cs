using System;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using PCIShield.Domain.Entities;

namespace PCIShield.Infrastructure.Data.Config;

public  class ServiceProviderConfiguration
    : IEntityTypeConfiguration<ServiceProvider>
{
    public void Configure(EntityTypeBuilder<ServiceProvider> builder)
    {
        builder.ToTable("ServiceProvider", "dbo");
        builder.HasKey(t => t.ServiceProviderId).IsClustered();
        builder.Property(t => t.ServiceProviderId)
            .IsRequired()
            .HasColumnName("ServiceProviderId")
            .HasColumnType("uniqueidentifier").ValueGeneratedNever();

        builder.Property(t => t.TenantId)
            .IsRequired()
            .HasColumnName("TenantId")
            .HasColumnType("uniqueidentifier");

        builder.Property(t => t.MerchantId)
            .IsRequired()
            .HasColumnName("MerchantId")
            .HasColumnType("uniqueidentifier");

        builder.Property(t => t.ProviderName)
            .IsRequired()
            .HasColumnName("ProviderName")
            .HasColumnType("nvarchar(200)")
            .HasMaxLength(200);

        builder.Property(t => t.ServiceType)
            .IsRequired()
            .HasColumnName("ServiceType")
            .HasColumnType("nvarchar(100)")
            .HasMaxLength(100);

        builder.Property(t => t.IsPCICompliant)
            .IsRequired()
            .HasColumnName("IsPCICompliant")
            .HasColumnType("bit");

        builder.Property(t => t.AOCExpiryDate)
            .HasColumnName("AOCExpiryDate")
            .HasColumnType("date");

        builder.Property(t => t.ResponsibilityMatrix)
            .HasColumnName("ResponsibilityMatrix")
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
        builder.HasOne(t => t.Merchant)
            .WithMany(t => t.ServiceProviders)
            .HasForeignKey(d => d.MerchantId)
            .HasConstraintName("FK_ServiceProvider_Merchant");

    }

}
