namespace Cazzar.Deck.Abstractions.Protocol.Events;

public sealed record ActionAppeared(ActionRef Ref, IPayload Settings) : IDeckEvent;
