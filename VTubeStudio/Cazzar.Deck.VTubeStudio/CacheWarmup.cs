using Cazzar.Deck.VTubeStudio.Actions.Movement;
using Cazzar.Deck.VTubeStudio.Caches;
using Microsoft.Extensions.Hosting;

namespace Cazzar.Deck.VTubeStudio;

// Caches subscribe to VTS events in their constructors, so they have to exist before any action is placed.
sealed class CacheWarmup(ModelCache models, HotkeyCache hotkeys, ExpressionCache expressions, ModelPositionTracker tracker)
    : IHostedService
{
    public Task StartAsync(CancellationToken cancellationToken)
    {
        _ = (models, hotkeys, expressions, tracker);
        return Task.CompletedTask;
    }

    public Task StopAsync(CancellationToken cancellationToken) => Task.CompletedTask;
}
