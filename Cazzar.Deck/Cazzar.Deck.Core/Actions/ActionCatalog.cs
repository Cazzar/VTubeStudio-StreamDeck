using System.Collections.Concurrent;
using Cazzar.Deck.Abstractions.Actions;
using Cazzar.Deck.Abstractions.Protocol;
using Cazzar.Deck.Shared;
using Microsoft.Extensions.Logging;

namespace Cazzar.Deck.Core.Actions;

public sealed class ActionCatalog(
    IEnumerable<IActionProvider> providers,
    IDeckHostInfo host,
    IDeckPackageInfo package,
    ILogger<ActionCatalog> logger)
{
    private readonly ConcurrentDictionary<string, DeckActionDescriptor> _actions = new();

    public IReadOnlyCollection<DeckActionDescriptor> Actions => _actions.Values.ToArray();

    public DeckActionDescriptor? Find(string actionId) => _actions.GetValueOrDefault(actionId);

    public void Load()
    {
        foreach (var declared in providers.SelectMany(p => p.GetActions()))
        {
            var descriptor = declared with { ActionId = DeckActionNaming.Qualify(package.Uuid, declared.ActionId) };

            if (!descriptor.Hosts.HasFlag(host.Id))
            {
                logger.LogDebug("Skipping {ActionId}: declared for {Hosts}, running on {Host}",
                    descriptor.ActionId, descriptor.Hosts, host.Id);
                continue;
            }

            if ((descriptor.RequiredFeatures & host.Features) != descriptor.RequiredFeatures)
            {
                logger.LogDebug("Skipping {ActionId}: needs {Required}, {Host} provides {Available}",
                    descriptor.ActionId, descriptor.RequiredFeatures, host.Name, host.Features);
                continue;
            }

            if (_actions.TryAdd(descriptor.ActionId, descriptor))
            {
                logger.LogInformation("Registered {ActionId} -> {ActionType}", descriptor.ActionId, descriptor.ActionType);
                continue;
            }

            logger.LogError("Duplicate action id {ActionId}; keeping {Kept}, ignoring {Ignored}",
                descriptor.ActionId, _actions[descriptor.ActionId].ActionType, descriptor.ActionType);
        }
    }
}
