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
}
