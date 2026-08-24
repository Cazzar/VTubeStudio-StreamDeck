using Cazzar.Deck.Core.Actions;
using Cazzar.Deck.Core.Protocol;
using Cazzar.Deck.Core;
using Cazzar.Deck.Hosts.Loopback;
using Cazzar.Deck.Abstractions.Protocol;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging.Abstractions;
using Microsoft.Extensions.Logging;
using VTubeStudio.Api;

namespace Cazzar.Deck.VTubeStudio.Tests;

public sealed class PluginHarness : IAsyncDisposable
{
    private readonly ServiceProvider _services;

    public PluginHarness(IDeckHostInfo host)
    {
        var services = new ServiceCollection();
        services.AddSingleton<ILoggerFactory>(NullLoggerFactory.Instance);
        services.AddSingleton(typeof(ILogger<>), typeof(NullLogger<>));
        services.AddLoopbackHost(host.Id, host.Features);
        services.AddSingleton<IDeckPackageInfo>(
            new DeckPackageInfo(EntryAssemblyPackageInfo.Uuid(typeof(PluginHarness).Assembly)));
        services.AddDeckCore();
        services.AddSingleton<IVtsPluginInfo, TestPluginInfo>();
        services.AddSingleton<IVTubeStudio>(Vts);
        services.AddVTubeStudio();

        _services = services.BuildServiceProvider();

        Catalog = _services.GetRequiredService<ActionCatalog>();
        Catalog.Load();

        Dispatcher = _services.GetRequiredService<DeckEventDispatcher>();
        Faults = _services.GetRequiredService<RecordedFaults>();
    }

    public FakeVTubeStudio Vts { get; } = new();

    public ActionCatalog Catalog { get; }
    public DeckEventDispatcher Dispatcher { get; }
    public RecordedFaults Faults { get; }

    public ValueTask DisposeAsync() => _services.DisposeAsync();
}
