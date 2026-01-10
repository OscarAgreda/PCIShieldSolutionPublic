using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using PCIShield.Domain.Entities;
using PCIShield.Infrastructure.Data;

namespace PCIShield.Infrastructure.Services.Audit
{
    public interface IAuthAuditService
    {
        Task LogAsync(
            string action,
            string category,
            string? userId = null,
            string? userName = null,
            string? description = null,
            string? targetEntityId = null,
            string? targetEntityType = null,
            string? oldValues = null,
            string? newValues = null,
            string? ipAddress = null,
            string? userAgent = null,
            bool success = true,
            string? errorMessage = null,
            string severity = "Info",
            string? metadata = null);

        Task LogLoginAttemptAsync(string userId, string userName, bool success, string? ipAddress, string? userAgent, string? failureReason = null);
        Task LogUserCreatedAsync(string createdByUserId, string createdByUserName, string newUserId, string newUserName, string? ipAddress = null);
        Task LogUserModifiedAsync(string modifiedByUserId, string targetUserId, string targetUserName, string? oldValues, string? newValues, string? ipAddress = null);
        Task LogUserDeletedAsync(string deletedByUserId, string targetUserId, string targetUserName, string? ipAddress = null);
        Task LogPasswordChangedAsync(string userId, string userName, bool byAdmin, string? ipAddress = null);
        Task LogSecurityStampRefreshedAsync(string targetUserId, string targetUserName, string? performedByUserId, string? ipAddress = null);
        Task LogRoleAssignmentAsync(string userId, string userName, string targetUserId, string roleName, bool granted, string? ipAddress = null);
        Task LogPermissionGrantedAsync(string userId, string userName, string targetUserId, string permissionKey, string? ipAddress = null);
    }
    public sealed class AuthAuditService : IAuthAuditService
    {
        private readonly AuthorizationDbContext _context;

        public AuthAuditService(AuthorizationDbContext context)
        {
            _context = context;
        }

        public async Task LogAsync(
            string action,
            string category,
            string? userId = null,
            string? userName = null,
            string? description = null,
            string? targetEntityId = null,
            string? targetEntityType = null,
            string? oldValues = null,
            string? newValues = null,
            string? ipAddress = null,
            string? userAgent = null,
            bool success = true,
            string? errorMessage = null,
            string severity = "Info",
            string? metadata = null)
        {
            try
            {
                var auditLog = new AuthAuditLog
                {
                    Id = Guid.NewGuid(),
                    UserId = userId,
                    UserName = userName,
                    Action = action,
                    Category = category,
                    Description = description,
                    TargetEntityId = targetEntityId,
                    TargetEntityType = targetEntityType,
                    OldValues = oldValues,
                    NewValues = newValues,
                    IpAddress = ipAddress,
                    UserAgent = userAgent,
                    Success = success,
                    ErrorMessage = errorMessage,
                    Severity = severity,
                    Timestamp = DateTimeOffset.UtcNow,
                    Metadata = metadata
                };

                await _context.AuthAuditLogs.AddAsync(auditLog);
                await _context.SaveChangesAsync();
            }
            catch
            {
            }
        }

        public async Task LogLoginAttemptAsync(
            string userId,
            string userName,
            bool success,
            string? ipAddress,
            string? userAgent,
            string? failureReason = null)
        {
            await LogAsync(
                action: success ? "Login" : "LoginFailed",
                category: "Authentication",
                userId: userId,
                userName: userName,
                description: success ? "User logged in successfully" : $"Login failed: {failureReason}",
                ipAddress: ipAddress,
                userAgent: userAgent,
                success: success,
                errorMessage: failureReason,
                severity: success ? "Info" : "Warning"
            );
        }

        public async Task LogUserCreatedAsync(
            string createdByUserId,
            string createdByUserName,
            string newUserId,
            string newUserName,
            string? ipAddress = null)
        {
            await LogAsync(
                action: "UserCreated",
                category: "UserManagement",
                userId: createdByUserId,
                userName: createdByUserName,
                description: $"Created new user: {newUserName}",
                targetEntityId: newUserId,
                targetEntityType: "User",
                ipAddress: ipAddress,
                severity: "Info"
            );
        }

        public async Task LogUserModifiedAsync(
            string modifiedByUserId,
            string targetUserId,
            string targetUserName,
            string? oldValues,
            string? newValues,
            string? ipAddress = null)
        {
            await LogAsync(
                action: "UserModified",
                category: "UserManagement",
                userId: modifiedByUserId,
                description: $"Modified user: {targetUserName}",
                targetEntityId: targetUserId,
                targetEntityType: "User",
                oldValues: oldValues,
                newValues: newValues,
                ipAddress: ipAddress,
                severity: "Info"
            );
        }

        public async Task LogUserDeletedAsync(
            string deletedByUserId,
            string targetUserId,
            string targetUserName,
            string? ipAddress = null)
        {
            await LogAsync(
                action: "UserDeleted",
                category: "UserManagement",
                userId: deletedByUserId,
                description: $"Deleted user: {targetUserName}",
                targetEntityId: targetUserId,
                targetEntityType: "User",
                ipAddress: ipAddress,
                severity: "Warning"
            );
        }

        public async Task LogPasswordChangedAsync(
            string userId,
            string userName,
            bool byAdmin,
            string? ipAddress = null)
        {
            await LogAsync(
                action: "PasswordChanged",
                category: "Security",
                userId: userId,
                userName: userName,
                description: byAdmin ? $"Password changed by administrator" : "Password changed by user",
                ipAddress: ipAddress,
                severity: "Warning"
            );
        }

        public async Task LogSecurityStampRefreshedAsync(
            string targetUserId,
            string targetUserName,
            string? performedByUserId,
            string? ipAddress = null)
        {
            await LogAsync(
                action: "SecurityStampRefreshed",
                category: "Security",
                userId: performedByUserId,
                description: $"Security stamp refreshed for user: {targetUserName} (forced logout)",
                targetEntityId: targetUserId,
                targetEntityType: "User",
                ipAddress: ipAddress,
                severity: "Warning"
            );
        }

        public async Task LogRoleAssignmentAsync(
            string userId,
            string userName,
            string targetUserId,
            string roleName,
            bool granted,
            string? ipAddress = null)
        {
            await LogAsync(
                action: granted ? "RoleGranted" : "RoleRevoked",
                category: "RoleManagement",
                userId: userId,
                userName: userName,
                description: $"{(granted ? "Granted" : "Revoked")} role '{roleName}' for user",
                targetEntityId: targetUserId,
                targetEntityType: "User",
                newValues: roleName,
                ipAddress: ipAddress,
                severity: "Info"
            );
        }

        public async Task LogPermissionGrantedAsync(
            string userId,
            string userName,
            string targetUserId,
            string permissionKey,
            string? ipAddress = null)
        {
            await LogAsync(
                action: "PermissionGranted",
                category: "PermissionManagement",
                userId: userId,
                userName: userName,
                description: $"Granted permission '{permissionKey}' to user",
                targetEntityId: targetUserId,
                targetEntityType: "User",
                newValues: permissionKey,
                ipAddress: ipAddress,
                severity: "Info"
            );
        }
    }

}
