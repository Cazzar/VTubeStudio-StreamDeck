namespace Cazzar.Deck.Abstractions.Protocol.Events;

public sealed record GlobalSettingsReceived(IPayload Settings) : IDeckEvent;
