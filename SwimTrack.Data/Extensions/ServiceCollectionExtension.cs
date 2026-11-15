using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

using SwimTrack.Data.Contracts;
using SwimTrack.Data.Repository;
using SwimTrack.Data.Repository.Contracts;

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
        services.AddScoped<ITrainingPlanRepository, TrainingPlanRepository>();
        services.AddScoped<IWorkoutRepository, WorkoutRepository>();
        services.AddScoped<IWorkoutResultRepository, WorkoutResultRepository>();

        services.AddScoped<IUnitOfWork, UnitOfWork>();

        return services;
    }
}
