using Cazzar.Deck.Core;
using Cazzar.Deck.Core.Protocol;
using Cazzar.Deck.Abstractions.Protocol;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;

namespace Cazzar.Deck.Hosts.CreatorCentral;

public static class CreatorCentralHostExtensions
{
    private const string Section = "CreatorCentral";

    public static IServiceCollection AddCreatorCentralHost(this IServiceCollection services, IConfiguration configuration)
    {
        services.AddDeckLaunchOptions(configuration, Section);

        services.TryAddSingleton<IDeckHostInfo, CreatorCentralHostInfo>();
        services.TryAddSingleton<IDeckCodec, CreatorCentralCodec>();
        services.TryAddSingleton<IDeckHandshake, DeckHandshake>();

        return services;
    }

    public static IConfigurationBuilder AddCreatorCentralCommandLine(this IConfigurationBuilder configuration, string[] args) =>
        configuration.AddCommandLine(args, new Dictionary<string, string>
        {
            ["-port"] = $"{Section}:Port",
            ["-packageUUID"] = $"{Section}:Uuid",
            ["-registerEvent"] = $"{Section}:RegisterEvent",
            ["-info"] = $"{Section}:Info",
        });
}
