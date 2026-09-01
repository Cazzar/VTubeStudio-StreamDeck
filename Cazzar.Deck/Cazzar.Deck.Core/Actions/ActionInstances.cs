using Cazzar.Deck.Abstractions.Actions.Handlers;
using Cazzar.Deck.Abstractions.Surfaces;
using Cazzar.Deck.Abstractions;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using System.Collections.Concurrent;

namespace Cazzar.Deck.Core.Actions;

public sealed class ActionInstances(
    ActionCatalog catalog,
    IServiceScopeFactory scopeFactory,
    IWidgetSurface widget,
    IEnumerable<IActionFaultObserver> faultObservers,
    ILogger<ActionInstances> logger)
{
    private readonly ConcurrentDictionary<string, Placement> _placements = new();

    private sealed record Placement(object Instance, IServiceScope Scope);

    public void Create(ActionRef @ref, IPayload settings)
    {
        if (catalog.Find(@ref.ActionId) is not { } descriptor)
        {
            logger.LogCritical("No action registered for {ActionId}", @ref.ActionId);
            return;
        }

        var scope = scopeFactory.CreateScope();

        object instance;
        try
        {
            instance = descriptor.Activate(scope.ServiceProvider);
        }
        catch (Exception e)
        {
            logger.LogCritical(e, "Could not construct {ActionType} for {Context}", descriptor.ActionType, @ref.ContextId);
            Report(@ref, e);
            scope.Dispose();
            return;
        }

        if (_placements.TryRemove(@ref.ContextId, out var replaced)) Release(@ref, replaced);

        _placements[@ref.ContextId] = new(instance, scope);

        if (instance is IContextBound bound) Guard(@ref, () => bound.Bind(@ref));
        if (instance is ISettingsHandler handler) Guard(@ref, () => handler.GotSettings(settings));
    }

    public void Destroy(ActionRef @ref)
    {
        if (!_placements.TryRemove(@ref.ContextId, out var placement))
        {
            logger.LogWarning("Disappear for unknown context {Context}", @ref.ContextId);
            return;
        }

        Release(@ref, placement);
    }

    public void Invoke<T>(ActionRef @ref, Action<T> body) where T : class
    {
        if (!_placements.TryGetValue(@ref.ContextId, out var placement))
        {
            logger.LogWarning("Event for unknown context {Context}", @ref.ContextId);
            return;
        }

        if (placement.Instance is T typed) Guard(@ref, () => body(typed));
    }

    public void Tick()
    {
        foreach (var (contextId, placement) in _placements)
        {

            if (placement.Instance is not ITickHandler handler) continue;

            Guard(new(contextId, string.Empty), handler.Tick);
        }
    }

    private void Release(ActionRef @ref, Placement placement)
    {
        if (placement.Instance is IDisposable disposable) Guard(@ref, disposable.Dispose);
        Guard(@ref, placement.Scope.Dispose);
    }

    private void Guard(ActionRef @ref, Action body)
    {
        try
        {
            body();
        }
        catch (Exception e)
        {
            logger.LogCritical(e, "Action {Context} threw", @ref.ContextId);
            Report(@ref, e);

            try
            {
                _ = widget.ShowAlertAsync(@ref);
            }
            catch (NotSupportedException)
            {
            }
        }
    }

    private void Report(ActionRef @ref, Exception exception)
    {
        foreach (var observer in faultObservers)
            observer.Faulted(@ref, exception);
    }
}
