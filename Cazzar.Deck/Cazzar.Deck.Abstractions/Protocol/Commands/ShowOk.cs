namespace Cazzar.Deck.Abstractions.Protocol.Commands;

[RequiresFeature(DeckFeature.Alerts)]
public sealed record ShowOk(ActionRef Ref) : IDeckCommand;
