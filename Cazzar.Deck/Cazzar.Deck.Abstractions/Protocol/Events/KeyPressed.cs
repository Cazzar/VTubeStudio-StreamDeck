namespace Cazzar.Deck.Abstractions.Protocol.Events;

public sealed record KeyPressed(ActionRef Ref, uint State) : IDeckEvent;
