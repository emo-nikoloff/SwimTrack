using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.DependencyInjection;

using SwimTrack.Data.Seeding.Utilities;

namespace SwimTrack.Web.Infrastructure.Extensions;

public static class WebApplicationExtension
{
    public static void UseDefaultRoles(this WebApplication app)
    {
        using IServiceScope serviceScope = app.Services.CreateScope();
        IServiceProvider serviceProvider = serviceScope.ServiceProvider;

        IdentitySeeder.SeedRoles(serviceProvider);
    }
}
