using System.Text.Json.Nodes;

namespace Cazzar.Deck.Build;

public interface IManifestWriter
{
    string Host { get; }
    JsonObject Write(IReadOnlyList<(ActionMetadata Action, JsonObject? Fragment)> actions, PackageMetadata package);
}
