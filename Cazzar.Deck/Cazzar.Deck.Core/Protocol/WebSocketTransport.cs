using System.Net.WebSockets;
using System.Runtime.CompilerServices;
using System.Text;
using Cazzar.Deck.Abstractions.Protocol;
using Microsoft.Extensions.Logging;

namespace Cazzar.Deck.Core.Protocol;

public sealed class WebSocketTransport(IDeckLaunchOptions options, ILogger<WebSocketTransport> logger) : IDeckTransport
{
    private const int BufferSize = 1024 * 1024;

    private readonly ClientWebSocket _socket = new();
    private readonly SemaphoreSlim _send = new(1, 1);

    public async Task ConnectAsync(CancellationToken cancellationToken)
    {
        logger.LogDebug("Connecting to ws://localhost:{Port}", options.Port);
        await _socket.ConnectAsync(new($"ws://localhost:{options.Port}"), cancellationToken);

        if (_socket.State != WebSocketState.Open)
            throw new InvalidOperationException($"Websocket did not open; state is {_socket.State}.");
    }

    public async Task SendAsync(string frame, CancellationToken cancellationToken)
    {
        if (_socket.State != WebSocketState.Open)
        {
            logger.LogWarning("Dropping frame; socket state is {State}", _socket.State);
            return;
        }

        await _send.WaitAsync(cancellationToken);
        try
        {
            await _socket.SendAsync(Encoding.UTF8.GetBytes(frame), WebSocketMessageType.Text, true, cancellationToken);
        }
        finally
        {
            _send.Release();
        }
    }

    public async IAsyncEnumerable<string> ReadAsync([EnumeratorCancellation] CancellationToken cancellationToken)
    {
        var buffer = new byte[BufferSize];
        var text = new StringBuilder(BufferSize);

        var decoder = Encoding.UTF8.GetDecoder();

        while (!cancellationToken.IsCancellationRequested)
        {
            ValueWebSocketReceiveResult result;
            try
            {
                result = await _socket.ReceiveAsync(buffer.AsMemory(), cancellationToken);
            }
            catch (OperationCanceledException)
            {
                yield break;
            }
            catch (WebSocketException e)
            {
                logger.LogWarning(e, "Websocket faulted; ending read loop");
                yield break;
            }

            if (result.MessageType == WebSocketMessageType.Close)
            {
                logger.LogInformation("Host closed the connection: {Status}", _socket.CloseStatus);
                yield break;
            }

            if (result.MessageType != WebSocketMessageType.Text) continue;

            var chars = new char[Encoding.UTF8.GetMaxCharCount(result.Count)];
            text.Append(chars, 0, decoder.GetChars(buffer, 0, result.Count, chars, 0, result.EndOfMessage));

            if (!result.EndOfMessage) continue;

            yield return text.ToString();
            text.Clear();
        }
    }

    public async Task CloseAsync()
    {
        if (_socket.State == WebSocketState.Open)
            await _socket.CloseAsync(WebSocketCloseStatus.NormalClosure, null, CancellationToken.None);
    }

    public ValueTask DisposeAsync()
    {
        _send.Dispose();
        _socket.Dispose();
        return ValueTask.CompletedTask;
    }
}
