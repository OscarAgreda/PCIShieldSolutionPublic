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

namespace PCIShield.Api.Auth.AuthAdminUi.Features.Admin.Roles
{
    public sealed class List : EndpointWithoutRequest<List<RoleListItemDto>>
    {
        private readonly RoleManager<IdentityRole> _roleManager;
        private readonly AuthorizationDbContext _authContext;

        public List(RoleManager<IdentityRole> roleManager, AuthorizationDbContext authContext)
        {
            _roleManager = roleManager;
            _authContext = authContext;
        }

        public override void Configure()
        {
            Get("/admin/roles");
            AuthSchemes(JwtBearerDefaults.AuthenticationScheme);
            Roles("Administrator");
            Options(x => x.WithTags("Admin - Roles"));
            Summary(s =>
            {
                s.Summary = "Get all system roles";
                s.Description = "Retrieves all roles with user count and system role flags.";
            });
        }

        public override async Task HandleAsync(CancellationToken ct)
        {
            var systemRoles = new HashSet<string> { "Administrator", "User", "Customer", "Employee" };

            var rolesQuery = from role in _authContext.Roles
                             select new RoleListItemDto
                             {
                                 Id = role.Id,
                                 Name = role.Name ?? string.Empty,
                                 IsSystemRole = systemRoles.Contains(role.Name ?? string.Empty),
                                 UserCount = _authContext.UserRoles.Count(ur => ur.RoleId == role.Id),
                                 CreatedDate = DateTimeOffset.UtcNow
                             };

            var roles = await rolesQuery
                .OrderBy(r => r.Name)
                .AsNoTracking()
                .ToListAsync(ct);

            await SendOkAsync(roles, ct);
        }
    }
    public sealed class GetRoleRequest
    {
        public string Id { get; set; } = string.Empty;
    }
    public sealed class Get : Endpoint<GetRoleRequest, RoleDetailsDto>
    {
        private readonly RoleManager<IdentityRole> _roleManager;
        private readonly UserManager<CustomPCIShieldUser> _userManager;
        private readonly AuthorizationDbContext _authContext;

        public Get(
            RoleManager<IdentityRole> roleManager,
            UserManager<CustomPCIShieldUser> userManager,
            AuthorizationDbContext authContext)
        {
            _roleManager = roleManager;
            _userManager = userManager;
            _authContext = authContext;
        }

        public override void Configure()
        {
            Get("/admin/roles/{Id}");
            AuthSchemes(JwtBearerDefaults.AuthenticationScheme);
            Roles("Administrator");
            Options(x => x.WithTags("Admin - Roles"));
            Summary(s =>
            {
                s.Summary = "Get role by ID";
                s.Description = "Retrieves detailed information about a specific role, including users assigned.";
            });
        }

        public override async Task HandleAsync(GetRoleRequest req, CancellationToken ct)
        {
            var role = await _roleManager.FindByIdAsync(req.Id);
            if (role == null)
            {
                await SendNotFoundAsync(ct);
                return;
            }

            var systemRoles = new HashSet<string> { "Administrator", "User", "Customer", "Employee" };
            var usersInRole = await (
                from userRole in _authContext.UserRoles
                join user in _authContext.Users on userRole.UserId equals user.Id
                where userRole.RoleId == role.Id
                select new UserRoleAssignmentDto
                {
                    UserId = user.Id,
                    UserName = user.UserName ?? string.Empty,
                    Email = user.Email ?? string.Empty,
                    AssignedDate = user.CreatedDate
                })
                .AsNoTracking()
                .ToListAsync(ct);

            var response = new RoleDetailsDto
            {
                Id = role.Id,
                Name = role.Name ?? string.Empty,
                IsSystemRole = systemRoles.Contains(role.Name ?? string.Empty),
                Users = usersInRole,
                CreatedDate = DateTimeOffset.UtcNow
            };

            await SendOkAsync(response, ct);
        }
    }
    public sealed class Create : Endpoint<CreateRoleRequest, AdminOperationResult>
    {
        private readonly RoleManager<IdentityRole> _roleManager;
        private readonly IAppLoggerService<Create> _logger;

        public Create(RoleManager<IdentityRole> roleManager, IAppLoggerService<Create> logger)
        {
            _roleManager = roleManager;
            _logger = logger;
        }

        public override void Configure()
        {
            Post("/admin/roles");
            AuthSchemes(JwtBearerDefaults.AuthenticationScheme);
            Roles("Administrator");
            Options(x => x.WithTags("Admin - Roles"));
            Summary(s =>
            {
                s.Summary = "Create a role";
                s.Description = "Creates a new role if it does not already exist.";
            });
        }

        public override async Task HandleAsync(CreateRoleRequest req, CancellationToken ct)
        {
            _logger.LogInformation("Creating new role: {RoleName}", req.Name);
            var existingRole = await _roleManager.FindByNameAsync(req.Name);
            if (existingRole != null)
            {
                await SendAsync(new AdminOperationResult
                {
                    Success = false,
                    Message = "Role already exists",
                    Errors = new() { $"A role with the name '{req.Name}' already exists." }
                }, 400, ct);
                return;
            }

            var newRole = new IdentityRole
            {
                Name = req.Name,
                NormalizedName = req.Name.ToUpperInvariant()
            };

            var result = await _roleManager.CreateAsync(newRole);

            if (!result.Succeeded)
            {
                _logger.LogWarning("Failed to create role {RoleName}: {Errors}",
                    req.Name,
                    string.Join(", ", result.Errors.Select(e => e.Description)));

                await SendAsync(new AdminOperationResult
                {
                    Success = false,
                    Message = "Failed to create role",
                    Errors = result.Errors.Select(e => e.Description).ToList()
                }, 400, ct);
                return;
            }

            _logger.LogInformation("Successfully created role {RoleName}", req.Name);

            await SendOkAsync(new AdminOperationResult
            {
                Success = true,
                Message = $"Role '{req.Name}' created successfully"
            }, ct);
        }
    }
    public sealed class Update : Endpoint<UpdateRoleRequest, AdminOperationResult>
    {
        private readonly RoleManager<IdentityRole> _roleManager;
        private readonly IAppLoggerService<Update> _logger;

