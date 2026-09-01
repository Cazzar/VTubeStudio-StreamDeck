namespace Cazzar.Deck.Abstractions.Protocol.Events;

public sealed record PropertyViewClosed(ActionRef Ref) : IDeckEvent;
