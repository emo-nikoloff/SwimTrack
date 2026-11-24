using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.Configuration;
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

    public static void UseAdminSeeding(this WebApplication app)
    {
        using IServiceScope serviceScope = app.Services.CreateScope();
        IServiceProvider serviceProvider = serviceScope.ServiceProvider;

        string adminEmail = app.Configuration.GetValue<string>("IdentitySeed:AdminAccount:Email")
            ?? throw new ArgumentException();
        string adminPassword = app.Configuration.GetValue<string>("IdentitySeed:AdminAccount:Password")
            ?? throw new ArgumentException();

        IdentitySeeder.SeedAdminUser(serviceProvider, adminEmail, adminPassword);
    }
}
