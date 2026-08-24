namespace Cazzar.Deck.Abstractions.Protocol;

public interface IDeckLaunchOptions
{
    int Port { get; }
    string Uuid { get; }
    string RegisterEvent { get; }
    string? Info { get; }
}
