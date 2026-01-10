/*

Here’s a **super-condensed, enumerated inventory** of what my `AuthDbContextSeed.SeedEssentialsAsync` does—everything, in execution order, with guards and side effects.

1. Bootstrap & Scope

* Creates DI scope; resolves `AuthorizationDbContext`, `AppDbContext`, `UserManager<CustomPCIShieldUser>`, `RoleManager<IdentityRole>`.
* Uses constants: `defaultTenantId = 9132836A-6FA1-84C6-CA3A-2042AFFBD3FF`, `systemUserId = 11111111-1111-1111-1111-111111111111`.
* Logs start/finish; wraps whole flow in try/catch (rethrows on error).

2. Seed Roles (Idempotent)

* Target roles: `Administrator`, `User`, `Customer`, `Employee`.
* For each: `RoleExistsAsync` → create if missing, log result.

3. Seed Localizations (Idempotent-by-table)

* Guard: if `AppLocalizations` is empty → inserts a fixed list of keys in `en-US`/`es-ES` (Dashboard.*, MudDataGrid.*, Common.*), stamped with tenant/createdAt/createdBy.
* Else: logs “already seeded”.

4. Seed Identity Users (Idempotent-by-table)

* Guard: if any Identity users exist → fetch & return all.
* Else: creates 4 users with passwords `Password123!`

  * `admin@pciShield.com` → add to `Administrator`.
  * `user@pciShield.com` → add to `User` + `Customer`.
  * `admin2@pciShield.com` → add to `Administrator`.
  * `user2@pciShield.com` → add to `User`.
* Each user has: confirmed email/phone, `TenantId`, `ApplicationUserId` (new Guid), metadata (CreatedAt/By, First/Last, IsDeleted=false).
* Returns the list of created (or existing) `CustomPCIShieldUser`.

5. Seed Domain ApplicationUsers (Bridges Identity→Domain; Conditional)

* Precondition: at least one Identity user.
* For each Identity user:

  * Guard: if no domain `ApplicationUser` with same `ApplicationUserId` → create domain entity with mirrored data (login flags, approvals, tenant, timestamps).
  * Set `IdentityUserId` via writable property; fallback to calling `SetIdentityUserId(string)` via reflection; else log a **critical warning** to implement it.
  * Apply role flags by username:

    * `user@…` → `IsCustomer=true`, `IsEmployee=false`
    * `admin@…` or `admin2@…` → `IsErpOwner=true`, `IsEmployee=true`
    * `user2@…` → no special flags
* If any added → `SaveChangesAsync`.

6. Seed Companies (Idempotent-by-table)

* Guard: if no companies exist → create two `Company` rows:

  * **ACME001** (USA-ish data, logo/url/contacts/addresses, legal/tax fields).
  * **TECHSYS002** (second company with analogous fields).
* Else: fetch all existing companies.
* Always returns the company list.

7. Seed CompanyUserRoles (Per-user-per-company guards)

* Precondition: have Identity users AND companies.
* Determine `assignerId` = admin’s Id if available, else `systemUserId`.
* For each company:

  * Ensure **Owner** role for `admin@…` (if not present).
  * For ACME001 only: ensure `Accountant` role for `user@…` (if not present).
* If any added → `SaveChangesAsync`.

8. Idempotency Summary (Where it won’t double-seed)

* Roles: per-role existence check.
* Localizations: table-empty check (all-or-nothing first run).
* Identity users: table-existence check; otherwise creates the fixed set.
* Domain users: per `ApplicationUserId` existence check.
* Companies: table-empty check; otherwise reuses existing.
* CompanyUserRoles: per (UserId, CompanyId, CompanySpecificRole) existence check.

9. Cross-Context Contracts & Mapping

* Identity→Domain linkage via **shared `ApplicationUserId`** and **string `IdentityUserId`** stored on domain entity (property or `SetIdentityUserId` method).
* Tenanting: writes `TenantId` on users and localizations; companies include standard metadata.

10. Side Effects & Observability

* Writes to: `AspNetRoles`, `AspNetUsers` (+ join tables), `AppLocalizations`, `ApplicationUsers` (domain), `Companies`, `CompanyUserRoles`.
* Extensive console logging for each phase and decision branch; errors are logged then rethrown.

11. Failure Modes & Guards

* Any exception → logged with full `ex.ToString()` and rethrown (caller must handle).
* Missing writable `IdentityUserId` on domain entity → logs **critical** guidance to add property and EF mapping.

12. Net Result After First Successful Run (Happy Path)

* 4 Identity users with roles.
* 1:1 Domain `ApplicationUser` rows aligned to Identity users, with role flags set.
* 2 Companies created.
* CompanyUserRoles: `admin@…` = Owner on both companies; `user@…` = Accountant on ACME001.
* UI strings localized (en/es) for Dashboard, MudDataGrid, Common.

---
 */
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

using PCIShield.Domain.Entities;
using PCIShield.Infrastructure.Data;

using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;

