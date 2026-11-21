using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

using SwimTrack.Data;

namespace SwimTrack.Web.Infrastructure.Extensions;

public static class ServiceCollectionExtension
{
    public static IServiceCollection AddApplicationIdentity(this IServiceCollection services, IConfiguration configuration)
    {
        services.AddDefaultIdentity<IdentityUser>(options =>
        {
            configuration.GetSection("IdentitySettings").Bind(options);
        })
        .AddEntityFrameworkStores<SwimTrackDbContext>();

        return services;
    }
}
