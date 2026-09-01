namespace Cazzar.Deck.Abstractions.Protocol.Commands;

[RequiresFeature(DeckFeature.HostLog)]
public sealed record SendLog(string Message) : IDeckCommand;
