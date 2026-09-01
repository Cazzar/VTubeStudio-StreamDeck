using Cazzar.Deck.Abstractions.Protocol;

namespace Cazzar.Deck.Core.Protocol;

public sealed class DeckLaunchOptions : IDeckLaunchOptions
{
    public int Port { get; set; }
    public string Uuid { get; set; } = string.Empty;
    public string RegisterEvent { get; set; } = string.Empty;
    public string? Info { get; set; }
}