namespace PCIShield.Infrastructure.SeedData
{
    public static class AuthDbContextSeed
    {
        public static async Task SeedEssentialsAsync(IServiceProvider services)
        {
            using var scope = services.CreateScope();
            var serviceProvider = scope.ServiceProvider;
            var authContext = serviceProvider.GetRequiredService<AuthorizationDbContext>();
            var appContext = serviceProvider.GetRequiredService<AppDbContext>();
            var userManager = serviceProvider.GetRequiredService<UserManager<CustomPCIShieldUser>>();
            var roleManager = serviceProvider.GetRequiredService<RoleManager<IdentityRole>>();
            var defaultTenantId = Guid.Parse("9132836A-6FA1-84C6-CA3A-2042AFFBD3FF");
            var systemUserId = Guid.Parse("11111111-1111-1111-1111-111111111111");
            try
            {
                await SeedRolesAsync(roleManager);

                await SeedPermissionsAsync(scope, authContext, roleManager);

                if (!await authContext.AppLocalizations.AnyAsync())
                {
                    await SeedAppLocalizationsAsync(
                        authContext,
                        defaultTenantId,
                        systemUserId
                    );
                }
                else
                {
                }
                List<CustomPCIShieldUser> identityUsers = await SeedAndGetIdentityUsersAsync(
                    userManager,
                    defaultTenantId,
                    systemUserId
                );
                if (identityUsers.Any())
                {
                    bool needToSeedDomainUsers = false;
                    foreach (var idUser in identityUsers)
                    {
                        if (
                            !await appContext.ApplicationUsers.AnyAsync(au =>
                                au.ApplicationUserId == idUser.ApplicationUserId
                            )
                        )
                        {
                            needToSeedDomainUsers = true;
                            break;
                        }
                    }
                    if (needToSeedDomainUsers || !await appContext.ApplicationUsers.AnyAsync())
                    {
                        await SeedDomainApplicationUsersAsync(
                            appContext,
                            identityUsers,
                            systemUserId
                        );
                    }
                    else
                    {
                        Console.WriteLine(
                            "Domain ApplicationUsers appear to be already seeded for the existing IdentityUsers."
                        );
                    }
                }
                else
                {
                    Console.WriteLine(
                        "No IdentityUsers found or created, skipping Domain ApplicationUser seeding."
                    );
                }
                List<Company> companies;
                if (!await authContext.Set<Company>().AnyAsync())
                {
                    companies = await SeedCompaniesAsync(
                        authContext,
                        defaultTenantId,
                        systemUserId
                    );
                }
                else
                {
                    companies = await authContext.Set<Company>().ToListAsync();
                }
                if (identityUsers.Any() && companies.Any())
                {
                    bool needToSeedCompanyRoles = false;
                    foreach (var user in identityUsers)
                    {
                        if (
                            !await authContext
                                .Set<CompanyUserRole>()
                                .AnyAsync(cur =>
                                    cur.UserId == user.Id
                                    && companies.Select(c => c.CompanyId).Contains(cur.CompanyId)
                                )
                        )
                        {
                            needToSeedCompanyRoles = true;
                            break;
                        }
                    }
                    if (
                        needToSeedCompanyRoles
                        || !await authContext.Set<CompanyUserRole>().AnyAsync()
                    )
                    {
                        await SeedCompanyUserRolesAsync(
                            authContext,
                            identityUsers,
                            companies,
                            systemUserId
                        );
                    }
                    else
                    {
                        Console.WriteLine(
                            "CompanyUserRoles appear to be at least partially seeded for these users/companies."
                        );
                    }
                }
                else
                {
                    Console.WriteLine(
                        "Skipping CompanyUserRole seeding due to missing users or companies."
                    );
                }
            }
            catch (Exception ex)
            {
                Console.Error.WriteLine($"ERROR DURING SEEDING: {ex.ToString()}");
                throw;
            }
        }

        private static async Task SeedRolesAsync(RoleManager<IdentityRole> roleManager)
        {
            string[] roleNames = { "Administrator", "User", "Customer", "Employee" };
            foreach (var roleName in roleNames)
            {
                if (!await roleManager.RoleExistsAsync(roleName))
                {
                    var result = await roleManager.CreateAsync(new IdentityRole(roleName));
                    if (result.Succeeded)
                    {
                    }
                    else
                    {
                        Console.Error.WriteLine(
                            $"Failed to create role '{roleName}': {string.Join(", ", result.Errors.Select(e => e.Description))}"
                        );
                    }
                }
                else
                {
                }
            }
        }

