namespace Cazzar.Deck.Abstractions.Protocol.Commands;

public sealed record SetTitle(ActionRef Ref, string? Title, uint? State = null) : IDeckCommand;
