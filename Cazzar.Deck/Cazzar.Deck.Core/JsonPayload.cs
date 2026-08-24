using Cazzar.Deck.Abstractions;
using System.Text.Json.Nodes;
using System.Text.Json;

namespace Cazzar.Deck.Core;

public sealed class JsonPayload(JsonNode node) : IPayload
{
    public JsonNode Node { get; } = node;

    public T? As<T>() => Node.Deserialize(DeckJson.TypeInfo<T>());

    public bool TryGet<T>(string key, out T? value)
    {
        value = default;
        if (Node is not JsonObject obj || !obj.TryGetPropertyValue(key, out var found) || found is null) return false;

        value = found.Deserialize(DeckJson.TypeInfo<T>());
        return true;
    }

    public static IPayload From(JsonNode? node) => node is null ? IPayload.Empty : new JsonPayload(node);
}
