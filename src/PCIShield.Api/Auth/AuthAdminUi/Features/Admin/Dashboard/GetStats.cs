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

using PCIShield.Api.Auth.AuthAdminUi.Features.Admin.Users;
using PCIShield.BlazorMauiShared.Auth;
using PCIShield.Domain.Entities;
using PCIShield.Infrastructure.Data;
using PCIShield.Infrastructure.Services;

using PCIShieldLib.SharedKernel.Interfaces;

namespace PCIShield.Api.Auth.AuthAdminUi.Features.Admin.Dashboard
{
    public sealed class GetStats : EndpointWithoutRequest<AdminDashboardStatsDto>
    {
        private readonly AuthorizationDbContext _authContext;
        private readonly UserManager<CustomPCIShieldUser> _userManager;

        public GetStats(AuthorizationDbContext authContext, UserManager<CustomPCIShieldUser> userManager)
        {
            _authContext = authContext;
            _userManager = userManager;
        }

        public override void Configure()
        {
            Get("/admin/dashboard/stats");
            AuthSchemes(JwtBearerDefaults.AuthenticationScheme);
            Roles("Administrator"); 
            Options(x => x.WithTags("Admin - Dashboard"));
            Summary(s =>
            {
                s.Summary = "Get admin dashboard statistics";
                s.Description = "Retrieves comprehensive system statistics including user counts, login history, and role distribution.";
            });
        }

        public override async Task HandleAsync(CancellationToken ct)
        {
            var now = DateTimeOffset.UtcNow;
            var totalUsers = await _authContext.Users.CountAsync(ct);
            var activeUsers = await _authContext.Users
                .Where(u => u.LastLogin.HasValue && u.LastLogin.Value > now.AddDays(-30))
                .CountAsync(ct);
            var lockedUsers = await _authContext.Users
                .Where(u => u.LockoutEnd.HasValue && u.LockoutEnd.Value > now)
                .CountAsync(ct);
            var unconfirmedEmails = await _authContext.Users
                .Where(u => !u.EmailConfirmed)
                .CountAsync(ct);
            var usersWithExternalLogins = await _authContext.UserLogins
                .Select(ul => ul.UserId)
                .Distinct()
                .CountAsync(ct);
            var totalRoles = await _authContext.Roles.CountAsync(ct);
            var recentLogins = new List<LoginHistoryDto>();
            try
            {
                recentLogins = await _authContext.Set<LoginHistory>()
                    .OrderByDescending(lh => lh.LoginDate)
                    .Take(10)
                    .Select(lh => new LoginHistoryDto
                    {
                        Id = lh.Id,
                        UserId = lh.UserId,
                        LoginDate = lh.LoginDate,
                        IpAddress = lh.IpAddress,
                        UserAgent = lh.UserAgent,
                        Success = lh.Success,
                        FailureReason = lh.FailureReason
                    })
                    .AsNoTracking()
                    .ToListAsync(ct);
            }
            catch
            {
            }
            var usersByRole = await (
                from userRole in _authContext.UserRoles
                join role in _authContext.Roles on userRole.RoleId equals role.Id
                group userRole by role.Name into roleGroup
                select new { Role = roleGroup.Key ?? "Unknown", Count = roleGroup.Count() }
            )
            .AsNoTracking()
            .ToDictionaryAsync(x => x.Role, x => x.Count, ct);

            var stats = new AdminDashboardStatsDto
            {
                TotalUsers = totalUsers,
                ActiveUsers = activeUsers,
                LockedUsers = lockedUsers,
                UnconfirmedEmails = unconfirmedEmails,
                UsersWithExternalLogins = usersWithExternalLogins,
                TotalRoles = totalRoles,
                RecentLogins = recentLogins,
                UsersByRole = usersByRole
            };

            await SendOkAsync(stats, ct);
        }
    }
}
