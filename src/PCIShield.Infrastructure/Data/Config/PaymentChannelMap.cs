using System;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using PCIShield.Domain.Entities;

namespace PCIShield.Infrastructure.Data.Config;

public  class PaymentChannelConfiguration
    : IEntityTypeConfiguration<PaymentChannel>
{
    public void Configure(EntityTypeBuilder<PaymentChannel> builder)
    {
        builder.ToTable("PaymentChannel", "dbo");
        builder.HasKey(t => t.PaymentChannelId).IsClustered();
        builder.Property(t => t.PaymentChannelId)
            .IsRequired()
            .HasColumnName("PaymentChannelId")
            .HasColumnType("uniqueidentifier").ValueGeneratedNever();

        builder.Property(t => t.TenantId)
            .IsRequired()
            .HasColumnName("TenantId")
            .HasColumnType("uniqueidentifier");

        builder.Property(t => t.MerchantId)
            .IsRequired()
            .HasColumnName("MerchantId")
            .HasColumnType("uniqueidentifier");

        builder.Property(t => t.ChannelCode)
            .IsRequired()
            .HasColumnName("ChannelCode")
            .HasColumnType("nvarchar(30)")
            .HasMaxLength(30);

        builder.Property(t => t.ChannelName)
            .IsRequired()
            .HasColumnName("ChannelName")
            .HasColumnType("nvarchar(100)")
            .HasMaxLength(100);

        builder.Property(t => t.ChannelType)
            .IsRequired()
            .HasColumnName("ChannelType")
            .HasColumnType("int");

        builder.Property(t => t.ProcessingVolume)
            .IsRequired()
            .HasColumnName("ProcessingVolume")
            .HasColumnType("decimal(18,2)");

        builder.Property(t => t.IsInScope)
            .IsRequired()
            .HasColumnName("IsInScope")
            .HasColumnType("bit")
            .HasDefaultValue(true);

        builder.Property(t => t.TokenizationEnabled)
            .IsRequired()
            .HasColumnName("TokenizationEnabled")
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
            .WithMany(t => t.PaymentChannels)
            .HasForeignKey(d => d.MerchantId)
            .HasConstraintName("FK_PaymentChannel_Merchant");

    }

}
