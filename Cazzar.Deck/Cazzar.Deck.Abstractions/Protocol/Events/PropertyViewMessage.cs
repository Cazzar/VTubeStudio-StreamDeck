namespace Cazzar.Deck.Abstractions.Protocol.Events;

public sealed record PropertyViewMessage(ActionRef Ref, IPayload Body) : IDeckEvent;
