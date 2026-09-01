namespace Cazzar.Deck.Abstractions.Protocol.Events;

/// <summary>Only raised by hosts advertising <see cref="DeckFeature.Encoder"/>.</summary>
public sealed record DialPressed(ActionRef Ref) : IDeckEvent;
