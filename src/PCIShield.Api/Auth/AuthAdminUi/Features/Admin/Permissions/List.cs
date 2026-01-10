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
    public sealed class List : EndpointWithoutRequest<System.Collections.Generic.List<PermissionListItemDto>>
    {
        private readonly AuthorizationDbContext _authContext;

        public List(AuthorizationDbContext authContext)
        {
            _authContext = authContext;
        }

        public override void Configure()
        {
            Get("/admin/permissions");
            AuthSchemes(JwtBearerDefaults.AuthenticationScheme);
            Roles("Administrator");
            Options(x => x.WithTags("Admin - Permissions"));
            Summary(s =>
            {
                s.Summary = "Get all system permissions";
                s.Description = "Retrieves complete permission catalog with assignment statistics.";
            });
        }

        public override async Task HandleAsync(CancellationToken ct)
        {
            var permissionsQuery =
                from permission in _authContext.Permissions
                select new PermissionListItemDto
                {
                    Id = permission.Id,
                    Key = permission.Key,
                    DisplayName = permission.DisplayName,
                    Description = permission.Description,
                    Category = permission.Category,
                    IsSystemPermission = permission.IsSystemPermission,
                    CreatedAt = permission.CreatedAt,
                    RoleCount = _authContext.RolePermissions.Count(rp => rp.PermissionId == permission.Id),
                    UserCount = _authContext.UserPermissions.Count(up => up.PermissionId == permission.Id)
                };

            var permissions = await permissionsQuery
                .OrderBy(p => p.Category)
                .ThenBy(p => p.DisplayName)
                .AsNoTracking()
                .ToListAsync(ct);

            await SendOkAsync(permissions, ct);
        }
    }
}
