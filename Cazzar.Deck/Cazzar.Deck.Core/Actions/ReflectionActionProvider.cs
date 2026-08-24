using Cazzar.Deck.Abstractions.Actions;
using Cazzar.Deck.Abstractions;
using Cazzar.Deck.Shared;
using Microsoft.Extensions.DependencyInjection;
using System.Reflection;

namespace Cazzar.Deck.Core.Actions;

public sealed class ReflectionActionProvider(params Assembly[] assemblies) : IActionProvider
{
    public IEnumerable<DeckActionDescriptor> GetActions()
    {
        foreach (var type in assemblies.SelectMany(GetLoadableTypes))
        {
            if (type.GetCustomAttribute<DeckActionAttribute>() is not { } attribute) continue;

            var factory = ActivatorUtilities.CreateFactory(type, Type.EmptyTypes);

            yield return new()
            {
                ActionId = attribute.ActionId ?? DeckActionNaming.Derive(type.Name),
                ActionType = type,
                Hosts = attribute.Hosts,
                RequiredFeatures = RequiredFeatures(type),
                Name = attribute.Name,
                Tooltip = attribute.Tooltip,
                Icon = attribute.Icon,
                PropertyView = attribute.PropertyView,
                Activate = services => factory(services, null),
            };
        }
    }

    public static DeckFeature RequiredFeatures(Type type) =>
        type.GetInterfaces()
            .Append(type)
            .Select(t => t.GetCustomAttribute<RequiresFeatureAttribute>(inherit: true))
            .Where(a => a is not null)
            .Aggregate(DeckFeature.None, (features, a) => features | a!.Feature);

    private static IEnumerable<Type> GetLoadableTypes(Assembly assembly)
    {
        try
        {
            return assembly.GetTypes();
        }
        catch (ReflectionTypeLoadException e)
        {
            return e.Types.OfType<Type>();
        }
    }
}
