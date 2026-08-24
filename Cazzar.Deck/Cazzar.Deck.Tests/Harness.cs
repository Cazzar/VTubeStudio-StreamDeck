using Cazzar.Deck.Abstractions;
using Cazzar.Deck.Core.Actions;
using Cazzar.Deck.Core.Protocol;
using Cazzar.Deck.Core;
using Cazzar.Deck.Hosts.Loopback;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging.Abstractions;
using Microsoft.Extensions.Logging;

namespace Cazzar.Deck.Tests;

public sealed class Harness : IAsyncDisposable
{
    private readonly ServiceProvider _services;

    public Harness(DeckHost host, DeckFeature features = DeckFeature.All)
    {
        DeckJson.AddContext(TestJsonContext.Default);
        PropertyViewCommandRouter.Use(GeneratedPropertyViewCommands.Dispatch);

        var services = new ServiceCollection();
        services.AddSingleton<ILoggerFactory>(NullLoggerFactory.Instance);
        services.AddSingleton(typeof(ILogger<>), typeof(NullLogger<>));
        services.AddLoopbackHost(host, features);
        services.AddDeckCore();
        services.AddDeckActions(typeof(Harness).Assembly);

        _services = services.BuildServiceProvider();

        Catalog = _services.GetRequiredService<ActionCatalog>();
        Catalog.Load();

        Instances = _services.GetRequiredService<ActionInstances>();
        Dispatcher = _services.GetRequiredService<DeckEventDispatcher>();
        Codec = _services.GetRequiredService<LoopbackCodec>();
        Client = _services.GetRequiredService<DeckClient>();
        Faults = _services.GetRequiredService<RecordedFaults>();
    }

    public ActionCatalog Catalog { get; }
    public ActionInstances Instances { get; }
    public DeckEventDispatcher Dispatcher { get; }
    public LoopbackCodec Codec { get; }
    public DeckClient Client { get; }
    public RecordedFaults Faults { get; }

    public ValueTask DisposeAsync() => _services.DisposeAsync();
}
