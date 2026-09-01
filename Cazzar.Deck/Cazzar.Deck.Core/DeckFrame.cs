using System.Text.Json;
using System.Text.Json.Nodes;

namespace Cazzar.Deck.Core;

public static class DeckFrame
{
    public static string Build(string @event, Action<JsonObject> configure)
    {
        var message = new JsonObject { ["event"] = @event };
        configure(message);
        return message.ToJsonString(DeckJson.Options);
    }

    public static string? Str(JsonObject? o, string key) =>
        o?[key] is { } node && node.GetValueKind() == JsonValueKind.String ? node.GetValue<string>() : null;

    public static int Int(JsonObject? o, string key) =>
        o?[key] is { } node && node.GetValueKind() == JsonValueKind.Number ? node.GetValue<int>() : 0;

    public static bool Bool(JsonObject? o, string key) =>
        o?[key] is { } node && node.GetValueKind() == JsonValueKind.True;
}