        private static async Task<List<CustomPCIShieldUser>> SeedAndGetIdentityUsersAsync(
            UserManager<CustomPCIShieldUser> userManager,
            Guid tenantId,
            Guid systemUserId
        )
        {
            if (await userManager.Users.AnyAsync())
            {
                return await userManager.Users.ToListAsync();
            }
            var createdUsers = new List<CustomPCIShieldUser>();
            var adminAppUserId = Guid.NewGuid();
            var userAppUserId = Guid.NewGuid();
            var adminUser = new CustomPCIShieldUser
            {
                UserName = "admin@pciShield.com",
                Email = "admin@pciShield.com",
                EmailConfirmed = true,
                PhoneNumber = "1234567890",
                PhoneNumberConfirmed = true,
                ApplicationUserId = adminAppUserId,
                CreatedAt = DateTime.UtcNow,
                CreatedBy = systemUserId,
                FirstName = "Admin",
                LastName = "PCIShield",
                IsDeleted = false,
                TenantId = tenantId,
            };
            var adminResult = await userManager.CreateAsync(adminUser, "Password123!");
            if (adminResult.Succeeded)
            {
                await userManager.AddToRoleAsync(adminUser, "Administrator");
                createdUsers.Add(adminUser);
                Console.WriteLine(
                    $"User '{adminUser.UserName}' created and added to Administrator role."
                );
            }
            else
            {
                Console.Error.WriteLine(
                    $"Failed to create admin user '{adminUser.UserName}': {string.Join(", ", adminResult.Errors.Select(e => e.Description))}"
                );
            }
            var regularUser = new CustomPCIShieldUser
            {
                UserName = "user@pciShield.com",
                Email = "user@pciShield.com",
                EmailConfirmed = true,
                PhoneNumber = "0987654321",
                PhoneNumberConfirmed = true,
                ApplicationUserId = userAppUserId,
                CreatedAt = DateTime.UtcNow,
                CreatedBy = systemUserId,
                FirstName = "Regular",
                LastName = "User",
                IsDeleted = false,
                TenantId = tenantId,
            };
            var userResult = await userManager.CreateAsync(regularUser, "Password123!");
            if (userResult.Succeeded)
            {
                await userManager.AddToRolesAsync(regularUser, new[] { "User", "Customer" });
                createdUsers.Add(regularUser);
                Console.WriteLine(
                    $"User '{regularUser.UserName}' created and added to User and Customer roles."
                );
            }
            else
            {
                Console.Error.WriteLine(
                    $"Failed to create regular user '{regularUser.UserName}': {string.Join(", ", userResult.Errors.Select(e => e.Description))}"
                );
            }
            var adminUser2AppUserId = Guid.NewGuid();
            var adminUser2 = new CustomPCIShieldUser
            {
                UserName = "admin2@pciShield.com",
                Email = "admin2@pciShield.com",
                EmailConfirmed = true,
                PhoneNumber = "1234567890",
                PhoneNumberConfirmed = true,
                ApplicationUserId = adminUser2AppUserId,
                CreatedAt = DateTime.UtcNow,
                CreatedBy = systemUserId,
                FirstName = "AdminTwo",
                LastName = "PCIShield",
                IsDeleted = false,
                TenantId = tenantId,
            };
            var adminResult2 = await userManager.CreateAsync(adminUser2, "Password123!");
            if (adminResult2.Succeeded)
            {
                await userManager.AddToRoleAsync(adminUser2, "Administrator");
                createdUsers.Add(adminUser2);
                Console.WriteLine(
                    $"User '{adminUser2.UserName}' created and added to Administrator role."
                );
            }
            else
            {
                Console.Error.WriteLine(
                    $"Failed to create admin user 2 '{adminUser2.UserName}': {string.Join(", ", adminResult2.Errors.Select(e => e.Description))}"
                );
            }
            var regularUser2AppUserId = Guid.NewGuid();
            var regularUser2 = new CustomPCIShieldUser
            {
                UserName = "user2@pciShield.com",
                Email = "user2@pciShield.com",
                EmailConfirmed = true,
                PhoneNumber = "0987654321",
                PhoneNumberConfirmed = true,
                ApplicationUserId = regularUser2AppUserId,
                CreatedAt = DateTime.UtcNow,
                CreatedBy = systemUserId,
                FirstName = "RegularTwo",
                LastName = "User",
                IsDeleted = false,
                TenantId = tenantId,
            };
            var userResult2 = await userManager.CreateAsync(regularUser2, "Password123!");
            if (userResult2.Succeeded)
            {
                await userManager.AddToRoleAsync(regularUser2, "User");
                createdUsers.Add(regularUser2);
                Console.WriteLine(
                    $"User '{regularUser2.UserName}' created and added to User role."
                );
            }
            else
            {
                Console.Error.WriteLine(
                    $"Failed to create regular user 2 '{regularUser2.UserName}': {string.Join(", ", userResult2.Errors.Select(e => e.Description))}"
                );
            }
            return createdUsers;
        }

