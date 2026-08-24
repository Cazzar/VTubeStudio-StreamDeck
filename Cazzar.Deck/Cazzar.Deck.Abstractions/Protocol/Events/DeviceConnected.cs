namespace Cazzar.Deck.Abstractions.Protocol.Events;

/// <summary>Only raised by hosts advertising <see cref="DeckFeature.DeviceEvents"/>.</summary>
public sealed record DeviceConnected(string DeviceId, string? Name, int Columns, int Rows) : IDeckEvent;
