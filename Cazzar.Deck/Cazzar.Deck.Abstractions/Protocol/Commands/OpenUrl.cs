namespace Cazzar.Deck.Abstractions.Protocol.Commands;

[RequiresFeature(DeckFeature.OpenUrl)]
public sealed record OpenUrl(Uri Url) : IDeckCommand;
