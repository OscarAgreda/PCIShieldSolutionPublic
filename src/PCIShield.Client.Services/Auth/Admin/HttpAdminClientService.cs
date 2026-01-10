using System;
using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Net.Http.Json;
using System.Reactive.Disposables;
using System.Reactive.Linq;
using System.Threading.Tasks;
using System.Threading.Tasks;

using Flurl;
using Flurl.Http;
using Flurl.Http.Configuration;

using LanguageExt;

using Microsoft.Extensions.Logging;
using Microsoft.Maui.Devices;

using PCIShield.BlazorMauiShared.Auth;
using PCIShield.BlazorMauiShared.Models;
using PCIShield.BlazorMauiShared.Models.Auth;
using PCIShield.Client.Services.Common;
using PCIShield.Client.Services.InvoiceSession;
using PCIShield.Client.Services.Merchant;

using PCIShieldLib.SharedKernel.Interfaces;

using ReactiveUI;

using static System.Net.WebRequestMethods;
using static LanguageExt.Prelude;

using JsonSerializer = System.Text.Json.JsonSerializer;
using JsonSerializerOptions = System.Text.Json.JsonSerializerOptions;
using Unit = System.Reactive.Unit;

namespace PCIShield.Client.Services.Auth
{
    public partial interface IHttpAuthClientService
    {
        Task<Either<string, BlazorMauiShared.Auth.PagedResult<UserListItemDto>>> GetUsersAsync();
        Task<Either<string, UserDetailsDto>> GetUserByIdAsync(string userId);
        Task<Either<string, AdminOperationResult>> CreateUserAsync(CreateUserRequest request);
        Task<Either<string, AdminOperationResult>> UpdateUserAsync(UpdateUserRequest request);
        Task<Either<string, AdminOperationResult>> DeleteUserAsync(DeleteUserRequest request);
        Task<Either<string, AdminOperationResult>> LockUserAsync(LockUserRequest request);
        Task<Either<string, AdminOperationResult>> RefreshSecurityStampAsync(RefreshSecurityStampRequest request);
        Task<Either<string, AdminOperationResult>> ChangeUserPasswordAsync(ChangeUserPasswordRequest request);
        Task<Either<string, List<RoleListItemDto>>> GetRolesAsync();
        Task<Either<string, RoleDetailsDto>> GetRoleByIdAsync(string roleId);
        Task<Either<string, AdminOperationResult>> CreateRoleAsync(CreateRoleRequest request);
        Task<Either<string, AdminOperationResult>> UpdateRoleAsync(UpdateRoleRequest request);
        Task<Either<string, AdminOperationResult>> DeleteRoleAsync(DeleteRoleRequest request);
        Task<Either<string, AdminDashboardStatsDto>> GetDashboardStatsAsync();
        Task<Either<string, List<PermissionListItemDto>>> GetPermissionsAsync();
        Task<Either<string, List<PermissionCatalogDto>>> GetPermissionCatalogAsync();
        Task<Either<string, PermissionDetailsDto>> GetPermissionByIdAsync(Guid permissionId);
        Task<Either<string, AdminOperationResult>> CreatePermissionAsync(CreatePermissionRequest request);
        Task<Either<string, AdminOperationResult>> UpdatePermissionAsync(UpdatePermissionRequest request);
        Task<Either<string, AdminOperationResult>> DeletePermissionAsync(DeletePermissionRequest request);
        Task<Either<string, AdminOperationResult>> AssignPermissionsToRoleAsync(AssignRolePermissionsRequest request);
        Task<Either<string, AdminOperationResult>> GrantPermissionToUserAsync(GrantUserPermissionRequest request);
        Task<Either<string, UserEffectivePermissionsDto>> GetUserEffectivePermissionsAsync(string userId);

    }
    public partial class HttpAuthClientService
    {

        #region User Management

        public async Task<Either<string, BlazorMauiShared.Auth.PagedResult<UserListItemDto>>> GetUsersAsync()
        {
            return await TryRequest(
                async () =>
                {
                    var token = await _tokenService.GetTokenAsync();
                    var result = await _httpClient.BaseAddress
                        .AppendPathSegment("api/admin/users")
                        .WithOAuthBearerToken(token)
                        .GetJsonAsync<BlazorMauiShared.Auth.PagedResult<UserListItemDto>>();
                    return result;
                },
                "retrieve user list"
            );
        }

