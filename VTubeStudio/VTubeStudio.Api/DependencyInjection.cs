using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;

namespace VTubeStudio.Api;

public static class DependencyInjection
{
    public static IServiceCollection AddVTubeStudioApi(this IServiceCollection services)
    {
        services.AddOptions<VtsConnectionOptions>();

        services.TryAddSingleton<VTubeStudioClient>();
        services.TryAddSingleton<IVTubeStudio>(s => s.GetRequiredService<VTubeStudioClient>());
        services.AddHostedService<VTubeStudioConnection>();

        return services;
    }
}
