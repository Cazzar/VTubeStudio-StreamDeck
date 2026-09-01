using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

namespace Cazzar.Deck.Core.Actions;

public sealed class ActionTimer(
    ActionInstances instances,
    IOptions<ActionTimerOptions> options,
    ILogger<ActionTimer> logger) : BackgroundService
{
    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        using var timer = new PeriodicTimer(options.Value.Interval);

        try
        {
            while (await timer.WaitForNextTickAsync(stoppingToken))
                instances.Tick();
        }
        catch (OperationCanceledException) when (stoppingToken.IsCancellationRequested)
        {
            logger.LogTrace("Action timer stopped");
        }
    }
}
