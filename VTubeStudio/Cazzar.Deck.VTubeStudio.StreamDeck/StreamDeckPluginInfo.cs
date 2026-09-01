using Cazzar.Deck.VTubeStudio;
using VTubeStudio.Api;

namespace VTubeStudio.StreamDeck;

public sealed class StreamDeckPluginInfo : IVtsPluginInfo
{
    public string Name => "StreamDeck Integration";
    public string Developer => "Cazzar";
    public string? Icon { get; } = EmbeddedIcon.Read(typeof(StreamDeckPluginInfo).Assembly, "vts_plugin_icon.png");
}
