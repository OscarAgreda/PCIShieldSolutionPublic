using System;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using PCIShield.Domain.Entities;

namespace PCIShield.Infrastructure.Data.Config;

public  class CryptographicInventoryConfiguration
    : IEntityTypeConfiguration<CryptographicInventory>
{
    public void Configure(EntityTypeBuilder<CryptographicInventory> builder)
    {
        builder.ToTable("CryptographicInventory", "dbo");
        builder.HasKey(t => t.CryptographicInventoryId).IsClustered();
        builder.Property(t => t.CryptographicInventoryId)
            .IsRequired()
            .HasColumnName("CryptographicInventoryId")
            .HasColumnType("uniqueidentifier").ValueGeneratedNever();

        builder.Property(t => t.TenantId)
            .IsRequired()
            .HasColumnName("TenantId")
            .HasColumnType("uniqueidentifier");

        builder.Property(t => t.MerchantId)
            .IsRequired()
            .HasColumnName("MerchantId")
            .HasColumnType("uniqueidentifier");

        builder.Property(t => t.KeyName)
            .IsRequired()
            .HasColumnName("KeyName")
            .HasColumnType("nvarchar(200)")
            .HasMaxLength(200);

        builder.Property(t => t.KeyType)
            .IsRequired()
            .HasColumnName("KeyType")
            .HasColumnType("nvarchar(50)")
            .HasMaxLength(50);

        builder.Property(t => t.Algorithm)
            .IsRequired()
            .HasColumnName("Algorithm")
            .HasColumnType("nvarchar(50)")
            .HasMaxLength(50);

        builder.Property(t => t.KeyLength)
            .IsRequired()
            .HasColumnName("KeyLength")
            .HasColumnType("int");

        builder.Property(t => t.KeyLocation)
            .IsRequired()
            .HasColumnName("KeyLocation")
            .HasColumnType("nvarchar(200)")
            .HasMaxLength(200);

        builder.Property(t => t.CreationDate)
            .IsRequired()
            .HasColumnName("CreationDate")
            .HasColumnType("date");

        builder.Property(t => t.LastRotationDate)
            .HasColumnName("LastRotationDate")
            .HasColumnType("date");

        builder.Property(t => t.NextRotationDue)
            .IsRequired()
            .HasColumnName("NextRotationDue")
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
        builder.HasOne(t => t.Merchant)
            .WithMany(t => t.CryptographicInventories)
            .HasForeignKey(d => d.MerchantId)
            .HasConstraintName("FK_CryptographicInventory_Merchant");

    }

}
