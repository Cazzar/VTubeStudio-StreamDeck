using VTubeStudio.Api.Models;

namespace Cazzar.Deck.VTubeStudio.Caches;

public sealed class HotkeyCacheUpdatedEventArgs(IReadOnlyDictionary<string, IReadOnlyList<Hotkey>> hotkeys) : EventArgs
{
    public IReadOnlyDictionary<string, IReadOnlyList<Hotkey>> Hotkeys { get; } = hotkeys;
}