        public Update(RoleManager<IdentityRole> roleManager, IAppLoggerService<Update> logger)
        {
            _roleManager = roleManager;
            _logger = logger;
        }

        public override void Configure()
        {
            Put("/admin/roles");
            AuthSchemes(JwtBearerDefaults.AuthenticationScheme);
            Roles("Administrator");
            Options(x => x.WithTags("Admin - Roles"));
            Summary(s =>
            {
                s.Summary = "Update a role";
                s.Description = "Updates the name of an existing role; system roles cannot be modified.";
            });
        }

        public override async Task HandleAsync(UpdateRoleRequest req, CancellationToken ct)
        {
            var role = await _roleManager.FindByIdAsync(req.Id);
            if (role == null)
            {
                await SendNotFoundAsync(ct);
                return;
            }
            var systemRoles = new HashSet<string> { "Administrator", "User", "Customer", "Employee" };
            if (systemRoles.Contains(role.Name ?? string.Empty))
            {
                await SendAsync(new AdminOperationResult
                {
                    Success = false,
                    Message = "Cannot modify system roles",
                    Errors = new() { "System roles (Administrator, User, Customer, Employee) cannot be modified." }
                }, 400, ct);
                return;
            }

            _logger.LogInformation("Updating role {RoleId}: {OldName} -> {NewName}",
                role.Id, role.Name, req.Name);

            role.Name = req.Name;
            role.NormalizedName = req.Name.ToUpperInvariant();

            var result = await _roleManager.UpdateAsync(role);

            if (!result.Succeeded)
            {
                await SendAsync(new AdminOperationResult
                {
                    Success = false,
                    Message = "Failed to update role",
                    Errors = result.Errors.Select(e => e.Description).ToList()
                }, 400, ct);
                return;
            }

            _logger.LogInformation("Successfully updated role {RoleId}", role.Id);

            await SendOkAsync(new AdminOperationResult
            {
                Success = true,
                Message = "Role updated successfully"
            }, ct);
        }
    }
    public sealed class Delete : Endpoint<DeleteRoleRequest, AdminOperationResult>
    {
        private readonly RoleManager<IdentityRole> _roleManager;
        private readonly UserManager<CustomPCIShieldUser> _userManager;
        private readonly IAppLoggerService<Delete> _logger;

        public Delete(
            RoleManager<IdentityRole> roleManager,
            UserManager<CustomPCIShieldUser> userManager,
            IAppLoggerService<Delete> logger)
        {
            _roleManager = roleManager;
            _userManager = userManager;
            _logger = logger;
        }

        public override void Configure()
        {
            Delete("/admin/roles");
            AuthSchemes(JwtBearerDefaults.AuthenticationScheme);
            Roles("Administrator");
            Options(x => x.WithTags("Admin - Roles"));
            Summary(s =>
            {
                s.Summary = "Delete a role";
                s.Description = "Deletes a role if it is not a system role and has no assigned users.";
            });
        }

        public override async Task HandleAsync(DeleteRoleRequest req, CancellationToken ct)
        {
            var role = await _roleManager.FindByIdAsync(req.RoleId);
            if (role == null)
            {
                await SendNotFoundAsync(ct);
                return;
            }
            var systemRoles = new HashSet<string> { "Administrator", "User", "Customer", "Employee" };
            if (systemRoles.Contains(role.Name ?? string.Empty))
            {
                await SendAsync(new AdminOperationResult
                {
                    Success = false,
                    Message = "Cannot delete system roles",
                    Errors = new() { "System roles (Administrator, User, Customer, Employee) cannot be deleted." }
                }, 400, ct);
                return;
            }
            var usersInRole = await _userManager.GetUsersInRoleAsync(role.Name ?? string.Empty);
            if (usersInRole.Any())
            {
                await SendAsync(new AdminOperationResult
                {
                    Success = false,
                    Message = "Cannot delete role with assigned users",
                    Errors = new() { $"This role is assigned to {usersInRole.Count} user(s). Remove all users from this role before deleting." }
                }, 400, ct);
                return;
            }

            _logger.LogWarning("Deleting role {RoleId}: {RoleName}", role.Id, role.Name);

            var result = await _roleManager.DeleteAsync(role);

            if (!result.Succeeded)
            {
                await SendAsync(new AdminOperationResult
                {
                    Success = false,
                    Message = "Failed to delete role",
                    Errors = result.Errors.Select(e => e.Description).ToList()
                }, 400, ct);
                return;
            }

            _logger.LogInformation("Successfully deleted role {RoleId}", role.Id);

            await SendOkAsync(new AdminOperationResult
            {
                Success = true,
                Message = "Role deleted successfully"
            }, ct);
        }
    }
}
