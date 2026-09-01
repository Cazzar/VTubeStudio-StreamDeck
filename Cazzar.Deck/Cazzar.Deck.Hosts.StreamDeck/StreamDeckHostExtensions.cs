using Cazzar.Deck.Core;
using Cazzar.Deck.Core.Protocol;
using Cazzar.Deck.Abstractions.Protocol;
using Cazzar.Deck.Abstractions.Surfaces;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;

namespace Cazzar.Deck.Hosts.StreamDeck;

public static class StreamDeckHostExtensions
{
    private const string Section = "StreamDeck";

    public static IServiceCollection AddStreamDeckHost(this IServiceCollection services, IConfiguration configuration)
    {
        services.AddDeckLaunchOptions(configuration, Section);

        services.TryAddSingleton<IDeckHostInfo, StreamDeckHostInfo>();
        services.TryAddSingleton<IDeckCodec, StreamDeckCodec>();
        services.TryAddSingleton<IDeckHandshake, DeckHandshake>();

        services.TryAddSingleton<IEncoderSurface>(s => s.GetRequiredService<DeckClient>());

        return services;
    }

    public static IConfigurationBuilder AddStreamDeckCommandLine(this IConfigurationBuilder configuration, string[] args) =>
        configuration.AddCommandLine(args, new Dictionary<string, string>
        {
            ["-port"] = $"{Section}:Port",
            ["-pluginUUID"] = $"{Section}:Uuid",
            ["-registerEvent"] = $"{Section}:RegisterEvent",
            ["-info"] = $"{Section}:Info",
        });
}
