using System;
using System.Linq;
using System.Collections.Generic;
using System.Linq.Expressions;
using PCIShieldLib.SharedKernel.Interfaces;
using Ardalis.Specification;
using Ardalis.GuardClauses;
using PCIShield.Domain.Entities;
using PCIShield.Domain.ModelEntityDto;

namespace PCIShield.Domain.Specifications
{
    public sealed class ApplicationUserListPagedSpec : PagedSpecification<ApplicationUser, ApplicationUserEntityDto>
    {
        public ApplicationUserListPagedSpec(int pageNumber, int pageSize)
        {
            _ = Guard.Against.NegativeOrZero(pageNumber, nameof(pageNumber));
            _ = Guard.Against.NegativeOrZero(pageSize, nameof(pageSize));

            _ = Query
                .OrderByDescending(i => i.CreatedDate)
                .Skip((pageNumber - 1) * pageSize)
                .Take(pageSize);

            _ = Query.Select(x => new ApplicationUserEntityDto
            {
                ApplicationUserId = x.ApplicationUserId,
                FirstName = x.FirstName,
                LastName = x.LastName,
                UserName = x.UserName,
                CreatedDate = x.CreatedDate,
                CreatedBy = x.CreatedBy,
                UpdatedDate = x.UpdatedDate,
                UpdatedBy = x.UpdatedBy,
                IsLoginAllowed = x.IsLoginAllowed,
                LastLogin = x.LastLogin,
                LogoutTime = x.LogoutTime,
                LastFailedLogin = x.LastFailedLogin,
                FailedLoginCount = x.FailedLoginCount,
                Email = x.Email,
                Phone = x.Phone,
                AvatarUrl = x.AvatarUrl,
                IsUserApproved = x.IsUserApproved,
                IsPhoneVerified = x.IsPhoneVerified,
                IsEmailVerified = x.IsEmailVerified,
                ConfirmationEmail = x.ConfirmationEmail,
                LastPasswordChange = x.LastPasswordChange,
                IsLocked = x.IsLocked,
                LockedUntil = x.LockedUntil,
                IsDeleted = x.IsDeleted,
                IsUserFullyRegistered = x.IsUserFullyRegistered,
                AvailabilityRank = x.AvailabilityRank,
                IsBanned = x.IsBanned,
                IsFullyRegistered = x.IsFullyRegistered,
                LastLoginIP = x.LastLoginIP,
                LastActiveAt = x.LastActiveAt,
                IsOnline = x.IsOnline,
                IsConnectedToSignalr = x.IsConnectedToSignalr,
                TimeLastSignalrPing = x.TimeLastSignalrPing,
                IsLoggedIntoApp = x.IsLoggedIntoApp,
                TimeLastLoggedToApp = x.TimeLastLoggedToApp,
                AverageResponseTime = x.AverageResponseTime,
                UserIconUrl = x.UserIconUrl,
                UserProfileImagePath = x.UserProfileImagePath,
                UserBirthDate = x.UserBirthDate,
                CreatedAt = x.CreatedAt,
                UpdatedAt = x.UpdatedAt,
                TenantId = x.TenantId,
                IsEmployee = x.IsEmployee,
                IsErpOwner = x.IsErpOwner,
                IsCustomer = x.IsCustomer,
            })
            .AsNoTracking()
            .AsSplitQuery()
            .EnableCache($"ApplicationUserListPagedSpec-{pageNumber}-{pageSize}");
        }
    }
    public sealed class ApplicationUserSearchSpec : Specification<ApplicationUser>
    {
        public ApplicationUserSearchSpec(string searchTerm)
        {
            string searchLower = searchTerm?.ToLower() ?? string.Empty;

            Query
                .Where(c =>
                        (c.AvailabilityRank != null && c.AvailabilityRank.ToLower().Contains(searchLower)) ||
                        (c.FirstName != null && c.FirstName.ToLower().Contains(searchLower)) ||
                        (c.LastName != null && c.LastName.ToLower().Contains(searchLower)) ||
                        (c.UserName != null && c.UserName.ToLower().Contains(searchLower))                )
                .OrderByDescending(c => c.CreatedDate);
        }
    }

