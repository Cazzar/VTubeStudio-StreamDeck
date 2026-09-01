namespace Cazzar.Deck.Abstractions;

[Flags]
public enum DeckHost
{
    None = 0,
    StreamDeck = 1,
    CreatorCentral = 2,
    All = StreamDeck | CreatorCentral,
}
