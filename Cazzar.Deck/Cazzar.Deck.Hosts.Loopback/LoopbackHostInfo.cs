using Cazzar.Deck.Abstractions.Protocol;
using Cazzar.Deck.Abstractions;

namespace Cazzar.Deck.Hosts.Loopback;

public sealed class LoopbackHostInfo(DeckHost id, DeckFeature features) : IDeckHostInfo
{
    public DeckHost Id => id;
    public string Name => $"Loopback ({id})";
    public DeckFeature Features => features;
}
