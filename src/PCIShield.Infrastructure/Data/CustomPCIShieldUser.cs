using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

using PCIShieldLib.SharedKernel.Interfaces;

namespace PCIShield.Infrastructure.Data
{
    public class CustomPCIShieldUser : IdentityUser, IAuthEntity
    {
        public DateTime RefreshTokenExpiryTime { get; set; } = DateTime.UtcNow.AddDays(30);
        public string? RefreshToken { get; set; }
        public Guid ApplicationUserId { get; set; }
        public DateTime CreatedAt { get; set; }
        public Guid CreatedBy { get; set; }
        public string? FirstName { get; set; }
        public bool IsDeleted { get; set; }
        public string? LastName { get; set; }
        public DateTime? LockedUntil { get; set; }
        public Guid TenantId { get; set; }
        public DateTime? UpdatedAt { get; set; }
        public Guid? UpdatedBy { get; set; }
        public DateTimeOffset? LastLogin { get; set; }
        public DateTimeOffset CreatedDate { get; set; }
    }

    [Keyless]
    public class CustomMgmPCIShieldUser : CustomPCIShieldUser
    {
        public List<string> UserClaimsList { get; set; } = new List<string>();
    }

    public class AppLocalization : IAuthEntity
    {
        [Key]
        public int Id { get; set; }

        [Required]
        [MaxLength(255)]
        public string Key { get; set; } = default!;

        [Required]
        [MaxLength(10)]
        public string Culture { get; set; } = default!;

        [Required]
        public string Text { get; set; } = default!;

        public DateTime CreatedAt { get; set; }
        public Guid CreatedBy { get; set; }
        public DateTime? UpdatedAt { get; set; }
        public Guid? UpdatedBy { get; set; }
        public bool IsDeleted { get; set; }
        public Guid TenantId { get; set; }
    }

    [Index(nameof(CompanyCode), IsUnique = true)]
    [Index(nameof(TaxIdentificationNumber), IsUnique = true)]
    [Index(nameof(LegalName))]
    [Index(nameof(TradeName))]
    [Index(nameof(IsDeleted))]
    public class Company
    {
        [Key]
        public Guid CompanyId { get; set; }

        [MaxLength(2048)]
        public string? LogoUrl { get; set; }

        [Required]
        [MaxLength(50)]
        public string CompanyCode { get; set; } = default!;

        [Required]
        [Range(1, 2)]
        [Display(Name = "Legal Entity Type", Description = "1 = Juridical Person, 2 = Natural Person")]
        public int LegalEntityType { get; set; }

        [Required]
        [MaxLength(255)]
        public string LegalName { get; set; } = default!;

        [MaxLength(255)]
        public string? TradeName { get; set; }

        [Required]
        [MaxLength(50)]
        public string TaxIdentificationNumber { get; set; } = default!;

        [MaxLength(500)]
        public string? BusinessActivityDescription { get; set; }

        public Guid? TaxpayerTypeId { get; set; }

        [MaxLength(50)]
        public string? NationalTaxRegistryCode { get; set; }

        [MaxLength(50)]
        public string? SocialSecurityCode { get; set; }

        public Guid? DefaultCurrencyId { get; set; }

        [MaxLength(255)]
        public string? AddressStreetLine1 { get; set; }

        [MaxLength(255)]
        public string? AddressStreetLine2 { get; set; }

        [MaxLength(20)]
        public string? AddressPostalCode { get; set; }

        public Guid? AddressMunicipalityId { get; set; }
        public Guid? AddressStateProvinceId { get; set; }
        public Guid? AddressCountryId { get; set; }

        [MaxLength(254)]
        [EmailAddress]
        public string? CompanyEmailAddress { get; set; }

        [MaxLength(50)]
        [Phone]
        public string? CompanyPhoneNumber { get; set; }

        [MaxLength(2048)]
        [Url]
        public string? CompanyWebsiteUrl { get; set; }

        [Required]
        public DateTime CreatedDate { get; set; }

        [Required]
        public Guid CreatedBy { get; set; }

        public DateTime? UpdatedDate { get; set; }
        public Guid? UpdatedBy { get; set; }

        [Required]
        [DefaultValue(false)]
        public bool IsDeleted { get; set; }
    }

