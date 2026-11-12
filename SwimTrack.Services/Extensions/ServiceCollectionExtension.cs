using Microsoft.Extensions.DependencyInjection;

using SwimTrack.Services.Contracts;
using SwimTrack.Services.Services;

namespace SwimTrack.Services.Extensions;

public static class ServiceCollectionExtension
{
    public static IServiceCollection AddApplicationServices(this IServiceCollection services)
    {
        services.AddScoped<ITrainingPlanService, TrainingPlanService>();
        services.AddScoped<IWorkoutService, WorkoutService>();

        return services;
    }
}
