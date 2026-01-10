using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PCIShield.BlazorMauiShared.Auth
{

    #region User Management DTOs
    public sealed class UserListItemDto
    {
        public string Id { get; set; } = string.Empty;
        public string UserName { get; set; } = string.Empty;
        public string Email { get; set; } = string.Empty;
        public string? FirstName { get; set; }
        public string? LastName { get; set; }
        public string FullName => $"{FirstName} {LastName}".Trim();
        public bool EmailConfirmed { get; set; }
        public bool PhoneNumberConfirmed { get; set; }
        public bool TwoFactorEnabled { get; set; }
        public bool LockoutEnabled { get; set; }
        public DateTimeOffset? LockoutEnd { get; set; }
        public bool IsLockedOut => LockoutEnd.HasValue && LockoutEnd.Value > DateTimeOffset.UtcNow;
        public int AccessFailedCount { get; set; }
        public DateTimeOffset? LastLoginDate { get; set; }
        public List<string> Roles { get; set; } = new();
        public int ExternalLoginsCount { get; set; }
        public DateTimeOffset CreatedDate { get; set; }
    }
    public sealed class UserDetailsDto
    {
        public string Id { get; set; } = string.Empty;
        public string UserName { get; set; } = string.Empty;
        public string Email { get; set; } = string.Empty;
        public string? FirstName { get; set; }
        public string? LastName { get; set; }
        public string? PhoneNumber { get; set; }
        public bool EmailConfirmed { get; set; }

        public bool IsLockedOut { get; set; }

        public bool PhoneNumberConfirmed { get; set; }
        public bool TwoFactorEnabled { get; set; }
        public bool LockoutEnabled { get; set; }
        public DateTimeOffset? LockoutEnd { get; set; }
        public int AccessFailedCount { get; set; }
        public string? SecurityStamp { get; set; }
        public DateTimeOffset? LastLoginDate { get; set; }
        public DateTimeOffset CreatedDate { get; set; }
        public List<string> Roles { get; set; } = new();
        public List<string> AllAvailableRoles { get; set; } = new();
        public List<ExternalLoginDto> ExternalLogins { get; set; } = new();
        public List<UserClaimDto> Claims { get; set; } = new();
        public List<LoginHistoryDto> RecentLogins { get; set; } = new();
    }
    public sealed class CreateUserRequest
    {
        [Required(ErrorMessage = "Username is required")]
        [StringLength(256, ErrorMessage = "Username cannot exceed 256 characters")]
        public string UserName { get; set; } = string.Empty;

        [Required(ErrorMessage = "Email is required")]
        [EmailAddress(ErrorMessage = "Invalid email format")]
        public string Email { get; set; } = string.Empty;

        [Required(ErrorMessage = "Password is required")]
        [StringLength(100, MinimumLength = 6, ErrorMessage = "Password must be at least 6 characters")]
        public string Password { get; set; } = string.Empty;

        [Compare(nameof(Password), ErrorMessage = "Passwords do not match")]
        public string ConfirmPassword { get; set; } = string.Empty;

        public string? FirstName { get; set; }
        public string? LastName { get; set; }
        public string? PhoneNumber { get; set; }
        public bool EmailConfirmed { get; set; }
        public bool SendWelcomeEmail { get; set; } = true;
        public List<string> Roles { get; set; } = new();
    }
    public sealed class UpdateUserRequest
    {
        [Required]
        public string Id { get; set; } = string.Empty;

        [Required]
        [EmailAddress]
        public string Email { get; set; } = string.Empty;

        public string? FirstName { get; set; }
        public string? LastName { get; set; }
        public string? PhoneNumber { get; set; }
        public bool EmailConfirmed { get; set; }
        public bool PhoneNumberConfirmed { get; set; }
        public bool TwoFactorEnabled { get; set; }
        public List<string> Roles { get; set; } = new();
    }
    public sealed class ChangeUserPasswordRequest
    {
        [Required]
        public string UserId { get; set; } = string.Empty;

        [Required]
        [StringLength(100, MinimumLength = 6)]
        public string NewPassword { get; set; } = string.Empty;

        [Compare(nameof(NewPassword))]
        public string ConfirmPassword { get; set; } = string.Empty;

        public bool RequirePasswordChange { get; set; }
    }
    public sealed class LockUserRequest
    {
        [Required]
        public string UserId { get; set; } = string.Empty;

        public bool Lock { get; set; }
        public DateTimeOffset? LockoutEnd { get; set; }
        public string? Reason { get; set; }
    }
    public sealed class RefreshSecurityStampRequest
    {
        [Required]
        public string UserId { get; set; } = string.Empty;

        public string? Reason { get; set; }
    }
    public sealed class DeleteUserRequest
    {
        [Required]
        public string UserId { get; set; } = string.Empty;

        public bool DeleteApplicationUser { get; set; } = true;
        public string? Reason { get; set; }
    }

    #endregion

    #region Role Management DTOs
    public sealed class RoleListItemDto
    {
        public string Id { get; set; } = string.Empty;
        public string Name { get; set; } = string.Empty;
        public string? Description { get; set; }
        public int UserCount { get; set; }
        public bool IsSystemRole { get; set; }
        public DateTimeOffset CreatedDate { get; set; }
    }
    public sealed class RoleDetailsDto
    {
        public string Id { get; set; } = string.Empty;
        public string Name { get; set; } = string.Empty;
        public string? Description { get; set; }
        public bool IsSystemRole { get; set; }
        public List<string> Permissions { get; set; } = new();
        public List<UserRoleAssignmentDto> Users { get; set; } = new();
        public DateTimeOffset CreatedDate { get; set; }
    }
    public sealed class CreateRoleRequest
    {
        [Required]
        [StringLength(256)]
        public string Name { get; set; } = string.Empty;

        public string? Description { get; set; }
        public List<string> Permissions { get; set; } = new();
    }
    public sealed class UpdateRoleRequest
    {
        [Required]
        public string Id { get; set; } = string.Empty;

        [Required]
        [StringLength(256)]
        public string Name { get; set; } = string.Empty;

        public string? Description { get; set; }
        public List<string> Permissions { get; set; } = new();
    }
    public sealed class DeleteRoleRequest
    {
        [Required]
        public string RoleId { get; set; } = string.Empty;
    }

    #endregion

    #region Claims Management DTOs
    public sealed class UserClaimDto
    {
        public int Id { get; set; }
        public string ClaimType { get; set; } = string.Empty;
        public string ClaimValue { get; set; } = string.Empty;
    }
    public sealed class AddUserClaimRequest
    {
        [Required]
        public string UserId { get; set; } = string.Empty;

        [Required]
        public string ClaimType { get; set; } = string.Empty;

        [Required]
        public string ClaimValue { get; set; } = string.Empty;
    }
    public sealed class RemoveUserClaimRequest
    {
        [Required]
        public string UserId { get; set; } = string.Empty;

        [Required]
        public string ClaimType { get; set; } = string.Empty;

        [Required]
        public string ClaimValue { get; set; } = string.Empty;
    }

    #endregion

    #region External Logins DTOs
    public sealed class ExternalLoginDto
    {
        public string LoginProvider { get; set; } = string.Empty;
        public string ProviderKey { get; set; } = string.Empty;
        public string? ProviderDisplayName { get; set; }
        public DateTimeOffset? LinkedDate { get; set; }
    }
    public sealed class RemoveExternalLoginRequest
    {
        [Required]
        public string UserId { get; set; } = string.Empty;

        [Required]
        public string LoginProvider { get; set; } = string.Empty;

        [Required]
        public string ProviderKey { get; set; } = string.Empty;
    }

    #endregion

    #region Audit & History DTOs
    public sealed class LoginHistoryDto
    {
        public Guid Id { get; set; }
        public string UserId { get; set; } = string.Empty;
        public DateTimeOffset LoginDate { get; set; }
        public string? IpAddress { get; set; }
        public string? UserAgent { get; set; }
        public bool Success { get; set; }
        public string? FailureReason { get; set; }
    }
    public sealed class SecurityAuditDto
    {
        public Guid Id { get; set; }
        public string UserId { get; set; } = string.Empty;
        public string UserName { get; set; } = string.Empty;
        public string EventType { get; set; } = string.Empty;
        public string? Description { get; set; }
        public DateTimeOffset EventDate { get; set; }
        public string? IpAddress { get; set; }
        public string? PerformedBy { get; set; }
    }

    #endregion

    #region Supporting DTOs
    public sealed class UserRoleAssignmentDto
    {
        public string UserId { get; set; } = string.Empty;
        public string UserName { get; set; } = string.Empty;
        public string Email { get; set; } = string.Empty;
        public DateTimeOffset AssignedDate { get; set; }
    }
    public sealed class AdminDashboardStatsDto
    {
        public int TotalUsers { get; set; }
        public int ActiveUsers { get; set; }
        public int LockedUsers { get; set; }
        public int UnconfirmedEmails { get; set; }
        public int UsersWithExternalLogins { get; set; }
        public int TotalRoles { get; set; }
        public List<LoginHistoryDto> RecentLogins { get; set; } = new();
        public Dictionary<string, int> UsersByRole { get; set; } = new();
    }
    public sealed class PagedResult<T>
    {
        public List<T> Items { get; set; } = new();
        public int TotalCount { get; set; }
        public int PageNumber { get; set; }
        public int PageSize { get; set; }
        public int TotalPages => (int)Math.Ceiling((double)TotalCount / PageSize);
        public bool HasPreviousPage => PageNumber > 1;
        public bool HasNextPage => PageNumber < TotalPages;
    }
    public sealed class AdminOperationResult
    {
        public bool Success { get; set; }
        public string? Message { get; set; }
        public List<string> Errors { get; set; } = new();
    }

    #endregion

    #region Permission Management DTOs
    public sealed class PermissionListItemDto
    {
        public Guid Id { get; set; }
        public string Key { get; set; } = string.Empty;
        public string DisplayName { get; set; } = string.Empty;
        public string? Description { get; set; }
        public string Category { get; set; } = string.Empty;
        public bool IsSystemPermission { get; set; }
        public int RoleCount { get; set; }
        public int UserCount { get; set; }
        public DateTimeOffset CreatedAt { get; set; }
    }
    public sealed class PermissionDetailsDto
    {
        public Guid Id { get; set; }
        public string Key { get; set; } = string.Empty;
        public string DisplayName { get; set; } = string.Empty;
        public string? Description { get; set; }
        public string Category { get; set; } = string.Empty;
        public bool IsSystemPermission { get; set; }
        public DateTimeOffset CreatedAt { get; set; }
        public DateTimeOffset? UpdatedAt { get; set; }
        public List<RolePermissionAssignmentDto> AssignedRoles { get; set; } = new();
        public List<UserPermissionAssignmentDto> DirectUserGrants { get; set; } = new();
    }
    public sealed class RolePermissionAssignmentDto
    {
        public string RoleId { get; set; } = string.Empty;
        public string RoleName { get; set; } = string.Empty;
        public DateTimeOffset GrantedAt { get; set; }
        public string? GrantedBy { get; set; }
    }
    public sealed class UserPermissionAssignmentDto
    {
        public string UserId { get; set; } = string.Empty;
        public string UserName { get; set; } = string.Empty;
        public bool IsGranted { get; set; }
        public DateTimeOffset GrantedAt { get; set; }
        public DateTimeOffset? ExpiresAt { get; set; }
    }
    public sealed class CreatePermissionRequest
    {
        [Required]
        [StringLength(200)]
        public string Key { get; set; } = string.Empty;

        [Required]
        [StringLength(200)]
        public string DisplayName { get; set; } = string.Empty;

        [StringLength(500)]
        public string? Description { get; set; }

        [Required]
        [StringLength(100)]
        public string Category { get; set; } = string.Empty;
    }
    public sealed class UpdatePermissionRequest
    {
        [Required]
        public Guid Id { get; set; }

        [Required]
        [StringLength(200)]
        public string DisplayName { get; set; } = string.Empty;

        [StringLength(500)]
        public string? Description { get; set; }

        [Required]
        [StringLength(100)]
        public string Category { get; set; } = string.Empty;
    }
    public sealed class DeletePermissionRequest
    {
        [Required]
        public Guid PermissionId { get; set; }
    }
    public sealed class AssignRolePermissionsRequest
    {
        [Required]
        public string RoleId { get; set; } = string.Empty;

        [Required]
        public List<Guid> PermissionIds { get; set; } = new();

        public string? GrantedBy { get; set; }
    }
    public sealed class GrantUserPermissionRequest
    {
        [Required]
        public string UserId { get; set; } = string.Empty;

        [Required]
        public Guid PermissionId { get; set; }

        public bool IsGranted { get; set; } = true;

        public DateTimeOffset? ExpiresAt { get; set; }

        public string? GrantedBy { get; set; }
    }
    public sealed class UserEffectivePermissionsDto
    {
        public string UserId { get; set; } = string.Empty;
        public string UserName { get; set; } = string.Empty;
        public List<string> EffectivePermissions { get; set; } = new();
        public List<PermissionSourceDto> PermissionSources { get; set; } = new();
    }
    public sealed class PermissionSourceDto
    {
        public string PermissionKey { get; set; } = string.Empty;
        public string Source { get; set; } = string.Empty;
        public bool IsGranted { get; set; } = true;
    }
    public sealed class PermissionCatalogDto
    {
        public string Category { get; set; } = string.Empty;
        public List<PermissionListItemDto> Permissions { get; set; } = new();
    }

    #endregion

}
