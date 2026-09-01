namespace Cazzar.Deck.Abstractions.Protocol.Commands;

public sealed record Register(string Uuid, string Event) : IDeckCommand;
