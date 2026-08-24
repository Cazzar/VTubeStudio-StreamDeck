namespace Cazzar.Deck.Abstractions.Protocol.Commands;

[RequiresFeature(DeckFeature.Alerts)]
public sealed record ShowAlert(ActionRef Ref) : IDeckCommand;