    [Index(nameof(CompanyId))]
    [Index(nameof(UserId))]
    [Index(nameof(CompanySpecificRole))]
    [Index(nameof(CompanyId), nameof(UserId), nameof(IsActive), IsUnique = true)]
    public class CompanyUserRole
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.None)]
        public Guid CompanyUserRoleId { get; set; }

        [Required]
        public Guid CompanyId { get; set; }

        [Required]
        [MaxLength(450)]
        public string UserId { get; set; } = default!;

        [Required]
        [MaxLength(100)]
        [Display(Name = "Company Role", Description = "User's role within this specific company (e.g. Owner, Accountant)")]
        public string CompanySpecificRole { get; set; } = default!;

        [Required]
        public DateTime AssignedDate { get; set; }

        [MaxLength(450)]
        public string? AssignedBy { get; set; }

        [Required]
        [DefaultValue(true)]
        public bool IsActive { get; set; }

        [Required]
        [DefaultValue(false)]
        public bool IsDeleted { get; set; }

        public DateTime? DeletedDate { get; set; }

        [MaxLength(450)]
        public string? DeletedBy { get; set; }
    }
    public sealed class Permission
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.None)]
        public Guid Id { get; set; }
        [Required]
        [MaxLength(255)]
        public string Key { get; set; } = string.Empty;
        [Required]
        [MaxLength(255)]
        public string DisplayName { get; set; } = string.Empty;
        [MaxLength(2048)]
        public string? Description { get; set; }
        [Required]
        [MaxLength(100)]
        public string Category { get; set; } = string.Empty;
        [Required]
        [DefaultValue(false)]
        public bool IsSystemPermission { get; set; }
        [Required]
        public DateTimeOffset CreatedAt { get; set; }
        public DateTimeOffset? UpdatedAt { get; set; }
        [InverseProperty(nameof(RolePermission.Permission))]
        public ICollection<RolePermission> RolePermissions { get; set; } = new List<RolePermission>();

        [InverseProperty(nameof(UserPermission.Permission))]
        public ICollection<UserPermission> UserPermissions { get; set; } = new List<UserPermission>();
    }
    public sealed class RolePermission
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.None)]
        public Guid Id { get; set; }
        [Required]
        [MaxLength(450)]
        public string RoleId { get; set; } = string.Empty;
        [Required]
        public Guid PermissionId { get; set; }
        [Required]
        public DateTimeOffset GrantedAt { get; set; }
        [MaxLength(450)]
        public string? GrantedBy { get; set; }
        [ForeignKey(nameof(PermissionId))]
        public Permission Permission { get; set; } = null!;
    }
    public sealed class UserPermission
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.None)]
        public Guid Id { get; set; }
        [Required]
        [MaxLength(450)]
        public string UserId { get; set; } = string.Empty;
        [Required]
        public Guid PermissionId { get; set; }
        [Required]
        public bool IsGranted { get; set; }
        [Required]
        public DateTimeOffset GrantedAt { get; set; }
        [MaxLength(450)]
        public string? GrantedBy { get; set; }
        public DateTimeOffset? ExpiresAt { get; set; }
        [ForeignKey(nameof(PermissionId))]
        public Permission Permission { get; set; } = null!;
    }
    public class LoginHistory
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.None)]
        public Guid Id { get; set; }
        [Required]
        [MaxLength(450)]
        public string UserId { get; set; } = string.Empty;
        [Required]
        public DateTimeOffset LoginDate { get; set; }
        [MaxLength(64)]
        public string? IpAddress { get; set; }
        [MaxLength(1024)]
        public string? UserAgent { get; set; }
        [Required]
        public bool Success { get; set; }
        [MaxLength(1024)]
        public string? FailureReason { get; set; }
        [ForeignKey(nameof(UserId))]
        public CustomPCIShieldUser? User { get; set; }
    }
    public sealed class AuthAuditLog
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.None)]
        public Guid Id { get; set; }
        [MaxLength(450)]
        public string? UserId { get; set; }
        [MaxLength(256)]
        public string? UserName { get; set; }
        [Required]
        [MaxLength(100)]
        public string Action { get; set; } = string.Empty;
        [Required]
        [MaxLength(50)]
        public string Category { get; set; } = string.Empty;
        [MaxLength(1024)]
        public string? Description { get; set; }
        [MaxLength(450)]
        public string? TargetEntityId { get; set; }
        [MaxLength(100)]
        public string? TargetEntityType { get; set; }
        public string? OldValues { get; set; }
        public string? NewValues { get; set; }
        [MaxLength(50)]
        public string? IpAddress { get; set; }
        [MaxLength(1024)]
        public string? UserAgent { get; set; }
        [Required]
        public bool Success { get; set; }
        [MaxLength(1024)]
        public string? ErrorMessage { get; set; }
        [Required]
        [MaxLength(20)]
        public string Severity { get; set; } = "Info";
        [Required]
        public DateTimeOffset Timestamp { get; set; }
        public string? Metadata { get; set; }
        [ForeignKey(nameof(UserId))]
        public CustomPCIShieldUser? User { get; set; }
    }

}
