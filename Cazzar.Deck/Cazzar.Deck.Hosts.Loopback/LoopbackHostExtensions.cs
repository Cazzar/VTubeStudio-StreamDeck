using Cazzar.Deck.Abstractions.Actions.Handlers;
using Cazzar.Deck.Abstractions.Protocol;
using Cazzar.Deck.Abstractions.Surfaces;
using Cazzar.Deck.Abstractions;
using Cazzar.Deck.Core.Protocol;
using Microsoft.Extensions.DependencyInjection.Extensions;
using Microsoft.Extensions.DependencyInjection;

namespace Cazzar.Deck.Hosts.Loopback;

public static class LoopbackHostExtensions
{
    public static IServiceCollection AddLoopbackHost(
        this IServiceCollection services,
        DeckHost id = DeckHost.StreamDeck,
        DeckFeature features = DeckFeature.All)
    {
        var info = new LoopbackHostInfo(id, features);

        services.TryAddSingleton<IDeckLaunchOptions>(new LoopbackLaunchOptions());
        services.TryAddSingleton<IDeckHostInfo>(info);
        services.TryAddSingleton<LoopbackTransport>();
        services.TryAddSingleton<IDeckTransport>(s => s.GetRequiredService<LoopbackTransport>());
        services.TryAddSingleton<LoopbackCodec>();
        services.TryAddSingleton<IDeckCodec>(s => s.GetRequiredService<LoopbackCodec>());
        services.TryAddSingleton<IDeckHandshake, LoopbackHandshake>();

        services.TryAddSingleton<RecordedFaults>();
        services.AddSingleton<IActionFaultObserver>(s => s.GetRequiredService<RecordedFaults>());

        if (info.Has(DeckFeature.Encoder))
            services.TryAddSingleton<IEncoderSurface>(s => s.GetRequiredService<DeckClient>());

        return services;
    }
}
