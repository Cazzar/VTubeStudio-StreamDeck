using System.Text.Json;
using System.Text.Json.Nodes;
using System.Text.Json.Serialization;
using System.Text.Json.Serialization.Metadata;

namespace Cazzar.Deck.Core;

public static class DeckJson
{
    public static readonly JsonSerializerOptions Options = new(JsonSerializerDefaults.Web)
    {
        PropertyNamingPolicy = JsonNamingPolicy.CamelCase,
        DefaultIgnoreCondition = JsonIgnoreCondition.WhenWritingNull,
        TypeInfoResolver = DeckJsonContext.Default,
    };

    public static void AddContext(IJsonTypeInfoResolver resolver)
    {
        if (Options.TypeInfoResolverChain.Contains(resolver)) return;

        Options.TypeInfoResolverChain.Add(resolver);
    }

    public static JsonTypeInfo<T> TypeInfo<T>() => (JsonTypeInfo<T>)Options.GetTypeInfo(typeof(T));

    public static JsonNode? ToNode<T>(T value) => JsonSerializer.SerializeToNode(value, TypeInfo<T>());
}

[JsonSerializable(typeof(string))]
[JsonSerializable(typeof(bool))]
[JsonSerializable(typeof(int))]
[JsonSerializable(typeof(uint))]
[JsonSerializable(typeof(long))]
[JsonSerializable(typeof(double))]
sealed partial class DeckJsonContext : JsonSerializerContext;