        private static async Task SeedDomainApplicationUsersAsync(
            AppDbContext appDbContext,
            IEnumerable<CustomPCIShieldUser> identityUsers,
            Guid systemUserId
        )
        {
            int newDomainUsersCount = 0;
            foreach (var identityUser in identityUsers)
            {
                if (
                    !await appDbContext.ApplicationUsers.AnyAsync(au =>
                        au.ApplicationUserId == identityUser.ApplicationUserId
                    )
                )
                {
                    Guid domainApplicationUserId = identityUser.ApplicationUserId;
                    string aspNetIdentityUserId = identityUser.Id;
                    string userName = identityUser.UserName;
                    string? firstName = identityUser.FirstName;
                    string? lastName = identityUser.LastName;
                    string? email = identityUser.Email;
                    string? phone = identityUser.PhoneNumber;
                    Guid createdBy = systemUserId;
                    Guid tenantId = identityUser.TenantId;
                    DateTime recordCreatedAt = DateTime.UtcNow;
                    DateTime businessCreatedDate = DateTime.UtcNow;
                    bool isLoginAllowed = true;
                    int failedLoginCount = 0;
                    bool isUserApproved = true;
                    bool isPhoneVerified = identityUser.PhoneNumberConfirmed;
                    bool domainIsEmailVerified = identityUser.EmailConfirmed;
                    bool isLocked = false;
                    bool isDeleted = false;
                    bool isUserFullyRegisteredParam = true;
                    bool isBanned = false;
                    bool isFullyRegisteredParam = true;
                    var domainUser = new PCIShield.Domain.Entities.ApplicationUser(
                        applicationUserId: domainApplicationUserId,
                        userName: userName,
                        createdDate: businessCreatedDate,
                        createdBy: createdBy,
                        isLoginAllowed: isLoginAllowed,
                        failedLoginCount: failedLoginCount,
                        isUserApproved: isUserApproved,
                        isPhoneVerified: isPhoneVerified,
                        isEmailVerified: domainIsEmailVerified,
                        isLocked: isLocked,
                        isDeleted: isDeleted,
                        isUserFullyRegistered: isUserFullyRegisteredParam,
                        isBanned: isBanned,
                        isFullyRegistered: isFullyRegisteredParam,
                        createdAt: recordCreatedAt,
                        tenantId: tenantId
                    );
                    var identityUserIdPropInfo = domainUser.GetType().GetProperty("IdentityUserId");
                    if (identityUserIdPropInfo != null && identityUserIdPropInfo.CanWrite)
                    {
                        identityUserIdPropInfo.SetValue(domainUser, aspNetIdentityUserId);
                    }
                    else
                    {
                        var setIdentityUserIdMethodInfo = domainUser
                            .GetType()
                            .GetMethod("SetIdentityUserId", new[] { typeof(string) });
                        if (setIdentityUserIdMethodInfo != null)
                        {
                            setIdentityUserIdMethodInfo.Invoke(
                                domainUser,
                                new object[] { aspNetIdentityUserId }
                            );
                        }
                        else
                        {
                            Console.Error.WriteLine(
                                $"CRITICAL WARNING: Domain.ApplicationUser for {userName} cannot set IdentityUserId. Add 'public string IdentityUserId {{ get; private set; }}' and 'public void SetIdentityUserId(string id) {{ IdentityUserId = id; }}' to the domain entity, then update its EF Core configuration and add a migration."
                            );
                        }
                    }
                    domainUser.SetFirstName(firstName);
                    domainUser.SetLastName(lastName);
                    if (identityUser.UserName == "user@pciShield.com")
                    {
                        domainUser.SetIsCustomer(true);
                        domainUser.SetIsEmployee(false);
                    }
                    else if (
                        identityUser.UserName == "admin@pciShield.com"
                        || identityUser.UserName == "admin2@pciShield.com"
                    )
                    {
                        domainUser.SetIsErpOwner(true);
                        domainUser.SetIsEmployee(true);
                    }
                    else if (identityUser.UserName == "user2@pciShield.com") { }
                    appDbContext.ApplicationUsers.Add(domainUser);
                    newDomainUsersCount++;
                    Console.WriteLine(
                        $"Preparing to add Domain ApplicationUser for IdentityUser: {identityUser.UserName} (DomainAppUID: {domainUser.ApplicationUserId})"
                    );
                }
            }
            if (newDomainUsersCount > 0)
            {
                await appDbContext.SaveChangesAsync();
                Console.WriteLine(
                    $"{newDomainUsersCount} new Domain ApplicationUsers added to AppDbContext."
                );
            }
            else
            {
                Console.WriteLine(
                    "No new Domain ApplicationUsers to add (they may already exist for the given IdentityUsers)."
                );
            }
        }

