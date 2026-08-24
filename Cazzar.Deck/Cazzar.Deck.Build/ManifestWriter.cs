using System.Text.Json;
using System.Text.Json.Nodes;

namespace Cazzar.Deck.Build;

public static class ManifestWriter
{
    public static IManifestWriter For(string host) => host switch
    {
        "StreamDeck" => new StreamDeckManifestWriter(),
        "CreatorCentral" => new CreatorCentralManifestWriter(),
        _ => throw new ArgumentException($"Unknown host '{host}'.", nameof(host)),
    };

    public static string Serialise(JsonObject manifest) =>
        manifest.ToJsonString(new()
            { WriteIndented = true });

    // Attributes win on identity; a fragment may extend or override anything else.
    internal static JsonObject Merge(JsonObject basis, JsonObject? fragment, params string[] reserved)
    {
        if (fragment is null) return basis;

        foreach (var property in fragment)
        {
            if (reserved.Contains(property.Key))
                throw new InvalidOperationException($"Manifest fragment may not set '{property.Key}'; it comes from [DeckAction].");

            basis[property.Key] = property.Value?.DeepClone();
        }

        return basis;
    }
}
