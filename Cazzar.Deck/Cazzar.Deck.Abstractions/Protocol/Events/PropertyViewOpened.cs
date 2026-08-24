namespace Cazzar.Deck.Abstractions.Protocol.Events;

public sealed record PropertyViewOpened(ActionRef Ref) : IDeckEvent;