        public async Task<Either<string, UserDetailsDto>> GetUserByIdAsync(string userId)
        {
            return await TryRequest(
                async () =>
                {
                    var token = await _tokenService.GetTokenAsync();
                    var result = await _httpClient.BaseAddress
                        .AppendPathSegments("api/admin/users", userId)
                        .WithOAuthBearerToken(token)
                        .GetJsonAsync<UserDetailsDto>();
                    return result;
                },
                $"retrieve user {userId}"
            );
        }

        public async Task<Either<string, AdminOperationResult>> CreateUserAsync(CreateUserRequest request)
        {
            return await TryRequest(
                async () =>
                {
                    var token = await _tokenService.GetTokenAsync();
                    var result = await _httpClient.BaseAddress
                        .AppendPathSegment("api/admin/users")
                        .WithOAuthBearerToken(token)
                        .PostJsonAsync(request)
                        .ReceiveJson<AdminOperationResult>();
                    return result;
                },
                "create user"
            );
        }

        public async Task<Either<string, AdminOperationResult>> UpdateUserAsync(UpdateUserRequest request)
        {
            return await TryRequest(
                async () =>
                {
                    var token = await _tokenService.GetTokenAsync();
                    var result = await _httpClient.BaseAddress
                        .AppendPathSegment("api/admin/users")
                        .WithOAuthBearerToken(token)
                        .PutJsonAsync(request)
                        .ReceiveJson<AdminOperationResult>();
                    return result;
                },
                $"update user {request.Id}"
            );
        }

        public async Task<Either<string, AdminOperationResult>> DeleteUserAsync(DeleteUserRequest request)
        {
            return await TryRequest(
                async () =>
                {
                    var token = await _tokenService.GetTokenAsync();
                    var result = await _httpClient.BaseAddress
                        .AppendPathSegment("api/admin/users")
                        .WithOAuthBearerToken(token)
                        .SendJsonAsync(HttpMethod.Delete, request)
                        .ReceiveJson<AdminOperationResult>();
                    return result;
                },
                $"delete user {request.UserId}"
            );
        }

        public async Task<Either<string, AdminOperationResult>> LockUserAsync(LockUserRequest request)
        {
            return await TryRequest(
                async () =>
                {
                    var token = await _tokenService.GetTokenAsync();
                    var result = await _httpClient.BaseAddress
                        .AppendPathSegment("api/admin/users/lock")
                        .WithOAuthBearerToken(token)
                        .PostJsonAsync(request)
                        .ReceiveJson<AdminOperationResult>();
                    return result;
                },
                $"{(request.Lock ? "lock" : "unlock")} user {request.UserId}"
            );
        }

        public async Task<Either<string, AdminOperationResult>> RefreshSecurityStampAsync(RefreshSecurityStampRequest request)
        {
            return await TryRequest(
                async () =>
                {
                    var token = await _tokenService.GetTokenAsync();
                    var result = await _httpClient.BaseAddress
                        .AppendPathSegment("api/admin/users/refresh-security-stamp")
                        .WithOAuthBearerToken(token)
                        .PostJsonAsync(request)
                        .ReceiveJson<AdminOperationResult>();
                    return result;
                },
                $"refresh security stamp for user {request.UserId}"
            );
        }

        public async Task<Either<string, AdminOperationResult>> ChangeUserPasswordAsync(ChangeUserPasswordRequest request)
        {
            return await TryRequest(
                async () =>
                {
                    var token = await _tokenService.GetTokenAsync();
                    var result = await _httpClient.BaseAddress
                        .AppendPathSegment("api/admin/users/change-password")
                        .WithOAuthBearerToken(token)
                        .PostJsonAsync(request)
                        .ReceiveJson<AdminOperationResult>();
                    return result;
                },
                $"change password for user {request.UserId}"
            );
        }

        #endregion

        #region Role Management

        public async Task<Either<string, List<RoleListItemDto>>> GetRolesAsync()
        {
            return await TryRequest(
                async () =>
                {
                    var token = await _tokenService.GetTokenAsync();
                    var result = await _httpClient.BaseAddress
                        .AppendPathSegment("api/admin/roles")
                        .WithOAuthBearerToken(token)
                        .GetJsonAsync<List<RoleListItemDto>>();
                    return result;
                },
                "retrieve role list"
            );
        }

