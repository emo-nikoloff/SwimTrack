using Microsoft.AspNetCore.Identity;

using SwimTrack.Data;
using static SwimTrack.Shared.Constants.ApplicationDefaults.Identity;

namespace Microsoft.Extensions.DependencyInjection;

public static class ServiceCollectionExtension
{
    public static IServiceCollection AddApplicationIdentity(this IServiceCollection services)
    {
        services.AddDefaultIdentity<IdentityUser>(options =>
        {
            // Caution: Dev only options
            // TODO: Use appsettings.json to store different options depending on the app environment
            options.User.RequireUniqueEmail = true;

            options.Password.RequireDigit = false;
            options.Password.RequireNonAlphanumeric = false;
            options.Password.RequireUppercase = false;
            options.Password.RequiredLength = PasswordMinimumLength;

            options.SignIn.RequireConfirmedAccount = false;
        })
        .AddEntityFrameworkStores<SwimTrackDbContext>();

        return services;
    }
}
