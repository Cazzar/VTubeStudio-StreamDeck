using Cazzar.Deck.VTubeStudio;
using VTubeStudio.Api;

namespace VTubeStudio.CreatorCentral;

public sealed class CreatorCentralPluginInfo : IVtsPluginInfo
{
    public string Name => "Creator Central Integration";
    public string Developer => "Cazzar";
    public string? Icon { get; } = EmbeddedIcon.Read(typeof(CreatorCentralPluginInfo).Assembly, "vts_plugin_icon.png");
}
