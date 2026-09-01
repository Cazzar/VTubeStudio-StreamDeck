using System.Globalization;
using System.Reflection;
using Cazzar.Deck.Abstractions.Actions;
using Cazzar.Deck.Core.Actions;
using Cazzar.Deck.Core.Protocol;
using Cazzar.Deck.Abstractions.Protocol;
using Cazzar.Deck.Abstractions.Surfaces;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection.Extensions;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;

namespace Cazzar.Deck.Core;

public static class DependencyInjection
{
    public static IServiceCollection AddDeckCore(this IServiceCollection services)
    {
        services.TryAddSingleton<IDeckTransport, WebSocketTransport>();

        services.TryAddSingleton(_ => EntryAssemblyPackageInfo.Read());

        services.TryAddSingleton<DeckClient>();
        services.TryAddSingleton<IWidgetSurface>(s => s.GetRequiredService<DeckClient>());
        services.TryAddSingleton<ISettingsStore>(s => s.GetRequiredService<DeckClient>());
        services.TryAddSingleton<IPropertyViewChannel>(s => s.GetRequiredService<DeckClient>());

        services.TryAddSingleton<ActionCatalog>();
        services.TryAddSingleton<ActionInstances>();
        services.TryAddSingleton<DeckEventDispatcher>();
        services.TryAddSingleton<DeckActionContext>();

        services.AddOptions<ActionTimerOptions>();

        services.AddHostedService<DeckSession>();
        services.AddHostedService<ActionTimer>();

        return services;
    }

    public static IServiceCollection AddDeckLaunchOptions(this IServiceCollection services, IConfiguration configuration, string sectionName)
    {
        var section = configuration.GetSection(sectionName);

        services.Configure<DeckLaunchOptions>(options =>
        {
            options.Port = int.TryParse(section["Port"], CultureInfo.InvariantCulture, out var port) ? port : 0;
            options.Uuid = section["Uuid"] ?? string.Empty;
            options.RegisterEvent = section["RegisterEvent"] ?? string.Empty;
            options.Info = section["Info"];
        });

        services.TryAddSingleton<IDeckLaunchOptions>(s => s.GetRequiredService<IOptions<DeckLaunchOptions>>().Value);

        return services;
    }

    public static IServiceCollection AddDeckActions(this IServiceCollection services, params Assembly[] assemblies)
    {
        services.AddSingleton<IActionProvider>(new ReflectionActionProvider(assemblies));
        return services;
    }

    public static IServiceCollection AddDeckActionsFrom(this IServiceCollection services, string directory, string pattern = "*.Deck.dll")
    {
        if (!Directory.Exists(directory)) return services;

        var assemblies = Directory.EnumerateFiles(directory, pattern)
            .Select(TryLoad)
            .OfType<Assembly>()
            .ToArray();

        return assemblies.Length == 0 ? services : services.AddDeckActions(assemblies);

        static Assembly? TryLoad(string path)
        {
            try
            {
                return Assembly.LoadFrom(path);
            }
            catch (BadImageFormatException)
            {
                return null;
            }
        }
    }
}
