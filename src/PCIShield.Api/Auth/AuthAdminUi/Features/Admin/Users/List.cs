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

namespace PCIShield.Api.Auth.AuthAdminUi.Features.Admin.Users
{
    public sealed class List : EndpointWithoutRequest<BlazorMauiShared.Auth.PagedResult<UserListItemDto>>
    {
        private readonly AuthorizationDbContext _authContext;
        private readonly UserManager<CustomPCIShieldUser> _userManager;

        public List(AuthorizationDbContext authContext, UserManager<CustomPCIShieldUser> userManager)
        {
            _authContext = authContext;
            _userManager = userManager;
        }

        public override void Configure()
        {
            Get("/admin/users");
            AuthSchemes(JwtBearerDefaults.AuthenticationScheme);
            Roles("Administrator");
            Options(x => x.WithTags("Admin - Users"));
            Summary(s =>
            {
                s.Summary = "Get paginated list of all users";
                s.Description = "Retrieves all system users with their roles, status, and security information. Requires Administrator role.";
                s.Response<BlazorMauiShared.Auth.PagedResult<UserListItemDto>>(200, "Successfully retrieved user list");
                s.Response(401, "Unauthorized - Administrator role required");
            });
        }

        public override async Task HandleAsync(CancellationToken ct)
        {
            var usersQuery = from user in _authContext.Users
                             select new UserListItemDto
                             {
                                 Id = user.Id,
                                 UserName = user.UserName ?? string.Empty,
                                 Email = user.Email ?? string.Empty,
                                 FirstName = user.FirstName,
                                 LastName = user.LastName,
                                 EmailConfirmed = user.EmailConfirmed,
                                 PhoneNumberConfirmed = user.PhoneNumberConfirmed,
                                 TwoFactorEnabled = user.TwoFactorEnabled,
                                 LockoutEnabled = user.LockoutEnabled,
                                 LockoutEnd = user.LockoutEnd,
                                 AccessFailedCount = user.AccessFailedCount,
                                 LastLoginDate = user.LastLogin,
                                 CreatedDate = user.CreatedDate,
                                 Roles = (from userRole in _authContext.UserRoles
                                          join role in _authContext.Roles on userRole.RoleId equals role.Id
                                          where userRole.UserId == user.Id
                                          select role.Name ?? string.Empty).ToList(),
                                 ExternalLoginsCount = _authContext.UserLogins
                                     .Count(ul => ul.UserId == user.Id)
                             };

            var users = await usersQuery
                .OrderByDescending(u => u.CreatedDate)
                .AsNoTracking()
                .ToListAsync(ct);

            var result = new BlazorMauiShared.Auth.PagedResult<UserListItemDto>
            {
                Items = users,
                TotalCount = users.Count,
                PageNumber = 1,
                PageSize = users.Count
            };

            await SendOkAsync(result, ct);
        }
    }
    public sealed class GetUserRequest
    {
        public string Id { get; set; } = string.Empty;
    }
    public sealed class Get : Endpoint<GetUserRequest, UserDetailsDto>
    {
        private readonly UserManager<CustomPCIShieldUser> _userManager;
        private readonly RoleManager<IdentityRole> _roleManager;
        private readonly AuthorizationDbContext _authContext;

        public Get(
            UserManager<CustomPCIShieldUser> userManager,
            RoleManager<IdentityRole> roleManager,
            AuthorizationDbContext authContext)
        {
            _userManager = userManager;
            _roleManager = roleManager;
            _authContext = authContext;
        }

        public override void Configure()
        {
            Get("/admin/users/{Id}");
            AuthSchemes(JwtBearerDefaults.AuthenticationScheme);
            Roles("Administrator");
            Options(x => x.WithTags("Admin - Users"));
            Summary(s =>
            {
                s.Summary = "Get detailed user information";
                s.Description = "Retrieves comprehensive details for a specific user including roles, claims, external logins, and recent login history.";
            });
        }

