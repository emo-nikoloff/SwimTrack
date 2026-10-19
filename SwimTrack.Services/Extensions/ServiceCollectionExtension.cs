using SwimTrack.Services.Contracts;
using SwimTrack.Services.Services;

namespace Microsoft.Extensions.DependencyInjection;

public static class ServiceCollectionExtension
{
    public static IServiceCollection AddApplicationServices(this IServiceCollection services)
    {
        services.AddScoped<ITrainingPlanService, TrainingPlanService>();
        services.AddScoped<IWorkoutService, WorkoutService>();

        return services;
    }
}
