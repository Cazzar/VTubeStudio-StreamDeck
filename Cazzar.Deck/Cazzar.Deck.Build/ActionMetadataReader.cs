using System.Reflection;
using Cazzar.Deck.Shared;

namespace Cazzar.Deck.Build;

public static class ActionMetadataReader
{
    private const string ActionAttribute = "Cazzar.Deck.Abstractions.DeckActionAttribute";
    private const string StateAttribute = "Cazzar.Deck.Abstractions.DeckStateAttribute";
    private const string FeatureAttribute = "Cazzar.Deck.Abstractions.RequiresFeatureAttribute";
    private const int EncoderFeature = DeckHostFlags.EncoderFeature;

    public static IReadOnlyList<ActionMetadata> Read(string assemblyPath, IEnumerable<string> references)
    {
        var paths = references.Concat([assemblyPath]).Where(File.Exists).Distinct();

        using var context = new MetadataLoadContext(new PathAssemblyResolver(paths));
        var assembly = context.LoadFromAssemblyPath(assemblyPath);

        var actions = new List<ActionMetadata>();

        foreach (var type in assembly.GetTypes())
        {
            var attribute = type.GetCustomAttributesData()
                .FirstOrDefault(a => a.AttributeType.FullName == ActionAttribute);

            if (attribute is null) continue;

            actions.Add(new()
            {
                ActionId = ActionIdOf(attribute, type),
                TypeName = type.FullName ?? type.Name,
                Hosts = HostsOf(attribute),
                Name = Named(attribute, "Name"),
                Tooltip = Named(attribute, "Tooltip"),
                Icon = Named(attribute, "Icon"),
                PropertyView = Named(attribute, "PropertyView"),
                NeedsEncoder = NeedsEncoder(type),
                States = StatesOf(type),
                AutomaticStates = Flag(attribute, "AutomaticStates"),
            });
        }

        return actions;
    }

    private static string ActionIdOf(CustomAttributeData attribute, Type type) =>
        attribute.ConstructorArguments.Count > 0 && attribute.ConstructorArguments[0].Value is string actionId
            ? actionId
            : DeckActionNaming.Derive(type.Name);

    private static string HostsOf(CustomAttributeData attribute) =>
        attribute.NamedArguments.FirstOrDefault(a => a.MemberName == "Hosts") is { } argument &&
        argument.TypedValue.Value is int flags
            ? Describe(flags)
            : "All";

    private static string Describe(int flags) =>
        flags == DeckHostFlags.All ? "All" : string.Join(",", DeckHostFlags.Names(flags));

    private static string? Named(CustomAttributeData attribute, string member) =>
        attribute.NamedArguments.FirstOrDefault(a => a.MemberName == member).TypedValue.Value as string;

    private static bool Flag(CustomAttributeData attribute, string member) =>
        attribute.NamedArguments.FirstOrDefault(a => a.MemberName == member).TypedValue.Value is true;

    private static IReadOnlyList<StateMetadata> StatesOf(Type type)
    {
        for (var current = type; current is not null; current = current.BaseType)
        {
            if (!current.IsGenericType) continue;

            foreach (var argument in current.GetGenericArguments().Where(a => a.IsEnum))
                if (StatesOfEnum(argument) is { Count: > 0 } states)
                    return states;
        }

        return [];
    }

    private static IReadOnlyList<StateMetadata> StatesOfEnum(Type enumType) =>
        enumType.GetFields(BindingFlags.Public | BindingFlags.Static)
            .Select(f => (Field: f, Attribute: f.GetCustomAttributesData()
                .FirstOrDefault(a => a.AttributeType.FullName == StateAttribute)))
            .Where(x => x.Attribute is not null)
            .Select(x => new StateMetadata
            {
                Value = Convert.ToUInt32(x.Field.GetRawConstantValue()),
                Icon = Named(x.Attribute!, "Icon"),
                Name = Named(x.Attribute!, "Name"),
            })
            .OrderBy(s => s.Value)
            .ToList();

    private static bool NeedsEncoder(Type type) =>
        type.GetInterfaces().Append(type).Any(t => t.GetCustomAttributesData().Any(a =>
            a.AttributeType.FullName == FeatureAttribute &&
            a.ConstructorArguments.Count > 0 &&
            a.ConstructorArguments[0].Value is int feature &&
            (feature & EncoderFeature) == EncoderFeature));
}
