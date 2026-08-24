using Cazzar.Deck.Core.Actions;
using Cazzar.Deck.Abstractions.Protocol.Events;
using Cazzar.Deck.Abstractions.Protocol;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;

namespace Cazzar.Deck.Core.Protocol;

public sealed class DeckSession(
    IDeckTransport transport,
    IDeckCodec codec,
    IDeckHandshake handshake,
    DeckClient client,
    DeckEventDispatcher dispatcher,
    ActionCatalog catalog,
    IHostApplicationLifetime lifetime,
    ILogger<DeckSession> logger) : BackgroundService
{
    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        catalog.Load();

        try
        {
            await transport.ConnectAsync(stoppingToken);

            foreach (var message in handshake.OpeningMessages())
                await client.Send(message);

            await foreach (var frame in transport.ReadAsync(stoppingToken))
            {
                logger.LogTrace("<- {Frame}", frame);

                IReadOnlyList<IDeckEvent> events;
                try
                {
                    events = codec.Decode(frame);
                }
                catch (Exception e)
                {
                    logger.LogError(e, "Could not decode frame: {Frame}", frame);
                    continue;
                }

                foreach (var @event in events)
                    dispatcher.Dispatch(@event);
            }
        }
        catch (OperationCanceledException) when (stoppingToken.IsCancellationRequested)
        {
        }
        catch (Exception e)
        {
            logger.LogCritical(e, "Deck session ended unexpectedly");
        }
        finally
        {
            await transport.CloseAsync();
        }

        if (!stoppingToken.IsCancellationRequested) lifetime.StopApplication();
    }
}
