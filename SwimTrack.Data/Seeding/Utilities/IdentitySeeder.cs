using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.DependencyInjection;

using static SwimTrack.Shared.Constants.ApplicationDefaults.Identity.Role;
using static SwimTrack.Shared.Constants.ValidationMessages.Identity;

namespace SwimTrack.Data.Seeding.Utilities;

public static class IdentitySeeder
{
    public static readonly IReadOnlyCollection<string> DefaultRoles = new[] {
        User,
        Manager,
        Admin };

    public static void SeedRoles(IServiceProvider serviceProvider)
    {
        RoleManager<IdentityRole> roleManager = serviceProvider
            .GetRequiredService<RoleManager<IdentityRole>>();
        foreach (string roleName in DefaultRoles)
        {
            bool roleExists = roleManager.RoleExistsAsync(roleName)
                .GetAwaiter()
                .GetResult();
            if (roleExists)
            {
                continue;
            }

            IdentityRole identityRole = new IdentityRole(roleName);
            IdentityResult result = roleManager.CreateAsync(identityRole)
                .GetAwaiter()
                .GetResult();
            if (!result.Succeeded)
            {
                throw new InvalidOperationException(string.Format(
                    InvalidIdentityRoleOperation,
                    roleName
                ));
            }
        }
    }

    public static void SeedAdminUser(IServiceProvider serviceProvider, string adminEmail, string adminPassword)
    {
        UserManager<IdentityUser> userManager = serviceProvider
            .GetRequiredService<UserManager<IdentityUser>>();

        IdentityUser? adminUser = userManager.FindByEmailAsync(adminEmail)
            .GetAwaiter()
            .GetResult();
        if (adminUser != null)
        {
            return;
        }

        adminUser = new IdentityUser(adminEmail)
        {
            Email = adminEmail
        };

        IdentityResult result = userManager.CreateAsync(adminUser, adminPassword)
            .GetAwaiter()
            .GetResult();
        if (!result.Succeeded)
        {
            throw new InvalidOperationException(string.Format(
                InvalidAdminUserSeedOperation,
                adminEmail
            ));
        }

        result = userManager.AddToRoleAsync(adminUser, Admin)
            .GetAwaiter()
            .GetResult();
        if (!result.Succeeded)
        {
            throw new InvalidOperationException(string.Format(
                InvalidAdminUserRoleOperation,
                adminEmail,
                Admin
            ));
        }
    }
}
