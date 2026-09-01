namespace Cazzar.Deck.Abstractions.Protocol.Events;

public sealed record KeyReleased(ActionRef Ref, uint State) : IDeckEvent;
