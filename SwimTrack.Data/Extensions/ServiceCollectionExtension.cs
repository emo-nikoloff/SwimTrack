using System.Reflection;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

using SwimTrack.Data.Contracts;

namespace SwimTrack.Data.Extensions;

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

    public static IServiceCollection AddApplicationRepositories(this IServiceCollection services)
    {
        Assembly assembly = Assembly.GetExecutingAssembly();

        IEnumerable<Type> repositoryTypes = assembly.GetExportedTypes()
            .Where(t =>
                t.IsClass
                && !t.IsAbstract
                && t.Name.EndsWith("Repository"));

        foreach (Type repositoryType in repositoryTypes)
        {
            Type? matchingInterface = repositoryType.GetInterface($"I{repositoryType.Name}");
            if (matchingInterface != null)
            {
                services.AddScoped(matchingInterface, repositoryType);
            }
        }

        services.AddScoped<IUnitOfWork, UnitOfWork>();

        return services;
    }
}
