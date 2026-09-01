namespace Cazzar.Deck.Abstractions.Protocol;

public interface IDeckHostInfo
{
    DeckHost Id { get; }
    string Name { get; }
    DeckFeature Features { get; }
}