        private static async Task SeedAppLocalizationsAsync(
            AuthorizationDbContext context,
            Guid tenantId,
            Guid createdBy
        )
        {
            var localizations = new List<AppLocalization>
            {
                new AppLocalization
                {
                    Key = "Dashboard.Title",
                    Culture = "en-US",
                    Text = "Dashboard",
                    TenantId = tenantId,
                    CreatedAt = DateTime.UtcNow,
                    CreatedBy = createdBy,
                    IsDeleted = false,
                },
                new AppLocalization
                {
                    Key = "Dashboard.Title",
                    Culture = "es-ES",
                    Text = "Panel de Control",
                    TenantId = tenantId,
                    CreatedAt = DateTime.UtcNow,
                    CreatedBy = createdBy,
                    IsDeleted = false,
                },
                new AppLocalization
                {
                    Key = "Dashboard.WelcomeMessage",
                    Culture = "en-US",
                    Text = "Welcome to PCIShield",
                    TenantId = tenantId,
                    CreatedAt = DateTime.UtcNow,
                    CreatedBy = createdBy,
                    IsDeleted = false,
                },
                new AppLocalization
                {
                    Key = "Dashboard.WelcomeMessage",
                    Culture = "es-ES",
                    Text = "Bienvenido a PCIShield",
                    TenantId = tenantId,
                    CreatedAt = DateTime.UtcNow,
                    CreatedBy = createdBy,
                    IsDeleted = false,
                },
                new AppLocalization
                {
                    Key = "MudDataGrid.Filter",
                    Culture = "es-ES",
                    Text = "Filtro",
                    TenantId = tenantId,
                    CreatedAt = DateTime.UtcNow,
                    CreatedBy = createdBy,
                    IsDeleted = false,
                },
                new AppLocalization
                {
                    Key = "MudDataGrid.Apply",
                    Culture = "es-ES",
                    Text = "Aplicar",
                    TenantId = tenantId,
                    CreatedAt = DateTime.UtcNow,
                    CreatedBy = createdBy,
                    IsDeleted = false,
                },
                new AppLocalization
                {
                    Key = "MudDataGrid.Cancel",
                    Culture = "es-ES",
                    Text = "Cancelar",
                    TenantId = tenantId,
                    CreatedAt = DateTime.UtcNow,
                    CreatedBy = createdBy,
                    IsDeleted = false,
                },
                new AppLocalization
                {
                    Key = "MudDataGrid.Contains",
                    Culture = "es-ES",
                    Text = "Contiene",
                    TenantId = tenantId,
                    CreatedAt = DateTime.UtcNow,
                    CreatedBy = createdBy,
                    IsDeleted = false,
                },
                new AppLocalization
                {
                    Key = "MudDataGrid.Equals",
                    Culture = "es-ES",
                    Text = "Igual",
                    TenantId = tenantId,
                    CreatedAt = DateTime.UtcNow,
                    CreatedBy = createdBy,
                    IsDeleted = false,
                },
                new AppLocalization
                {
                    Key = "MudDataGrid.NotEquals",
                    Culture = "es-ES",
                    Text = "No Igual",
                    TenantId = tenantId,
                    CreatedAt = DateTime.UtcNow,
                    CreatedBy = createdBy,
                    IsDeleted = false,
                },
                new AppLocalization
                {
                    Key = "MudDataGrid.StartsWith",
                    Culture = "es-ES",
                    Text = "Comienza Con",
                    TenantId = tenantId,
                    CreatedAt = DateTime.UtcNow,
                    CreatedBy = createdBy,
                    IsDeleted = false,
                },
                new AppLocalization
                {
                    Key = "MudDataGrid.EndsWith",
                    Culture = "es-ES",
                    Text = "Termina Con",
                    TenantId = tenantId,
                    CreatedAt = DateTime.UtcNow,
                    CreatedBy = createdBy,
                    IsDeleted = false,
                },
                new AppLocalization
                {
                    Key = "MudDataGrid.Clear",
                    Culture = "es-ES",
                    Text = "Limpiar",
                    TenantId = tenantId,
                    CreatedAt = DateTime.UtcNow,
                    CreatedBy = createdBy,
                    IsDeleted = false,
                },
                new AppLocalization
                {
                    Key = "MudDataGrid.True",
                    Culture = "es-ES",
                    Text = "Verdadero",
                    TenantId = tenantId,
                    CreatedAt = DateTime.UtcNow,
                    CreatedBy = createdBy,
                    IsDeleted = false,
                },
                new AppLocalization
                {
                    Key = "MudDataGrid.False",
                    Culture = "es-ES",
                    Text = "Falso",
                    TenantId = tenantId,
                    CreatedAt = DateTime.UtcNow,
                    CreatedBy = createdBy,
                    IsDeleted = false,
                },
                new AppLocalization
                {
                    Key = "MudDataGrid.Clear",
                    Culture = "en-US",
                    Text = "Clear",
                    TenantId = tenantId,
                    CreatedAt = DateTime.UtcNow,
                    CreatedBy = createdBy,
                    IsDeleted = false,
                },
                new AppLocalization
                {
                    Key = "MudDataGrid.SortBy",
                    Culture = "en-US",
                    Text = "Sort by",
                    TenantId = tenantId,
                    CreatedAt = DateTime.UtcNow,
                    CreatedBy = createdBy,
                    IsDeleted = false,
                },
                new AppLocalization
                {
                    Key = "MudDataGrid.SortBy",
                    Culture = "es-ES",
                    Text = "Ordenar por",
                    TenantId = tenantId,
                    CreatedAt = DateTime.UtcNow,
                    CreatedBy = createdBy,
                    IsDeleted = false,
                },
                new AppLocalization
                {
                    Key = "Common.Loading",
                    Culture = "en-US",
                    Text = "Loading...",
                    TenantId = tenantId,
                    CreatedAt = DateTime.UtcNow,
                    CreatedBy = createdBy,
                    IsDeleted = false,
                },
                new AppLocalization
                {
                    Key = "Common.Loading",
                    Culture = "es-ES",
                    Text = "Cargando...",
                    TenantId = tenantId,
                    CreatedAt = DateTime.UtcNow,
                    CreatedBy = createdBy,
                    IsDeleted = false,
                },
                new AppLocalization
                {
                    Key = "Common.Save",
                    Culture = "en-US",
                    Text = "Save",
                    TenantId = tenantId,
                    CreatedAt = DateTime.UtcNow,
                    CreatedBy = createdBy,
                    IsDeleted = false,
                },
                new AppLocalization
                {
                    Key = "Common.Save",
                    Culture = "es-ES",
                    Text = "Guardar",
                    TenantId = tenantId,
                    CreatedAt = DateTime.UtcNow,
                    CreatedBy = createdBy,
                    IsDeleted = false,
                },
                new AppLocalization
                {
                    Key = "Common.Cancel",
                    Culture = "en-US",
                    Text = "Cancel",
                    TenantId = tenantId,
                    CreatedAt = DateTime.UtcNow,
                    CreatedBy = createdBy,
                    IsDeleted = false,
                },
                new AppLocalization
                {
                    Key = "Common.Cancel",
                    Culture = "es-ES",
                    Text = "Cancelar",
                    TenantId = tenantId,
                    CreatedAt = DateTime.UtcNow,
                    CreatedBy = createdBy,
                    IsDeleted = false,
                },
            };
            await context.AppLocalizations.AddRangeAsync(localizations);
            await context.SaveChangesAsync();
        }