        public override async Task HandleAsync(GetUserRequest req, CancellationToken ct)
        {
            var user = await _userManager.FindByIdAsync(req.Id);
            if (user == null)
            {
                await SendNotFoundAsync(ct);
                return;
            }

            var userRoles = await _userManager.GetRolesAsync(user);

            var allRoles = await _roleManager.Roles
                .Select(r => r.Name ?? string.Empty)
                .ToListAsync(ct);

            var claims = (await _userManager.GetClaimsAsync(user))
                .Select(c => new UserClaimDto { ClaimType = c.Type, ClaimValue = c.Value })
                .ToList();

            var externalLogins = (await _userManager.GetLoginsAsync(user))
                .Select(l => new ExternalLoginDto
                {
                    LoginProvider = l.LoginProvider,
                    ProviderKey = l.ProviderKey,
                    ProviderDisplayName = l.ProviderDisplayName
                })
                .ToList();

            var recentLogins = await _authContext.Set<LoginHistory>()
                .Where(lh => lh.UserId == user.Id)
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

            var response = new UserDetailsDto
            {
                Id = user.Id,
                UserName = user.UserName ?? string.Empty,
                Email = user.Email ?? string.Empty,
                FirstName = user.FirstName,
                LastName = user.LastName,
                PhoneNumber = user.PhoneNumber,
                EmailConfirmed = user.EmailConfirmed,
                PhoneNumberConfirmed = user.PhoneNumberConfirmed,
                TwoFactorEnabled = user.TwoFactorEnabled,
                LockoutEnabled = user.LockoutEnabled,
                LockoutEnd = user.LockoutEnd,
                AccessFailedCount = user.AccessFailedCount,
                SecurityStamp = user.SecurityStamp,
                LastLoginDate = user.LastLogin,
                CreatedDate = user.CreatedDate,
                Roles = userRoles.ToList(),
                AllAvailableRoles = allRoles,
                ExternalLogins = externalLogins,
                Claims = claims,
                RecentLogins = recentLogins
            };

            await SendOkAsync(response, ct);
        }
    }
    public sealed class Create : Endpoint<CreateUserRequest, AdminOperationResult>
    {
        private readonly UserManager<CustomPCIShieldUser> _userManager;
        private readonly IAppLoggerService<Create> _logger;

        public Create(UserManager<CustomPCIShieldUser> userManager, IAppLoggerService<Create> logger)
        {
            _userManager = userManager;
            _logger = logger;
        }

        public override void Configure()
        {
            Post("/admin/users");
            AuthSchemes(JwtBearerDefaults.AuthenticationScheme);
            Roles("Administrator");
            Options(x => x.WithTags("Admin - Users"));
            Summary(s =>
            {
                s.Summary = "Create a new user";
                s.Description = "Creates a new user account with specified roles and optional email confirmation.";
            });
        }

        public override async Task HandleAsync(CreateUserRequest req, CancellationToken ct)
        {
            _logger.LogInformation("Creating new user: {UserName}", req.UserName);

            var existingUser = await _userManager.FindByEmailAsync(req.Email);
            if (existingUser != null)
            {
                await SendAsync(new AdminOperationResult
                {
                    Success = false,
                    Message = "User with this email already exists",
                    Errors = new() { "A user with this email address is already registered." }
                }, 400, ct);
                return;
            }

            var newUser = new CustomPCIShieldUser
            {
                Id = Guid.NewGuid().ToString(),
                UserName = req.UserName,
                Email = req.Email,
                FirstName = req.FirstName,
                LastName = req.LastName,
                PhoneNumber = req.PhoneNumber,
                EmailConfirmed = req.EmailConfirmed,
                ApplicationUserId = Guid.NewGuid(),
                CreatedDate = DateTimeOffset.UtcNow,
                SecurityStamp = Guid.NewGuid().ToString()
            };

            var createResult = await _userManager.CreateAsync(newUser, req.Password);

            if (!createResult.Succeeded)
            {
                _logger.LogWarning("Failed to create user {UserName}: {Errors}",
                    req.UserName,
                    string.Join(", ", createResult.Errors.Select(e => e.Description)));

                await SendAsync(new AdminOperationResult
                {
                    Success = false,
                    Message = "Failed to create user",
                    Errors = createResult.Errors.Select(e => e.Description).ToList()
                }, 400, ct);
                return;
            }

            if (req.Roles.Any())
            {
                var roleResult = await _userManager.AddToRolesAsync(newUser, req.Roles);
                if (!roleResult.Succeeded)
                {
                    _logger.LogWarning("Failed to assign roles to user {UserName}", req.UserName);
                }
            }

            _logger.LogInformation("Successfully created user {UserName} with ID {UserId}",
                req.UserName, newUser.Id);

            await SendOkAsync(new AdminOperationResult
            {
                Success = true,
                Message = $"User '{req.UserName}' created successfully"
            }, ct);
        }
    }
    public sealed class Update : Endpoint<UpdateUserRequest, AdminOperationResult>
    {
        private readonly UserManager<CustomPCIShieldUser> _userManager;
        private readonly IAppLoggerService<Update> _logger;

