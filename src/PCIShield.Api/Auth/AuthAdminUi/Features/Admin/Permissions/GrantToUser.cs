using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

using FastEndpoints;

using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

using PCIShield.BlazorMauiShared.Auth;
using PCIShield.Domain.Entities;
using PCIShield.Infrastructure.Data;
using PCIShield.Infrastructure.Services;

using PCIShieldLib.SharedKernel.Interfaces;

namespace PCIShield.Api.Auth.AuthAdminUi.Features.Admin.Permissions
{
    public sealed class GrantToUser : Endpoint<GrantUserPermissionRequest, AdminOperationResult>
    {
        private readonly AuthorizationDbContext _authContext;
        private readonly IAppLoggerService<GrantToUser> _logger;

        public GrantToUser(AuthorizationDbContext authContext, IAppLoggerService<GrantToUser> logger)
        {
            _authContext = authContext;
            _logger = logger;
        }

        public override void Configure()
        {
            Post("/admin/permissions/grant-to-user");
            AuthSchemes(JwtBearerDefaults.AuthenticationScheme);
            Roles("Administrator");
            Options(x => x.WithTags("Admin - Permissions"));
            Summary(s =>
            {
                s.Summary = "Grant or revoke a permission for a user";
                s.Description = "Grants or revokes a direct user permission. Direct permissions override role-based permissions.";
            });
        }

        public override async Task HandleAsync(GrantUserPermissionRequest req, CancellationToken ct)
        {
            _logger.LogInformation("{Action} permission {PermissionId} to user {UserId}",
                req.IsGranted ? "Granting" : "Revoking", req.PermissionId, req.UserId);

            var existingPermission = await _authContext.UserPermissions
                .FirstOrDefaultAsync(up => up.UserId == req.UserId && up.PermissionId == req.PermissionId, ct);

            if (existingPermission != null)
            {
                existingPermission.IsGranted = req.IsGranted;
                existingPermission.GrantedAt = DateTimeOffset.UtcNow;
                existingPermission.GrantedBy = req.GrantedBy ?? "System";
                existingPermission.ExpiresAt = req.ExpiresAt;
            }
            else
            {
                var newPermission = new UserPermission
                {
                    Id = Guid.NewGuid(),
                    UserId = req.UserId,
                    PermissionId = req.PermissionId,
                    IsGranted = req.IsGranted,
                    GrantedAt = DateTimeOffset.UtcNow,
                    GrantedBy = req.GrantedBy ?? "System",
                    ExpiresAt = req.ExpiresAt
                };

                await _authContext.UserPermissions.AddAsync(newPermission, ct);
            }

            await _authContext.SaveChangesAsync(ct);

            _logger.LogInformation("Successfully {Action} permission to user {UserId}",
                req.IsGranted ? "granted" : "revoked", req.UserId);

            await SendOkAsync(new AdminOperationResult
            {
                Success = true,
                Message = $"Permission {(req.IsGranted ? "granted" : "revoked")} successfully"
            }, ct);
        }
    }
}
