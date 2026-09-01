namespace Cazzar.Deck.Abstractions.Protocol.Events;

// Only raised by hosts that report machine wake.
public sealed record HostWokeUp : IDeckEvent;
