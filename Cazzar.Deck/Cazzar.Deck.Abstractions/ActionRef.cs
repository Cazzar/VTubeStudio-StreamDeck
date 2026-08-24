namespace Cazzar.Deck.Abstractions;

public readonly record struct ActionRef(
    string ContextId,
    string ActionId,
    string? DeviceId = null,
    DeckController Controller = DeckController.Keypad);
