using Cazzar.Deck.Abstractions.Protocol;
using Cazzar.Deck.Abstractions;

namespace Cazzar.Deck.Hosts.CreatorCentral;

public sealed class CreatorCentralHostInfo : IDeckHostInfo
{
    public DeckHost Id => DeckHost.CreatorCentral;
    public string Name => "Creator Central";

    public DeckFeature Features => DeckFeature.MultiState | DeckFeature.GlobalSettings | DeckFeature.HostLog | DeckFeature.OpenUrl;
}