        public Update(UserManager<CustomPCIShieldUser> userManager, IAppLoggerService<Update> logger)
        {
            _userManager = userManager;
            _logger = logger;
        }

        public override void Configure()
        {
            Put("/admin/users");
            AuthSchemes(JwtBearerDefaults.AuthenticationScheme);
            Roles("Administrator");
            Options(x => x.WithTags("Admin - Users"));
            Summary(s =>
            {
                s.Summary = "Update a user";
                s.Description = "Updates user profile fields and role assignments.";
            });
        }

        public override async Task HandleAsync(UpdateUserRequest req, CancellationToken ct)
        {
            var user = await _userManager.FindByIdAsync(req.Id);
            if (user == null)
            {
                await SendNotFoundAsync(ct);
                return;
            }

            _logger.LogInformation("Updating user {UserId}: {UserName}", user.Id, user.UserName);

            user.Email = req.Email;
            user.FirstName = req.FirstName;
            user.LastName = req.LastName;
            user.PhoneNumber = req.PhoneNumber;
            user.EmailConfirmed = req.EmailConfirmed;
            user.PhoneNumberConfirmed = req.PhoneNumberConfirmed;
            user.TwoFactorEnabled = req.TwoFactorEnabled;

            var updateResult = await _userManager.UpdateAsync(user);
            if (!updateResult.Succeeded)
            {
                await SendAsync(new AdminOperationResult
                {
                    Success = false,
                    Message = "Failed to update user",
                    Errors = updateResult.Errors.Select(e => e.Description).ToList()
                }, 400, ct);
                return;
            }

            var currentRoles = await _userManager.GetRolesAsync(user);
            var rolesToRemove = currentRoles.Except(req.Roles).ToList();
            var rolesToAdd = req.Roles.Except(currentRoles).ToList();

            if (rolesToRemove.Any())
            {
                var removeResult = await _userManager.RemoveFromRolesAsync(user, rolesToRemove);
                if (!removeResult.Succeeded)
                {
                    _logger.LogWarning("Failed to remove roles from user {UserId}", user.Id);
                }
            }

            if (rolesToAdd.Any())
            {
                var addResult = await _userManager.AddToRolesAsync(user, rolesToAdd);
                if (!addResult.Succeeded)
                {
                    _logger.LogWarning("Failed to add roles to user {UserId}", user.Id);
                }
            }

            _logger.LogInformation("Successfully updated user {UserId}", user.Id);

            await SendOkAsync(new AdminOperationResult
            {
                Success = true,
                Message = "User updated successfully"
            }, ct);
        }
    }
    public sealed class Delete : Endpoint<DeleteUserRequest, AdminOperationResult>
    {
        private readonly UserManager<CustomPCIShieldUser> _userManager;
        private readonly IRepository<ApplicationUser> _appUserRepository;
        private readonly IAppLoggerService<Delete> _logger;

        public Delete(
            UserManager<CustomPCIShieldUser> userManager,
            IRepository<ApplicationUser> appUserRepository,
            IAppLoggerService<Delete> logger)
        {
            _userManager = userManager;
            _appUserRepository = appUserRepository;
            _logger = logger;
        }

        public override void Configure()
        {
            Delete("/admin/users");
            AuthSchemes(JwtBearerDefaults.AuthenticationScheme);
            Roles("Administrator");
            Options(x => x.WithTags("Admin - Users"));
            Summary(s =>
            {
                s.Summary = "Delete a user";
                s.Description = "Deletes an identity user and optionally related application user.";
            });
        }

