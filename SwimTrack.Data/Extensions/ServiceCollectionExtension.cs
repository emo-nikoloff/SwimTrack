using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;

using SwimTrack.Data;

namespace Microsoft.Extensions.DependencyInjection;

public static class ServiceCollectionExtension
{
    public static IServiceCollection AddApplicationDbContext(this IServiceCollection services, IConfiguration configuration)
    {
        string connectionString = configuration.GetConnectionString("DefaultConnection")
            ?? throw new InvalidOperationException("Connection string is not configured properly!");

        services.AddDbContext<SwimTrackDbContext>(options =>
        {
            options.UseSqlServer(connectionString);
        });

        return services;
    }
}
