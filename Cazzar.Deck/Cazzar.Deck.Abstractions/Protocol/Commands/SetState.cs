namespace Cazzar.Deck.Abstractions.Protocol.Commands;

public sealed record SetState(ActionRef Ref, uint State) : IDeckCommand;
