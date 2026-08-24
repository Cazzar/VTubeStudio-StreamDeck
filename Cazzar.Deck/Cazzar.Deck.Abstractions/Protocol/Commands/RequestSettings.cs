namespace Cazzar.Deck.Abstractions.Protocol.Commands;

public sealed record RequestSettings(ActionRef Ref) : IDeckCommand;
