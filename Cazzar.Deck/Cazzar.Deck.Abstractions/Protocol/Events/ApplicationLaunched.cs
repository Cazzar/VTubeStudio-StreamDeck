namespace Cazzar.Deck.Abstractions.Protocol.Events;

/// <summary>Only raised by hosts advertising <see cref="DeckFeature.ApplicationEvents"/>.</summary>
public sealed record ApplicationLaunched(string Application) : IDeckEvent;
