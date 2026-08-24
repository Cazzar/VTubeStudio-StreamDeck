using Cazzar.Deck.Abstractions.Actions.Handlers;
using Cazzar.Deck.Abstractions.Actions;
using Cazzar.Deck.Core.Actions;
using Cazzar.Deck.Core;
using Cazzar.Deck.VTubeStudio.Actions.Movement;
using Cazzar.Deck.VTubeStudio.Caches;
using Microsoft.Extensions.DependencyInjection.Extensions;
using Microsoft.Extensions.DependencyInjection;
using VTubeStudio.Api;

namespace Cazzar.Deck.VTubeStudio;

public static class DependencyInjection
{
    public static IServiceCollection AddVTubeStudio(this IServiceCollection services)
    {
        DeckJson.AddContext(VtsDeckJsonContext.Default);
        PropertyViewCommandRouter.Use(GeneratedPropertyViewCommands.Dispatch);

        services.TryAddSingleton<VtsSettingsStore>();
        services.TryAddSingleton<IVtsSettingsStore>(s => s.GetRequiredService<VtsSettingsStore>());
        services.AddSingleton<IGlobalSettingsHandler>(s => s.GetRequiredService<VtsSettingsStore>());

        services.AddVTubeStudioApi();

        services.TryAddSingleton<ModelCache>();
        services.TryAddSingleton<HotkeyCache>();
        services.TryAddSingleton<ExpressionCache>();
        services.TryAddSingleton<ModelPositionTracker>();

        services.AddSingleton<IActionProvider, GeneratedActionProvider>();
        services.AddHostedService<CacheWarmup>();

        return services;
    }
}