        public override async Task HandleAsync(DeleteUserRequest req, CancellationToken ct)
        {
            var user = await _userManager.FindByIdAsync(req.UserId);
            if (user == null)
            {
                await SendNotFoundAsync(ct);
                return;
            }

            _logger.LogWarning("Deleting user {UserId}: {UserName}. Reason: {Reason}",
                user.Id, user.UserName, req.Reason ?? "Not specified");

            var deleteResult = await _userManager.DeleteAsync(user);
            if (!deleteResult.Succeeded)
            {
                await SendAsync(new AdminOperationResult
                {
                    Success = false,
                    Message = "Failed to delete user",
                    Errors = deleteResult.Errors.Select(e => e.Description).ToList()
                }, 400, ct);
                return;
            }

            if (req.DeleteApplicationUser)
            {
                var appUser = await _appUserRepository.GetByIdAsync(user.ApplicationUserId, ct);
                if (appUser != null)
                {
                    await _appUserRepository.DeleteAsync(appUser, ct);
                    _logger.LogInformation("Deleted application user {ApplicationUserId}", user.ApplicationUserId);
                }
            }

            _logger.LogInformation("Successfully deleted user {UserId}", user.Id);

            await SendOkAsync(new AdminOperationResult
            {
                Success = true,
                Message = "User deleted successfully"
            }, ct);
        }
    }
    public sealed class LockUnlock : Endpoint<LockUserRequest, AdminOperationResult>
    {
        private readonly UserManager<CustomPCIShieldUser> _userManager;
        private readonly IAppLoggerService<LockUnlock> _logger;

        public LockUnlock(
            UserManager<CustomPCIShieldUser> userManager,
            IAppLoggerService<LockUnlock> logger)
        {
            _userManager = userManager;
            _logger = logger;
        }

        public override void Configure()
        {
            Post("/admin/users/lock");
            AuthSchemes(JwtBearerDefaults.AuthenticationScheme);
            Roles("Administrator");
            Options(x => x.WithTags("Admin - Users"));
            Summary(s =>
            {
                s.Summary = "Lock or unlock a user account";
                s.Description = "Locks a user account until specified date or unlocks it. Locked users cannot sign in.";
            });
        }

        public override async Task HandleAsync(LockUserRequest req, CancellationToken ct)
        {
            var user = await _userManager.FindByIdAsync(req.UserId);
            if (user == null)
            {
                await SendNotFoundAsync(ct);
                return;
            }

            if (req.Lock)
            {
                var lockoutEnd = req.LockoutEnd ?? DateTimeOffset.UtcNow.AddYears(100);
                var result = await _userManager.SetLockoutEndDateAsync(user, lockoutEnd);

                if (!result.Succeeded)
                {
                    await SendAsync(new AdminOperationResult
                    {
                        Success = false,
                        Message = "Failed to lock user",
                        Errors = result.Errors.Select(e => e.Description).ToList()
                    }, 400, ct);
                    return;
                }

                _logger.LogWarning("User {UserId} locked until {LockoutEnd}. Reason: {Reason}",
                    user.Id, lockoutEnd, req.Reason ?? "Not specified");

                await SendOkAsync(new AdminOperationResult
                {
                    Success = true,
                    Message = $"User locked until {lockoutEnd:yyyy-MM-dd HH:mm}"
                }, ct);
            }
            else
            {
                var result = await _userManager.SetLockoutEndDateAsync(user, null);

                if (!result.Succeeded)
                {
                    await SendAsync(new AdminOperationResult
                    {
                        Success = false,
                        Message = "Failed to unlock user",
                        Errors = result.Errors.Select(e => e.Description).ToList()
                    }, 400, ct);
                    return;
                }

                await _userManager.ResetAccessFailedCountAsync(user);

                _logger.LogInformation("User {UserId} unlocked", user.Id);

                await SendOkAsync(new AdminOperationResult
                {
                    Success = true,
                    Message = "User unlocked successfully"
                }, ct);
            }
        }
    }
    public sealed class RefreshSecurityStamp : Endpoint<RefreshSecurityStampRequest, AdminOperationResult>
    {
        private readonly UserManager<CustomPCIShieldUser> _userManager;
        private readonly IAppLoggerService<RefreshSecurityStamp> _logger;