        private static async Task<List<Company>> SeedCompaniesAsync(
            AuthorizationDbContext context,
            Guid tenantId,
            Guid systemUserId)
        {
            var companies = new List<Company>
            {
                new Company
                {
                    CompanyId = Guid.NewGuid(),
                    LogoUrl = "https://placeholder.com/logo/acme.png",
                    CompanyCode = "ACME001",
                    LegalEntityType = 1,
                    LegalName = "ACME Innovations Inc.",
                    TradeName = "ACME Solutions",
                    TaxIdentificationNumber = "US123456789",
                    BusinessActivityDescription = "Pioneering innovative solutions for a brighter future.",
                    NationalTaxRegistryCode = "ACMENAT1",
                    SocialSecurityCode = "ACMESOC1",
                    AddressStreetLine1 = "123 Innovation Drive",
                    AddressStreetLine2 = "Suite 100",
                    AddressPostalCode = "90210",
                     AddressStateProvinceId =  Guid.Parse("8085E136-B4D5-499F-A70D-848A723A51A5"),
                     AddressCountryId = Guid.Parse("94CDC961-D248-48AB-BDF8-57219A98B725"),
                    CompanyEmailAddress = "contact@acmeinnovations.com",
                    CompanyPhoneNumber = "+1-555-0100",
                    CompanyWebsiteUrl = "https://www.acmeinnovations.com",
                    CreatedDate = DateTime.UtcNow,
                    CreatedBy = systemUserId,
                    IsDeleted = false
                },
                new Company
                {
                    CompanyId = Guid.NewGuid(),
                    LogoUrl = "https://placeholder.com/logo/techsys.png",
                    CompanyCode = "TECHSYS002",
                    LegalEntityType = 1,
                    LegalName = "Tech Systems Global Ltd.",
                    TradeName = "TechSys",
                    TaxIdentificationNumber = "GB987654321",
                    BusinessActivityDescription = "Global provider of cutting-edge technology systems.",
                    NationalTaxRegistryCode = "TECHSYSNAT2",
                    SocialSecurityCode = "TECHSYSSOC2",
                    AddressStreetLine1 = "456 Enterprise Road",
                    AddressPostalCode = "SW1A 1AA",
                    AddressStreetLine2 = "Suite 100",
                    AddressStateProvinceId =  Guid.Parse("8085E136-B4D5-499F-A70D-848A723A51A5"),
                    AddressCountryId = Guid.Parse("94CDC961-D248-48AB-BDF8-57219A98B725"),
                    CompanyEmailAddress = "info@techsysglobal.com",
                    CompanyPhoneNumber = "+44-20-7946-0000",
                    CompanyWebsiteUrl = "https://www.techsysglobal.com",
                    CreatedDate = DateTime.UtcNow,
                    CreatedBy = systemUserId,
                    IsDeleted = false
                }
            };
            await context.Set<Company>().AddRangeAsync(companies);
            await context.SaveChangesAsync();
            return companies;
        }
        private static async Task SeedCompanyUserRolesAsync(
            AuthorizationDbContext context,
            List<CustomPCIShieldUser> users,
            List<Company> companies,
            Guid systemUserId)
        {
            if (!users.Any() || !companies.Any())
            {
                return;
            }

            var adminUser = users.FirstOrDefault(u => u.UserName == "admin@pciShield.com");
            var regularUser = users.FirstOrDefault(u => u.UserName == "user@pciShield.com");
            var rolesToAdd = new List<CompanyUserRole>();
            int count = 0;

            string assignerId = systemUserId.ToString();
            if (adminUser != null)
            {
                assignerId = adminUser.Id;
            }

            foreach (var company in companies)
            {
                if (adminUser != null)
                {
                    if (!await context.Set<CompanyUserRole>().AnyAsync(r => r.UserId == adminUser.Id && r.CompanyId == company.CompanyId && r.CompanySpecificRole == "Owner"))
                    {
                        rolesToAdd.Add(new CompanyUserRole
                        {
                            CompanyUserRoleId = Guid.NewGuid(),
                            CompanyId = company.CompanyId,
                            UserId = adminUser.Id,
                            CompanySpecificRole = "Owner",
                            AssignedDate = DateTime.UtcNow,
                            AssignedBy = assignerId,
                            IsActive = true,
                            IsDeleted = false,
                            DeletedBy = adminUser.Id,
                            DeletedDate = null
                        });
                        count++;
                    }
                }
                if (regularUser != null && company.CompanyCode == "ACME001")
                {
                    if (!await context.Set<CompanyUserRole>().AnyAsync(r => r.UserId == regularUser.Id && r.CompanyId == company.CompanyId && r.CompanySpecificRole == "Accountant"))
                    {
                        rolesToAdd.Add(new CompanyUserRole
                        {
                            CompanyUserRoleId = Guid.NewGuid(),
                            CompanyId = company.CompanyId,
                            UserId = regularUser.Id,
                            CompanySpecificRole = "Accountant",
                            AssignedDate = DateTime.UtcNow,
                            AssignedBy = assignerId,
                            IsActive = true,
                            IsDeleted = false,
                            DeletedBy = adminUser.Id,
                            DeletedDate = null
                        });
                        count++;
                    }
                }
            }

            if (rolesToAdd.Any())
            {
                await context.Set<CompanyUserRole>().AddRangeAsync(rolesToAdd);
                await context.SaveChangesAsync();
            }
        }
        private static async Task SeedPermissionsAsync(
            IServiceScope scope,
            AuthorizationDbContext authContext,
            RoleManager<IdentityRole> roleManager)
        {
            if (await authContext.Permissions.AnyAsync())
            {
                return;
            }
            var permissions = new List<Permission>
    {
        new() { Id = Guid.NewGuid(), Key = "users.view", DisplayName = "View Users", Category = "User Management", Description = "View user list and details", IsSystemPermission = true, CreatedAt = DateTimeOffset.UtcNow },
        new() { Id = Guid.NewGuid(), Key = "users.create", DisplayName = "Create Users", Category = "User Management", Description = "Create new user accounts", IsSystemPermission = true, CreatedAt = DateTimeOffset.UtcNow },
        new() { Id = Guid.NewGuid(), Key = "users.edit", DisplayName = "Edit Users", Category = "User Management", Description = "Modify existing user accounts", IsSystemPermission = true, CreatedAt = DateTimeOffset.UtcNow },
        new() { Id = Guid.NewGuid(), Key = "users.delete", DisplayName = "Delete Users", Category = "User Management", Description = "Delete user accounts permanently", IsSystemPermission = true, CreatedAt = DateTimeOffset.UtcNow },
        new() { Id = Guid.NewGuid(), Key = "users.lock", DisplayName = "Lock/Unlock Users", Category = "User Management", Description = "Lock or unlock user accounts", IsSystemPermission = true, CreatedAt = DateTimeOffset.UtcNow },
        new() { Id = Guid.NewGuid(), Key = "users.password.reset", DisplayName = "Reset User Passwords", Category = "User Management", Description = "Change passwords for other users", IsSystemPermission = true, CreatedAt = DateTimeOffset.UtcNow },
        new() { Id = Guid.NewGuid(), Key = "users.impersonate", DisplayName = "Impersonate Users", Category = "User Management", Description = "Login as another user for support purposes", IsSystemPermission = true, CreatedAt = DateTimeOffset.UtcNow },
        new() { Id = Guid.NewGuid(), Key = "roles.view", DisplayName = "View Roles", Category = "Role Management", Description = "View role list and details", IsSystemPermission = true, CreatedAt = DateTimeOffset.UtcNow },
        new() { Id = Guid.NewGuid(), Key = "roles.create", DisplayName = "Create Roles", Category = "Role Management", Description = "Create new roles", IsSystemPermission = true, CreatedAt = DateTimeOffset.UtcNow },
        new() { Id = Guid.NewGuid(), Key = "roles.edit", DisplayName = "Edit Roles", Category = "Role Management", Description = "Modify existing roles", IsSystemPermission = true, CreatedAt = DateTimeOffset.UtcNow },
        new() { Id = Guid.NewGuid(), Key = "roles.delete", DisplayName = "Delete Roles", Category = "Role Management", Description = "Delete custom roles", IsSystemPermission = true, CreatedAt = DateTimeOffset.UtcNow },
        new() { Id = Guid.NewGuid(), Key = "permissions.view", DisplayName = "View Permissions", Category = "Permission Management", Description = "View permission catalog", IsSystemPermission = true, CreatedAt = DateTimeOffset.UtcNow },
        new() { Id = Guid.NewGuid(), Key = "permissions.assign", DisplayName = "Assign Permissions", Category = "Permission Management", Description = "Grant or revoke permissions to roles/users", IsSystemPermission = true, CreatedAt = DateTimeOffset.UtcNow },
        new() { Id = Guid.NewGuid(), Key = "permissions.manage", DisplayName = "Manage Permissions", Category = "Permission Management", Description = "Create, edit, or delete custom permissions", IsSystemPermission = false, CreatedAt = DateTimeOffset.UtcNow },
        new() { Id = Guid.NewGuid(), Key = "audit.view", DisplayName = "View Audit Logs", Category = "Security & Audit", Description = "Access user activity and security audit logs", IsSystemPermission = true, CreatedAt = DateTimeOffset.UtcNow },
        new() { Id = Guid.NewGuid(), Key = "security.stamp.refresh", DisplayName = "Force User Logout", Category = "Security & Audit", Description = "Invalidate user sessions by refreshing security stamp", IsSystemPermission = true, CreatedAt = DateTimeOffset.UtcNow },
        new() { Id = Guid.NewGuid(), Key = "merchants.view", DisplayName = "View Merchants", Category = "Merchant Management", Description = "View merchant list and details", IsSystemPermission = false, CreatedAt = DateTimeOffset.UtcNow },
        new() { Id = Guid.NewGuid(), Key = "merchants.create", DisplayName = "Create Merchants", Category = "Merchant Management", Description = "Create new merchant accounts", IsSystemPermission = false, CreatedAt = DateTimeOffset.UtcNow },
        new() { Id = Guid.NewGuid(), Key = "merchants.edit", DisplayName = "Edit Merchants", Category = "Merchant Management", Description = "Modify merchant information", IsSystemPermission = false, CreatedAt = DateTimeOffset.UtcNow },
        new() { Id = Guid.NewGuid(), Key = "merchants.delete", DisplayName = "Delete Merchants", Category = "Merchant Management", Description = "Delete merchant accounts", IsSystemPermission = false, CreatedAt = DateTimeOffset.UtcNow },
        new() { Id = Guid.NewGuid(), Key = "merchants.approve", DisplayName = "Approve Merchants", Category = "Merchant Management", Description = "Approve or reject merchant applications", IsSystemPermission = false, CreatedAt = DateTimeOffset.UtcNow },
        new() { Id = Guid.NewGuid(), Key = "companies.view", DisplayName = "View Companies", Category = "Company Management", Description = "View company list and details", IsSystemPermission = false, CreatedAt = DateTimeOffset.UtcNow },
        new() { Id = Guid.NewGuid(), Key = "companies.create", DisplayName = "Create Companies", Category = "Company Management", Description = "Create new companies", IsSystemPermission = false, CreatedAt = DateTimeOffset.UtcNow },
        new() { Id = Guid.NewGuid(), Key = "companies.edit", DisplayName = "Edit Companies", Category = "Company Management", Description = "Modify company information", IsSystemPermission = false, CreatedAt = DateTimeOffset.UtcNow },
        new() { Id = Guid.NewGuid(), Key = "companies.delete", DisplayName = "Delete Companies", Category = "Company Management", Description = "Delete companies", IsSystemPermission = false, CreatedAt = DateTimeOffset.UtcNow },
        new() { Id = Guid.NewGuid(), Key = "reports.view", DisplayName = "View Reports", Category = "Reports & Analytics", Description = "Access standard reports", IsSystemPermission = false, CreatedAt = DateTimeOffset.UtcNow },
        new() { Id = Guid.NewGuid(), Key = "reports.export", DisplayName = "Export Reports", Category = "Reports & Analytics", Description = "Export reports to CSV/Excel", IsSystemPermission = false, CreatedAt = DateTimeOffset.UtcNow },
        new() { Id = Guid.NewGuid(), Key = "reports.financial", DisplayName = "View Financial Reports", Category = "Reports & Analytics", Description = "Access sensitive financial data", IsSystemPermission = false, CreatedAt = DateTimeOffset.UtcNow },
        new() { Id = Guid.NewGuid(), Key = "system.settings", DisplayName = "Manage System Settings", Category = "System Administration", Description = "Configure system-wide settings", IsSystemPermission = true, CreatedAt = DateTimeOffset.UtcNow },
        new() { Id = Guid.NewGuid(), Key = "system.maintenance", DisplayName = "System Maintenance", Category = "System Administration", Description = "Perform system maintenance tasks", IsSystemPermission = true, CreatedAt = DateTimeOffset.UtcNow },
    };
            await authContext.Permissions.AddRangeAsync(permissions);
            await authContext.SaveChangesAsync();
            var adminRole = await roleManager.FindByNameAsync("Administrator");
            var userRole = await roleManager.FindByNameAsync("User");
            var employeeRole = await roleManager.FindByNameAsync("Employee");
            var customerRole = await roleManager.FindByNameAsync("Customer");

            var rolePermissions = new List<RolePermission>();

            if (adminRole != null)
            {
                rolePermissions.AddRange(permissions.Select(p => new RolePermission
                {
                    Id = Guid.NewGuid(),
                    RoleId = adminRole.Id,
                    PermissionId = p.Id,
                    GrantedAt = DateTimeOffset.UtcNow,
                    GrantedBy = "System"
                }));
            }

            if (employeeRole != null)
            {
                var employeePermissionKeys = new[]
                {
            "users.view", "merchants.view", "merchants.create", "merchants.edit",
            "companies.view", "reports.view", "reports.export"
        };
                rolePermissions.AddRange(permissions
                    .Where(p => employeePermissionKeys.Contains(p.Key))
                    .Select(p => new RolePermission
                    {
                        Id = Guid.NewGuid(),
                        RoleId = employeeRole.Id,
                        PermissionId = p.Id,
                        GrantedAt = DateTimeOffset.UtcNow,
                        GrantedBy = "System"
                    }));
            }

            if (customerRole != null)
            {
                var customerPermissionKeys = new[] { "merchants.view", "reports.view" };
                rolePermissions.AddRange(permissions
                    .Where(p => customerPermissionKeys.Contains(p.Key))
                    .Select(p => new RolePermission
                    {
                        Id = Guid.NewGuid(),
                        RoleId = customerRole.Id,
                        PermissionId = p.Id,
                        GrantedAt = DateTimeOffset.UtcNow,
                        GrantedBy = "System"
                    }));
            }

            if (userRole != null)
            {
                var userPermissionKeys = new[] { "reports.view" };
                rolePermissions.AddRange(permissions
                    .Where(p => userPermissionKeys.Contains(p.Key))
                    .Select(p => new RolePermission
                    {
                        Id = Guid.NewGuid(),
                        RoleId = userRole.Id,
                        PermissionId = p.Id,
                        GrantedAt = DateTimeOffset.UtcNow,
                        GrantedBy = "System"
                    }));
            }

            await authContext.RolePermissions.AddRangeAsync(rolePermissions);
            await authContext.SaveChangesAsync();
        }

    }
}
