using System.Threading.Channels;
using Cazzar.Deck.Abstractions.Protocol;

namespace Cazzar.Deck.Hosts.Loopback;

public sealed class LoopbackTransport : IDeckTransport
{
    private readonly Channel<string> _inbound = Channel.CreateUnbounded<string>();
    private readonly List<string> _sent = [];

    public IReadOnlyList<string> Sent
    {
        get { lock (_sent) return [.. _sent]; }
    }

    public Task ConnectAsync(CancellationToken cancellationToken) => Task.CompletedTask;

    public Task SendAsync(string frame, CancellationToken cancellationToken)
    {
        lock (_sent) _sent.Add(frame);
        return Task.CompletedTask;
    }

    public IAsyncEnumerable<string> ReadAsync(CancellationToken cancellationToken) =>
        _inbound.Reader.ReadAllAsync(cancellationToken);

    public void Push(string frame) => _inbound.Writer.TryWrite(frame);

    public Task CloseAsync()
    {
        _inbound.Writer.TryComplete();
        return Task.CompletedTask;
    }

    public ValueTask DisposeAsync() => ValueTask.CompletedTask;
}
