namespace Cazzar.Deck.Abstractions.Protocol.Events;

public sealed record ActionDisappeared(ActionRef Ref) : IDeckEvent;
