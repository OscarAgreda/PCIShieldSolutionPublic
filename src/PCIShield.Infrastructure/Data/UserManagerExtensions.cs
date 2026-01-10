using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

namespace PCIShield.Infrastructure.Data
{
    public static class UserManagerExtensions
    {
        public static async Task<TUser?> FindByRefreshTokenAsync<TUser>(
            this UserManager<TUser> userManager, 
            string refreshToken,
            CancellationToken cancellationToken = default) where TUser : class
        {
            if (string.IsNullOrWhiteSpace(refreshToken))
                return null;
            return await userManager.Users
                .Where(u => u == refreshToken)
                .FirstOrDefaultAsync(cancellationToken);
        }
        public static async Task<TUser?> FindByValidRefreshTokenAsync<TUser>(
            this UserManager<TUser> userManager,
            string refreshToken,
            bool includeExpired = false,
            CancellationToken cancellationToken = default) where TUser : CustomPCIShieldUser
        {
            if (string.IsNullOrWhiteSpace(refreshToken))
                return null;

            var query = userManager.Users.Where(u => u.RefreshToken == refreshToken);

            if (!includeExpired && typeof(TUser).GetProperty("RefreshTokenExpiryTime") != null)
            {
                query = query.Where(u => u.RefreshTokenExpiryTime == null || u.RefreshTokenExpiryTime > DateTime.UtcNow);
            }

            return await query.FirstOrDefaultAsync(cancellationToken);
        }
        public static async Task<IdentityResult> UpdateRefreshTokenAsync<TUser>(
            this UserManager<TUser> userManager,
            string userId,
            string? newRefreshToken,
            DateTime? expiryTime = null,
            CancellationToken cancellationToken = default) where TUser : CustomPCIShieldUser
        {
            var user = await userManager.FindByIdAsync(userId);
            if (user == null)
            {
                return IdentityResult.Failed(new IdentityError 
                { 
                    Code = "UserNotFound", 
                    Description = "User not found" 
                });
            }

            user.RefreshToken = newRefreshToken;
            var expiryProperty = typeof(TUser).GetProperty("RefreshTokenExpiryTime");
            if (expiryProperty != null && expiryProperty.CanWrite)
            {
                expiryProperty.SetValue(user, expiryTime);
            }

            return await userManager.UpdateAsync(user);
        }
        public static async Task<IdentityResult> InvalidateRefreshTokenAsync<TUser>(
            this UserManager<TUser> userManager,
            string userId,
            CancellationToken cancellationToken = default) where TUser : CustomPCIShieldUser
        {
            return await UpdateRefreshTokenAsync(userManager, userId, null, null, cancellationToken);
        }
        public static async Task<int> CleanupExpiredRefreshTokensAsync<TUser>(
            this UserManager<TUser> userManager,
            CancellationToken cancellationToken = default) where TUser : CustomPCIShieldUser
        {
            if (typeof(TUser).GetProperty("RefreshTokenExpiryTime") == null)
                return 0;

            var expiredTokenUsers = await userManager.Users
                .Where(u => u.RefreshToken != null && u.RefreshTokenExpiryTime != null && u.RefreshTokenExpiryTime < DateTime.UtcNow)
                .ToListAsync(cancellationToken);

            int count = 0;
            foreach (var user in expiredTokenUsers)
            {
                user.RefreshToken = null;
                var expiryProperty = typeof(TUser).GetProperty("RefreshTokenExpiryTime");
                expiryProperty?.SetValue(user, null);
                
                var result = await userManager.UpdateAsync(user);
                if (result.Succeeded)
                    count++;
            }

            return count;
        }
    }
}