        public async Task<Either<string, RoleDetailsDto>> GetRoleByIdAsync(string roleId)
        {
            return await TryRequest(
                async () =>
                {
                    var token = await _tokenService.GetTokenAsync();
                    var result = await _httpClient.BaseAddress
                        .AppendPathSegments("api/admin/roles", roleId)
                        .WithOAuthBearerToken(token)
                        .GetJsonAsync<RoleDetailsDto>();
                    return result;
                },
                $"retrieve role {roleId}"
            );
        }

        public async Task<Either<string, AdminOperationResult>> CreateRoleAsync(CreateRoleRequest request)
        {
            return await TryRequest(
                async () =>
                {
                    var token = await _tokenService.GetTokenAsync();
                    var result = await _httpClient.BaseAddress
                        .AppendPathSegment("api/admin/roles")
                        .WithOAuthBearerToken(token)
                        .PostJsonAsync(request)
                        .ReceiveJson<AdminOperationResult>();
                    return result;
                },
                "create role"
            );
        }

        public async Task<Either<string, AdminOperationResult>> UpdateRoleAsync(UpdateRoleRequest request)
        {
            return await TryRequest(
                async () =>
                {
                    var token = await _tokenService.GetTokenAsync();
                    var result = await _httpClient.BaseAddress
                        .AppendPathSegment("api/admin/roles")
                        .WithOAuthBearerToken(token)
                        .PutJsonAsync(request)
                        .ReceiveJson<AdminOperationResult>();
                    return result;
                },
                $"update role {request.Id}"
            );
        }

        public async Task<Either<string, AdminOperationResult>> DeleteRoleAsync(DeleteRoleRequest request)
        {
            return await TryRequest(
                async () =>
                {
                    var token = await _tokenService.GetTokenAsync();
                    var result = await _httpClient.BaseAddress
                        .AppendPathSegment("api/admin/roles")
                        .WithOAuthBearerToken(token)
                        .SendJsonAsync(HttpMethod.Delete, request)
                        .ReceiveJson<AdminOperationResult>();
                    return result;
                },
                $"delete role {request.RoleId}"
            );
        }

        #endregion
        #region Permission Management

        public async Task<Either<string, List<PermissionListItemDto>>> GetPermissionsAsync()
        {
            return await TryRequest(
                async () =>
                {
                    var token = await _tokenService.GetTokenAsync();
                    var result = await _httpClient.BaseAddress
                        .AppendPathSegment("api/admin/permissions")
                        .WithOAuthBearerToken(token)
                        .GetJsonAsync<List<PermissionListItemDto>>();
                    return result;
                },
                "retrieve permission list"
            );
        }

        public async Task<Either<string, List<PermissionCatalogDto>>> GetPermissionCatalogAsync()
        {
            return await TryRequest(
                async () =>
                {
                    var token = await _tokenService.GetTokenAsync();
                    var result = await _httpClient.BaseAddress
                        .AppendPathSegment("api/admin/permissions/catalog")
                        .WithOAuthBearerToken(token)
                        .GetJsonAsync<List<PermissionCatalogDto>>();
                    return result;
                },
                "retrieve permission catalog"
            );
        }

        public async Task<Either<string, PermissionDetailsDto>> GetPermissionByIdAsync(Guid permissionId)
        {
            return await TryRequest(
                async () =>
                {
                    var token = await _tokenService.GetTokenAsync();
                    var result = await _httpClient.BaseAddress
                        .AppendPathSegments("api/admin/permissions", permissionId)
                        .WithOAuthBearerToken(token)
                        .GetJsonAsync<PermissionDetailsDto>();
                    return result;
                },
                $"retrieve permission {permissionId}"
            );
        }

        public async Task<Either<string, AdminOperationResult>> CreatePermissionAsync(CreatePermissionRequest request)
        {
            return await TryRequest(
                async () =>
                {
                    var token = await _tokenService.GetTokenAsync();
                    var result = await _httpClient.BaseAddress
                        .AppendPathSegment("api/admin/permissions")
                        .WithOAuthBearerToken(token)
                        .PostJsonAsync(request)
                        .ReceiveJson<AdminOperationResult>();
                    return result;
                },
                "create permission"
            );
        }