    public sealed class ApplicationUserLastCreatedSpec : Specification<ApplicationUser>
    {
        public ApplicationUserLastCreatedSpec()
        {
            Query
                .OrderByDescending(c => c.CreatedDate)
                .Take(1)
                .AsNoTracking()
                .EnableCache("ApplicationUserLastCreatedSpec");
        }
    }
    public sealed class ApplicationUserByIdSpec : Specification<ApplicationUser, ApplicationUserEntityDto>
    {
        public ApplicationUserByIdSpec(Guid id)
        {
            _ = Guard.Against.NullOrEmpty(id, nameof(id));

            _ = Query.Where(x => x.ApplicationUserId == id);

            _ = Query.Select(x => new ApplicationUserEntityDto
            {
                ApplicationUserId = x.ApplicationUserId,
                FirstName = x.FirstName,
                LastName = x.LastName,
                UserName = x.UserName,
                CreatedDate = x.CreatedDate,
                CreatedBy = x.CreatedBy,
                UpdatedDate = x.UpdatedDate,
                UpdatedBy = x.UpdatedBy,
                IsLoginAllowed = x.IsLoginAllowed,
                LastLogin = x.LastLogin,
                LogoutTime = x.LogoutTime,
                LastFailedLogin = x.LastFailedLogin,
                FailedLoginCount = x.FailedLoginCount,
                Email = x.Email,
                Phone = x.Phone,
                AvatarUrl = x.AvatarUrl,
                IsUserApproved = x.IsUserApproved,
                IsPhoneVerified = x.IsPhoneVerified,
                IsEmailVerified = x.IsEmailVerified,
                ConfirmationEmail = x.ConfirmationEmail,
                LastPasswordChange = x.LastPasswordChange,
                IsLocked = x.IsLocked,
                LockedUntil = x.LockedUntil,
                IsDeleted = x.IsDeleted,
                IsUserFullyRegistered = x.IsUserFullyRegistered,
                AvailabilityRank = x.AvailabilityRank,
                IsBanned = x.IsBanned,
                IsFullyRegistered = x.IsFullyRegistered,
                LastLoginIP = x.LastLoginIP,
                LastActiveAt = x.LastActiveAt,
                IsOnline = x.IsOnline,
                IsConnectedToSignalr = x.IsConnectedToSignalr,
                TimeLastSignalrPing = x.TimeLastSignalrPing,
                IsLoggedIntoApp = x.IsLoggedIntoApp,
                TimeLastLoggedToApp = x.TimeLastLoggedToApp,
                AverageResponseTime = x.AverageResponseTime,
                UserIconUrl = x.UserIconUrl,
                UserProfileImagePath = x.UserProfileImagePath,
                UserBirthDate = x.UserBirthDate,
                CreatedAt = x.CreatedAt,
                UpdatedAt = x.UpdatedAt,
                TenantId = x.TenantId,
                IsEmployee = x.IsEmployee,
                IsErpOwner = x.IsErpOwner,
                IsCustomer = x.IsCustomer,

            })
            .AsNoTracking()
            .AsSplitQuery()
            .EnableCache($"ApplicationUserByIdSpec-{id.ToString()}");
        }
    }
}

    public sealed class ApplicationUserAdvancedFilterSpec : Specification<ApplicationUser>
    {
        public ApplicationUserAdvancedFilterSpec(
            int pageNumber,
            int pageSize,
            Dictionary<string, string> filters = null,
            List<Sort> sorting = null
        )
        {
            _ = Guard.Against.NegativeOrZero(pageNumber, nameof(pageNumber));
            _ = Guard.Against.NegativeOrZero(pageSize, nameof(pageSize));

            _ = Query;

            if (filters != null)
            {
                foreach (var filter in filters)
                {
                    switch (filter.Key.ToLower())
                    {
                        case "availabilityrank":
                            Query.Where(c => c.AvailabilityRank.Contains(filter.Value));
                            break;
                        case "firstname":
                            Query.Where(c => c.FirstName.Contains(filter.Value));
                            break;
                        case "lastname":
                            Query.Where(c => c.LastName.Contains(filter.Value));
                            break;
                        case "username":
                            Query.Where(c => c.UserName.Contains(filter.Value));
                            break;
                        case "createddate":
                            if (DateTime.TryParse(filter.Value, out DateTime createddate))
                            {
                                Query.Where(c => c.CreatedDate >= createddate.AddHours(-6) && c.CreatedDate <= createddate.AddHours(6));
                            }
                            break;
                        case "updateddate":
                            if (DateTime.TryParse(filter.Value, out DateTime updateddate))
                            {
                                Query.Where(c => c.UpdatedDate >= updateddate.AddHours(-6) && c.UpdatedDate <= updateddate.AddHours(6));
                            }
                            break;
                        case "isloginallowed":
                            if (bool.TryParse(filter.Value, out bool isloginallowed))
                            {
                                Query.Where(c => c.IsLoginAllowed == isloginallowed);
                            }
                            break;
                        case "lastlogin":
                            if (DateTime.TryParse(filter.Value, out DateTime lastlogin))
                            {
                                Query.Where(c => c.LastLogin >= lastlogin.AddHours(-6) && c.LastLogin <= lastlogin.AddHours(6));
                            }
                            break;
                        case "logouttime":
                            if (DateTime.TryParse(filter.Value, out DateTime logouttime))
                            {
                                Query.Where(c => c.LogoutTime >= logouttime.AddHours(-6) && c.LogoutTime <= logouttime.AddHours(6));
                            }
                            break;
                        case "lastfailedlogin":
                            if (DateTime.TryParse(filter.Value, out DateTime lastfailedlogin))
                            {
                                Query.Where(c => c.LastFailedLogin >= lastfailedlogin.AddHours(-6) && c.LastFailedLogin <= lastfailedlogin.AddHours(6));
                            }
                            break;
                        case "failedlogincount":
                            if (int.TryParse(filter.Value, out int failedlogincount))
                            {
                                Query.Where(c => c.FailedLoginCount == failedlogincount);
                            }
                            break;
                        case "isuserapproved":
                            if (bool.TryParse(filter.Value, out bool isuserapproved))
                            {
                                Query.Where(c => c.IsUserApproved == isuserapproved);
                            }
                            break;
                        case "isphoneverified":
                            if (bool.TryParse(filter.Value, out bool isphoneverified))
                            {
                                Query.Where(c => c.IsPhoneVerified == isphoneverified);
                            }
                            break;
                        case "isemailverified":
                            if (bool.TryParse(filter.Value, out bool isemailverified))
                            {
                                Query.Where(c => c.IsEmailVerified == isemailverified);
                            }
                            break;
                        case "lastpasswordchange":
                            if (DateTime.TryParse(filter.Value, out DateTime lastpasswordchange))
                            {
                                Query.Where(c => c.LastPasswordChange >= lastpasswordchange.AddHours(-6) && c.LastPasswordChange <= lastpasswordchange.AddHours(6));
                            }
                            break;
                        case "islocked":
                            if (bool.TryParse(filter.Value, out bool islocked))
                            {
                                Query.Where(c => c.IsLocked == islocked);
                            }
                            break;
                        case "lockeduntil":
                            if (DateTime.TryParse(filter.Value, out DateTime lockeduntil))
                            {
                                Query.Where(c => c.LockedUntil >= lockeduntil.AddHours(-6) && c.LockedUntil <= lockeduntil.AddHours(6));
                            }
                            break;
                        case "isdeleted":
                            if (bool.TryParse(filter.Value, out bool isdeleted))
                            {
                                Query.Where(c => c.IsDeleted == isdeleted);
                            }
                            break;
                        case "isuserfullyregistered":
                            if (bool.TryParse(filter.Value, out bool isuserfullyregistered))
                            {
                                Query.Where(c => c.IsUserFullyRegistered == isuserfullyregistered);
                            }
                            break;
                        case "isbanned":
                            if (bool.TryParse(filter.Value, out bool isbanned))
                            {
                                Query.Where(c => c.IsBanned == isbanned);
                            }
                            break;
                        case "isfullyregistered":
                            if (bool.TryParse(filter.Value, out bool isfullyregistered))
                            {
                                Query.Where(c => c.IsFullyRegistered == isfullyregistered);
                            }
                            break;
                        case "lastactiveat":
                            if (DateTime.TryParse(filter.Value, out DateTime lastactiveat))
                            {
                                Query.Where(c => c.LastActiveAt >= lastactiveat.AddHours(-6) && c.LastActiveAt <= lastactiveat.AddHours(6));
                            }
                            break;
                        case "isonline":
                            if (bool.TryParse(filter.Value, out bool isonline))
                            {
                                Query.Where(c => c.IsOnline == isonline);
                            }
                            break;
                        case "isconnectedtosignalr":
                            if (bool.TryParse(filter.Value, out bool isconnectedtosignalr))
                            {
                                Query.Where(c => c.IsConnectedToSignalr == isconnectedtosignalr);
                            }
                            break;
                        case "timelastsignalrping":
                            if (DateTime.TryParse(filter.Value, out DateTime timelastsignalrping))
                            {
                                Query.Where(c => c.TimeLastSignalrPing >= timelastsignalrping.AddHours(-6) && c.TimeLastSignalrPing <= timelastsignalrping.AddHours(6));
                            }
                            break;
                        case "isloggedintoapp":
                            if (bool.TryParse(filter.Value, out bool isloggedintoapp))
                            {
                                Query.Where(c => c.IsLoggedIntoApp == isloggedintoapp);
                            }
                            break;
                        case "timelastloggedtoapp":
                            if (DateTime.TryParse(filter.Value, out DateTime timelastloggedtoapp))
                            {
                                Query.Where(c => c.TimeLastLoggedToApp >= timelastloggedtoapp.AddHours(-6) && c.TimeLastLoggedToApp <= timelastloggedtoapp.AddHours(6));
                            }
                            break;
                        case "averageresponsetime":
                            if (int.TryParse(filter.Value, out int averageresponsetime))
                            {
                                Query.Where(c => c.AverageResponseTime == averageresponsetime);
                            }
                            break;
                        case "userbirthdate":
                            if (DateTime.TryParse(filter.Value, out DateTime userbirthdate))
                            {
                                Query.Where(c => c.UserBirthDate >= userbirthdate.AddHours(-6) && c.UserBirthDate <= userbirthdate.AddHours(6));
                            }
                            break;
                        case "createdat":
                            if (DateTime.TryParse(filter.Value, out DateTime createdat))
                            {
                                Query.Where(c => c.CreatedAt >= createdat.AddHours(-6) && c.CreatedAt <= createdat.AddHours(6));
                            }
                            break;
                        case "updatedat":
                            if (DateTime.TryParse(filter.Value, out DateTime updatedat))
                            {
                                Query.Where(c => c.UpdatedAt >= updatedat.AddHours(-6) && c.UpdatedAt <= updatedat.AddHours(6));
                            }
                            break;
                        case "isemployee":
                            if (bool.TryParse(filter.Value, out bool isemployee))
                            {
                                Query.Where(c => c.IsEmployee == isemployee);
                            }
                            break;
                        case "iserpowner":
                            if (bool.TryParse(filter.Value, out bool iserpowner))
                            {
                                Query.Where(c => c.IsErpOwner == iserpowner);
                            }
                            break;
                        case "iscustomer":
                            if (bool.TryParse(filter.Value, out bool iscustomer))
                            {
                                Query.Where(c => c.IsCustomer == iscustomer);
                            }
                            break;
                    }
                }
            }

            if (sorting != null && sorting.Any())
            {
                var first = sorting.First();
                var ordered = ApplySort(Query, first);

                foreach (var sort in sorting.Skip(1))
                {
                    ordered = ApplyAdditionalSort(ordered, sort);
                }
            }
            else
            {
                Query.OrderByDescending(x => x.CreatedDate);
            }

            Query.Skip((pageNumber - 1) * pageSize).Take(pageSize);
        }

        private static IOrderedSpecificationBuilder<ApplicationUser> ApplySort(
            ISpecificationBuilder<ApplicationUser> query,
            Sort sort
        )
        {
            return sort.Direction == SortDirection.Descending
                ? query.OrderByDescending(GetSortProperty(sort.Field))
                : query.OrderBy(GetSortProperty(sort.Field));
        }

        private static IOrderedSpecificationBuilder<ApplicationUser> ApplyAdditionalSort(
            IOrderedSpecificationBuilder<ApplicationUser> query,
            Sort sort
        )
        {
            return sort.Direction == SortDirection.Descending
                ? query.ThenByDescending(GetSortProperty(sort.Field))
                : query.ThenBy(GetSortProperty(sort.Field));
        }

        private static Expression<Func<ApplicationUser, object>> GetSortProperty(
            string propertyName
        )
        {
            return propertyName.ToLower() switch
            {
                "availabilityrank" => c => c.AvailabilityRank,
                "firstname" => c => c.FirstName,
                "lastname" => c => c.LastName,
                "username" => c => c.UserName,
                _ => c => c.ApplicationUserId,
            };
        }
    }