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
    public sealed class GetUserEffectivePermissionsRequest
    {
        public string UserId { get; set; } = string.Empty;
    }
    public sealed class GetUserEffectivePermissions : Endpoint<GetUserEffectivePermissionsRequest, UserEffectivePermissionsDto>
    {
        private readonly AuthorizationDbContext _authContext;
        private readonly UserManager<CustomPCIShieldUser> _userManager;

        public GetUserEffectivePermissions(
            AuthorizationDbContext authContext,
            UserManager<CustomPCIShieldUser> userManager)
        {
            _authContext = authContext;
            _userManager = userManager;
        }

        public override void Configure()
        {
            Get("/admin/permissions/user/{UserId}");
            AuthSchemes(JwtBearerDefaults.AuthenticationScheme);
            Roles("Administrator");
            Options(x => x.WithTags("Admin - Permissions"));
            Summary(s =>
            {
                s.Summary = "Get a user's effective permissions";
                s.Description = "Calculates effective permissions by combining role-based grants with direct user grants/denials (with expiry).";
            });
        }

        public override async Task HandleAsync(GetUserEffectivePermissionsRequest req, CancellationToken ct)
        {
            var user = await _userManager.FindByIdAsync(req.UserId);
            if (user == null)
            {
                await SendNotFoundAsync(ct);
                return;
            }

            var userRoles = await _userManager.GetRolesAsync(user);
            var permissionSources = new List<PermissionSourceDto>();
            var effectivePermissions = new HashSet<string>();
            var rolePermissions = await (
                from role in _authContext.Roles
                join rolePermission in _authContext.RolePermissions on role.Id equals rolePermission.RoleId
                join permission in _authContext.Permissions on rolePermission.PermissionId equals permission.Id
                where userRoles.Contains(role.Name ?? string.Empty)
                select new { RoleName = role.Name, Permission = permission }
            )
            .AsNoTracking()
            .ToListAsync(ct);

            foreach (var rp in rolePermissions)
            {
                effectivePermissions.Add(rp.Permission.Key);
                permissionSources.Add(new PermissionSourceDto
                {
                    PermissionKey = rp.Permission.Key,
                    Source = $"Role: {rp.RoleName}",
                    IsGranted = true
                });
            }
            var userPermissions = await (
                from userPermission in _authContext.UserPermissions
                join permission in _authContext.Permissions on userPermission.PermissionId equals permission.Id
                where userPermission.UserId == req.UserId
                      && (!userPermission.ExpiresAt.HasValue || userPermission.ExpiresAt > DateTimeOffset.UtcNow)
                select new { UserPermission = userPermission, Permission = permission }
            )
            .AsNoTracking()
            .ToListAsync(ct);

            foreach (var up in userPermissions)
            {
                if (up.UserPermission.IsGranted)
                    effectivePermissions.Add(up.Permission.Key);
                else
                    effectivePermissions.Remove(up.Permission.Key);

                permissionSources.Add(new PermissionSourceDto
                {
                    PermissionKey = up.Permission.Key,
                    Source = "Direct Grant",
                    IsGranted = up.UserPermission.IsGranted
                });
            }

            var response = new UserEffectivePermissionsDto
            {
                UserId = req.UserId,
                UserName = user.UserName ?? string.Empty,
                EffectivePermissions = effectivePermissions.OrderBy(p => p).ToList(),
                PermissionSources = permissionSources
            };

            await SendOkAsync(response, ct);
        }
    }
}
