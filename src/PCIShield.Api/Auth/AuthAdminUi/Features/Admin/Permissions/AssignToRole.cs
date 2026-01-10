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
    public sealed class AssignToRole : Endpoint<AssignRolePermissionsRequest, AdminOperationResult>
    {
        private readonly AuthorizationDbContext _authContext;
        private readonly IAppLoggerService<AssignToRole> _logger;

        public AssignToRole(AuthorizationDbContext authContext, IAppLoggerService<AssignToRole> logger)
        {
            _authContext = authContext;
            _logger = logger;
        }

        public override void Configure()
        {
            Post("/admin/permissions/assign-to-role");
            AuthSchemes(JwtBearerDefaults.AuthenticationScheme);
            Roles("Administrator");
            Options(x => x.WithTags("Admin - Permissions"));
            Summary(s =>
            {
                s.Summary = "Assign permissions to a role";
                s.Description = "Replaces a role's existing permissions with the provided set.";
            });
        }

        public override async Task HandleAsync(AssignRolePermissionsRequest req, CancellationToken ct)
        {
            _logger.LogInformation("Assigning {Count} permissions to role {RoleId}",
                req.PermissionIds.Count, req.RoleId);
            var existingPermissions = await _authContext.RolePermissions
                .Where(rp => rp.RoleId == req.RoleId)
                .ToListAsync(ct);

            _authContext.RolePermissions.RemoveRange(existingPermissions);
            var newPermissions = req.PermissionIds.Select(permissionId => new RolePermission
            {
                Id = Guid.NewGuid(),
                RoleId = req.RoleId,
                PermissionId = permissionId,
                GrantedAt = DateTimeOffset.UtcNow,
                GrantedBy = req.GrantedBy ?? "System"
            });

            await _authContext.RolePermissions.AddRangeAsync(newPermissions, ct);
            await _authContext.SaveChangesAsync(ct);

            _logger.LogInformation("Successfully assigned {Count} permissions to role {RoleId}",
                req.PermissionIds.Count, req.RoleId);

            await SendOkAsync(new AdminOperationResult
            {
                Success = true,
                Message = $"Assigned {req.PermissionIds.Count} permissions to role"
            }, ct);
        }
    }
}
