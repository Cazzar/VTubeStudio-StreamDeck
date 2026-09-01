using Cazzar.Deck.Abstractions.Protocol;
using Cazzar.Deck.Abstractions;

namespace Cazzar.Deck.Hosts.StreamDeck;

public sealed class StreamDeckHostInfo : IDeckHostInfo
{
    public DeckHost Id => DeckHost.StreamDeck;
    public string Name => "Stream Deck";

    public DeckFeature Features =>
        DeckFeature.Encoder |
        DeckFeature.Touchscreen |
        DeckFeature.MultiState |
        DeckFeature.GlobalSettings |
        DeckFeature.DeviceEvents |
        DeckFeature.ApplicationEvents |
        DeckFeature.OpenUrl |
        DeckFeature.Alerts;
}
