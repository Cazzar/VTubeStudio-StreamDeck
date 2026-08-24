namespace Cazzar.Deck.Abstractions.Protocol.Events;

/// <summary>Only raised by hosts advertising <see cref="DeckFeature.Touchscreen"/>.</summary>
public sealed record TouchTapped(ActionRef Ref, int X, int Y, bool Hold) : IDeckEvent;
