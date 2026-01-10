using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

using BlazorMauiShared.Models.Merchant;

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
    public sealed class GetCatalog : EndpointWithoutRequest<List<PermissionCatalogDto>>
    {
        private readonly AuthorizationDbContext _authContext;

        public GetCatalog(AuthorizationDbContext authContext)
        {
            _authContext = authContext;
        }

        public override void Configure()
        {
            Get("/admin/permissions/catalog");
            AuthSchemes(JwtBearerDefaults.AuthenticationScheme);
            Roles("Administrator");
            Description(d =>
                d.Produces<ListMerchantResponse>(200, "application/json")
                    .Produces(400)
            );
            Tags("Merchants");
            Summary(s =>
            {
                s.Summary = "Get a paged list of Merchants";
                s.Description = "Gets a paginated list of Merchants with total count and pagination metadata";
                s.ExampleRequest = new GetMerchantListRequest
                {
                    PageNumber = 1,
                    PageSize = 10,
                };
            });
        }

        public override async Task HandleAsync(CancellationToken ct)
        {
            var permissions = await _authContext.Permissions
                .Select(p => new PermissionListItemDto
                {
                    Id = p.Id,
                    Key = p.Key,
                    DisplayName = p.DisplayName,
                    Description = p.Description,
                    Category = p.Category,
                    IsSystemPermission = p.IsSystemPermission,
                    CreatedAt = p.CreatedAt,
                    RoleCount = _authContext.RolePermissions.Count(rp => rp.PermissionId == p.Id),
                    UserCount = _authContext.UserPermissions.Count(up => up.PermissionId == p.Id)
                })
                .AsNoTracking()
                .ToListAsync(ct);

            var catalog = permissions
                .GroupBy(p => p.Category)
                .Select(g => new PermissionCatalogDto
                {
                    Category = g.Key,
                    Permissions = g.OrderBy(p => p.DisplayName).ToList()
                })
                .OrderBy(c => c.Category)
                .ToList();

            await SendOkAsync(catalog, ct);
        }
    }
}