        public RefreshSecurityStamp(
            UserManager<CustomPCIShieldUser> userManager,
            IAppLoggerService<RefreshSecurityStamp> logger)
        {
            _userManager = userManager;
            _logger = logger;
        }

        public override void Configure()
        {
            Post("/admin/users/refresh-security-stamp");
            AuthSchemes(JwtBearerDefaults.AuthenticationScheme);
            Roles("Administrator");
            Options(x => x.WithTags("Admin - Users"));
            Summary(s =>
            {
                s.Summary = "Force user logout (refresh security stamp)";
                s.Description = "Updates the user's security stamp, invalidating all existing tokens and forcing logout on all devices.";
            });
        }

        public override async Task HandleAsync(RefreshSecurityStampRequest req, CancellationToken ct)
        {
            var user = await _userManager.FindByIdAsync(req.UserId);
            if (user == null)
            {
                await SendNotFoundAsync(ct);
                return;
            }

            _logger.LogWarning("Refreshing security stamp for user {UserId}. Reason: {Reason}",
                user.Id, req.Reason ?? "Not specified");

            var result = await _userManager.UpdateSecurityStampAsync(user);

            if (!result.Succeeded)
            {
                await SendAsync(new AdminOperationResult
                {
                    Success = false,
                    Message = "Failed to refresh security stamp",
                    Errors = result.Errors.Select(e => e.Description).ToList()
                }, 400, ct);
                return;
            }

            _logger.LogInformation("Security stamp refreshed for user {UserId}. All sessions invalidated.", user.Id);

            await SendOkAsync(new AdminOperationResult
            {
                Success = true,
                Message = "Security stamp refreshed. User has been logged out from all devices."
            }, ct);
        }
    }
    public sealed class ChangePassword : Endpoint<ChangeUserPasswordRequest, AdminOperationResult>
    {
        private readonly UserManager<CustomPCIShieldUser> _userManager;
        private readonly IAppLoggerService<ChangePassword> _logger;

        public ChangePassword(
            UserManager<CustomPCIShieldUser> userManager,
            IAppLoggerService<ChangePassword> logger)
        {
            _userManager = userManager;
            _logger = logger;
        }

        public override void Configure()
        {
            Post("/admin/users/change-password");
            AuthSchemes(JwtBearerDefaults.AuthenticationScheme);
            Roles("Administrator");
            Options(x => x.WithTags("Admin - Users"));
            Summary(s =>
            {
                s.Summary = "Change a user's password";
                s.Description = "Removes the user's current password and sets a new one. Optionally updates security stamp to require re-login.";
            });
        }

        public override async Task HandleAsync(ChangeUserPasswordRequest req, CancellationToken ct)
        {
            var user = await _userManager.FindByIdAsync(req.UserId);
            if (user == null)
            {
                await SendNotFoundAsync(ct);
                return;
            }

            _logger.LogInformation("Admin changing password for user {UserId}", user.Id);

            var removeResult = await _userManager.RemovePasswordAsync(user);
            if (!removeResult.Succeeded)
            {
                await SendAsync(new AdminOperationResult
                {
                    Success = false,
                    Message = "Failed to change password",
                    Errors = removeResult.Errors.Select(e => e.Description).ToList()
                }, 400, ct);
                return;
            }

            var addResult = await _userManager.AddPasswordAsync(user, req.NewPassword);
            if (!addResult.Succeeded)
            {
                await SendAsync(new AdminOperationResult
                {
                    Success = false,
                    Message = "Failed to set new password",
                    Errors = addResult.Errors.Select(e => e.Description).ToList()
                }, 400, ct);
                return;
            }

            if (req.RequirePasswordChange)
            {
                await _userManager.UpdateSecurityStampAsync(user);
            }

            _logger.LogInformation("Password changed successfully for user {UserId}", user.Id);

            await SendOkAsync(new AdminOperationResult
            {
                Success = true,
                Message = "Password changed successfully"
            }, ct);
        }
    }
}
