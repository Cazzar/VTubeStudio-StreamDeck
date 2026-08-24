namespace Cazzar.Deck.Abstractions.Protocol.Commands;

public sealed record SetImage(ActionRef Ref, string Image, uint State = 0) : IDeckCommand;
