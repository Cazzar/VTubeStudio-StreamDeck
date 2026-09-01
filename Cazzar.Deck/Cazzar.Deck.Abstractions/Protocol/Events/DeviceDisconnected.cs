namespace Cazzar.Deck.Abstractions.Protocol.Events;

/// <summary>Only raised by hosts advertising <see cref="DeckFeature.DeviceEvents"/>.</summary>
public sealed record DeviceDisconnected(string DeviceId) : IDeckEvent;
