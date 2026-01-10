using System;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using PCIShield.Domain.Entities;

namespace PCIShield.Infrastructure.Data.Config;

public  class ApplicationUserConfiguration
    : IEntityTypeConfiguration<ApplicationUser>
{
    public void Configure(EntityTypeBuilder<ApplicationUser> builder)
    {
        builder.ToTable("ApplicationUser", "dbo");
        builder.HasKey(t => t.ApplicationUserId).IsClustered();
        builder.Property(t => t.ApplicationUserId)
            .IsRequired()
            .HasColumnName("ApplicationUserId")
            .HasColumnType("uniqueidentifier").ValueGeneratedNever();

        builder.Property(t => t.FirstName)
            .HasColumnName("FirstName")
            .HasColumnType("nvarchar(255)")
            .HasMaxLength(255);

        builder.Property(t => t.LastName)
            .HasColumnName("LastName")
            .HasColumnType("nvarchar(255)")
            .HasMaxLength(255);

        builder.Property(t => t.UserName)
            .IsRequired()
            .HasColumnName("UserName")
            .HasColumnType("nvarchar(255)")
            .HasMaxLength(255);

        builder.Property(t => t.CreatedDate)
            .IsRequired()
            .HasColumnName("CreatedDate")
            .HasColumnType("datetime2");

        builder.Property(t => t.CreatedBy)
            .IsRequired()
            .HasColumnName("CreatedBy")
            .HasColumnType("uniqueidentifier");

        builder.Property(t => t.UpdatedDate)
            .HasColumnName("UpdatedDate")
            .HasColumnType("datetime2");

        builder.Property(t => t.UpdatedBy)
            .HasColumnName("UpdatedBy")
            .HasColumnType("uniqueidentifier");

        builder.Property(t => t.IsLoginAllowed)
            .IsRequired()
            .HasColumnName("IsLoginAllowed")
            .HasColumnType("bit");

        builder.Property(t => t.LastLogin)
            .HasColumnName("LastLogin")
            .HasColumnType("datetime2");

        builder.Property(t => t.LogoutTime)
            .HasColumnName("LogoutTime")
            .HasColumnType("datetime2");

        builder.Property(t => t.LastFailedLogin)
            .HasColumnName("LastFailedLogin")
            .HasColumnType("datetime2");

        builder.Property(t => t.FailedLoginCount)
            .IsRequired()
            .HasColumnName("FailedLoginCount")
            .HasColumnType("int");

        builder.Property(t => t.Email)
            .HasColumnName("Email")
            .HasColumnType("nvarchar(255)")
            .HasMaxLength(255);

        builder.Property(t => t.AvatarUrl)
     .HasColumnName("AvatarUrl")
     .HasColumnType("nvarchar(255)")
     .HasMaxLength(255);

        builder.Property(t => t.Phone)
            .HasColumnName("Phone")
            .HasColumnType("nvarchar(255)")
            .HasMaxLength(255);

        builder.Property(t => t.IsUserApproved)
            .IsRequired()
            .HasColumnName("IsUserApproved")
            .HasColumnType("bit");

        builder.Property(t => t.IsPhoneVerified)
            .IsRequired()
            .HasColumnName("IsPhoneVerified")
            .HasColumnType("bit");

        builder.Property(t => t.IsEmailVerified)
            .IsRequired()
            .HasColumnName("IsEmailVerified")
            .HasColumnType("bit");

        builder.Property(t => t.ConfirmationEmail)
            .HasColumnName("ConfirmationEmail")
            .HasColumnType("nvarchar(255)")
            .HasMaxLength(255);

        builder.Property(t => t.LastPasswordChange)
            .HasColumnName("LastPasswordChange")
            .HasColumnType("datetime2");

        builder.Property(t => t.IsLocked)
            .IsRequired()
            .HasColumnName("IsLocked")
            .HasColumnType("bit");

        builder.Property(t => t.LockedUntil)
            .HasColumnName("LockedUntil")
            .HasColumnType("datetime2");

        builder.Property(t => t.IsDeleted)
            .IsRequired()
            .HasColumnName("IsDeleted")
            .HasColumnType("bit");

        builder.Property(t => t.IsUserFullyRegistered)
            .IsRequired()
            .HasColumnName("IsUserFullyRegistered")
            .HasColumnType("bit");

        builder.Property(t => t.AvailabilityRank)
            .HasColumnName("AvailabilityRank")
            .HasColumnType("nvarchar(50)")
            .HasMaxLength(50);

        builder.Property(t => t.IsBanned)
            .IsRequired()
            .HasColumnName("IsBanned")
            .HasColumnType("bit");

        builder.Property(t => t.IsFullyRegistered)
            .IsRequired()
            .HasColumnName("IsFullyRegistered")
            .HasColumnType("bit");

        builder.Property(t => t.LastLoginIP)
            .HasColumnName("LastLoginIP")
            .HasColumnType("nvarchar(50)")
            .HasMaxLength(50);

        builder.Property(t => t.LastActiveAt)
            .HasColumnName("LastActiveAt")
            .HasColumnType("datetime2");

        builder.Property(t => t.IsOnline)
            .HasColumnName("IsOnline")
            .HasColumnType("bit");

        builder.Property(t => t.IsConnectedToSignalr)
            .HasColumnName("IsConnectedToSignalr")
            .HasColumnType("bit");

        builder.Property(t => t.TimeLastSignalrPing)
            .HasColumnName("TimeLastSignalrPing")
            .HasColumnType("datetime2");

        builder.Property(t => t.IsLoggedIntoApp)
            .HasColumnName("IsLoggedIntoApp")
            .HasColumnType("bit");

        builder.Property(t => t.TimeLastLoggedToApp)
            .HasColumnName("TimeLastLoggedToApp")
            .HasColumnType("datetime2");

        builder.Property(t => t.AverageResponseTime)
            .HasColumnName("AverageResponseTime")
            .HasColumnType("int");

        builder.Property(t => t.UserIconUrl)
            .HasColumnName("UserIconUrl")
            .HasColumnType("nvarchar(255)")
            .HasMaxLength(255);

        builder.Property(t => t.UserProfileImagePath)
            .HasColumnName("UserProfileImagePath")
            .HasColumnType("nvarchar(255)")
            .HasMaxLength(255);

        builder.Property(t => t.UserBirthDate)
            .HasColumnName("UserBirthDate")
            .HasColumnType("datetime2");

        builder.Property(t => t.CreatedAt)
            .IsRequired()
            .HasColumnName("CreatedAt")
            .HasColumnType("datetime2");

        builder.Property(t => t.UpdatedAt)
            .HasColumnName("UpdatedAt")
            .HasColumnType("datetime2");

        builder.Property(t => t.TenantId)
            .IsRequired()
            .HasColumnName("TenantId")
            .HasColumnType("uniqueidentifier");

        builder.Property(t => t.IsEmployee)
            .HasColumnName("IsEmployee")
            .HasColumnType("bit");

        builder.Property(t => t.IsErpOwner)
            .HasColumnName("IsErpOwner")
            .HasColumnType("bit");

        builder.Property(t => t.IsCustomer)
            .HasColumnName("IsCustomer")
            .HasColumnType("bit");
    }

}
