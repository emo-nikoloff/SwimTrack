using System.Reflection;
using Microsoft.Extensions.DependencyInjection;

namespace SwimTrack.Services.Extensions;

public static class ServiceCollectionExtension
{
    public static IServiceCollection AddApplicationServices(this IServiceCollection services)
    {
        Assembly assembly = Assembly.GetExecutingAssembly();

        IEnumerable<Type> serviceTypes = assembly.GetExportedTypes()
            .Where(t =>
                t.IsClass
                && !t.IsAbstract
                && t.Name.EndsWith("Service"));

        foreach (Type serviceType in serviceTypes)
        {
            Type? matchingInterface = serviceType.GetInterface($"I{serviceType.Name}");
            if (matchingInterface != null)
            {
                services.AddScoped(matchingInterface, serviceType);
            }
        }

        return services;
    }
}
