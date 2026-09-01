namespace Cazzar.Deck.Abstractions.Protocol;

public interface IDeckTransport : IAsyncDisposable
{
    Task ConnectAsync(CancellationToken cancellationToken);
    Task SendAsync(string frame, CancellationToken cancellationToken);
    IAsyncEnumerable<string> ReadAsync(CancellationToken cancellationToken);
    Task CloseAsync();
}