        public async Task<Either<string, AdminOperationResult>> UpdatePermissionAsync(UpdatePermissionRequest request)
        {
            return await TryRequest(
                async () =>
                {
                    var token = await _tokenService.GetTokenAsync();
                    var result = await _httpClient.BaseAddress
                        .AppendPathSegment("api/admin/permissions")
                        .WithOAuthBearerToken(token)
                        .PutJsonAsync(request)
                        .ReceiveJson<AdminOperationResult>();
                    return result;
                },
                $"update permission {request.Id}"
            );
        }

        public async Task<Either<string, AdminOperationResult>> DeletePermissionAsync(DeletePermissionRequest request)
        {
            return await TryRequest(
                async () =>
                {
                    var token = await _tokenService.GetTokenAsync();
                    var result = await _httpClient.BaseAddress
                        .AppendPathSegment("api/admin/permissions")
                        .WithOAuthBearerToken(token)
                        .SendJsonAsync(HttpMethod.Delete, request)
                        .ReceiveJson<AdminOperationResult>();
                    return result;
                },
                $"delete permission {request.PermissionId}"
            );
        }

        public async Task<Either<string, AdminOperationResult>> AssignPermissionsToRoleAsync(AssignRolePermissionsRequest request)
        {
            return await TryRequest(
                async () =>
                {
                    var token = await _tokenService.GetTokenAsync();
                    var result = await _httpClient.BaseAddress
                        .AppendPathSegment("api/admin/permissions/assign-to-role")
                        .WithOAuthBearerToken(token)
                        .PostJsonAsync(request)
                        .ReceiveJson<AdminOperationResult>();
                    return result;
                },
                $"assign permissions to role {request.RoleId}"
            );
        }

        public async Task<Either<string, AdminOperationResult>> GrantPermissionToUserAsync(GrantUserPermissionRequest request)
        {
            return await TryRequest(
                async () =>
                {
                    var token = await _tokenService.GetTokenAsync();
                    var result = await _httpClient.BaseAddress
                        .AppendPathSegment("api/admin/permissions/grant-to-user")
                        .WithOAuthBearerToken(token)
                        .PostJsonAsync(request)
                        .ReceiveJson<AdminOperationResult>();
                    return result;
                },
                $"grant permission to user {request.UserId}"
            );
        }

        public async Task<Either<string, UserEffectivePermissionsDto>> GetUserEffectivePermissionsAsync(string userId)
        {
            return await TryRequest(
                async () =>
                {
                    var token = await _tokenService.GetTokenAsync();
                    var result = await _httpClient.BaseAddress
                        .AppendPathSegments("api/admin/permissions/user", userId)
                        .WithOAuthBearerToken(token)
                        .GetJsonAsync<UserEffectivePermissionsDto>();
                    return result;
                },
                $"retrieve effective permissions for user {userId}"
            );
        }

        #endregion
        #region Dashboard

        public async Task<Either<string, AdminDashboardStatsDto>> GetDashboardStatsAsync()
        {
            return await TryRequest(
                async () =>
                {
                    var token = await _tokenService.GetTokenAsync();
                    var result = await _httpClient.BaseAddress
                        .AppendPathSegment("api/admin/dashboard/stats")
                        .WithOAuthBearerToken(token)
                        .GetJsonAsync<AdminDashboardStatsDto>();
                    return result;
                },
                "retrieve dashboard statistics"
            );
        }

        #endregion

        #region Helper Methods

        private async Task<Either<string, T>> TryRequest<T>(Func<Task<T>> requestFunc, string operationDescription)
        {
            try
            {
                _logger.LogInformation("Attempting to {Operation}", operationDescription);
                var result = await requestFunc();
                _logger.LogInformation("Successfully completed {Operation}", operationDescription);
                return result;
            }
            catch (FlurlHttpException ex)
            {
                var error = await ex.GetResponseStringAsync();
                _logger.LogError(ex, "HTTP error during {Operation}: {Error}", operationDescription, error);
                return $"Failed to {operationDescription}: {ex.Message}";
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Unexpected error during {Operation}", operationDescription);
                return $"Unexpected error: {ex.Message}";
            }
        }

        #endregion

    }
}
