using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;

namespace VTubeStudio.Api;

// Keeps the socket up without every action having to poke it.
public sealed class VTubeStudioConnection(IVTubeStudio vts, ILogger<VTubeStudioConnection> logger) : BackgroundService
{
    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        using var timer = new PeriodicTimer(TimeSpan.FromSeconds(5));

        try
        {
            do {
                if (vts.IsConnected)
                    continue;
                
                logger.LogDebug("VTube Studio not connected; attempting to reconnect");
                vts.EnsureConnected();
            }
            while (await timer.WaitForNextTickAsync(stoppingToken));
        }
        catch (OperationCanceledException)
        {
        }
    }
}
