using VTubeStudio.Api;

namespace Cazzar.Deck.VTubeStudio.Tests;

public sealed class TestPluginInfo : IVtsPluginInfo
{
    public string Name => "Test Harness";
    public string Developer => "Cazzar";
    public string? Icon => null;
}
