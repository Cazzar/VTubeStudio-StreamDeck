namespace Cazzar.Deck.Abstractions.Protocol.Events;

public sealed record SettingsReceived(ActionRef Ref, IPayload Settings) : IDeckEvent;
