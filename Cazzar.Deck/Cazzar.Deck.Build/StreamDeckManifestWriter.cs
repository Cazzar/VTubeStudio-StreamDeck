using System.Text.Json.Nodes;

namespace Cazzar.Deck.Build;

public sealed class StreamDeckManifestWriter : IManifestWriter
{
    // Elgato requires exactly {major}.{minor}.{patch}.{build}.
    private static string Version(string version)
    {
        var parts = version.Split('.');
        return parts.Length >= 4 ? string.Join(".", parts.Take(4)) : string.Join(".", parts.Concat(Enumerable.Repeat("0", 4 - parts.Length)));
    }

    private static JsonObject State(string image, string? name)
    {
        var state = new JsonObject
        {
            ["Image"] = image,
            ["TitleAlignment"] = "bottom",
            ["FontSize"] = 10,
        };

        if (name is { Length: > 0 }) state["Name"] = name;

        return state;
    }

    private static JsonArray States(ActionMetadata action, PackageMetadata package) =>
        action.States.Count == 0
            ? new JsonArray(State(action.Icon ?? package.Icon, null))
            : new JsonArray(action.States
                .Select(s => (JsonNode)State(s.Icon ?? action.Icon ?? package.Icon, s.Name))
                .ToArray());

    public JsonObject Write(IReadOnlyList<(ActionMetadata Action, JsonObject? Fragment)> actions, PackageMetadata package)
    {
        var entries = new JsonArray();

        foreach (var (action, fragment) in actions)
        {
            var entry = new JsonObject
            {
                ["UUID"] = action.ActionId,
                ["Name"] = action.Name ?? action.ActionId,
                ["Icon"] = action.Icon ?? package.Icon,
                ["Tooltip"] = action.Tooltip,
                ["States"] = States(action, package),
                ["SupportedInMultiActions"] = true,
            };

            if (action.States.Count > 1 && !action.AutomaticStates) entry["DisableAutomaticStates"] = true;

            if (action.PropertyView is { Length: > 0 } view) entry["PropertyInspectorPath"] = view;
            if (action.NeedsEncoder) entry["Controllers"] = new JsonArray("Encoder");

            entries.Add(ManifestWriter.Merge(entry, fragment, "UUID"));
        }

        return new()
        {
            ["$schema"] = "https://schemas.elgato.com/streamdeck/plugins/manifest.json",
            ["Name"] = package.Name,
            ["UUID"] = package.Uuid,
            ["Category"] = string.IsNullOrEmpty(package.Category) ? package.Name : package.Category,
            ["CategoryIcon"] = package.Icon,
            ["Author"] = package.Author,
            ["Description"] = package.Description,
            ["Version"] = Version(package.Version),
            ["Icon"] = package.Icon,
            ["URL"] = package.Url,
            ["SDKVersion"] = 2,
            ["CodePath"] = $"win/{package.Executable}.exe",
            ["CodePathWin"] = $"win/{package.Executable}.exe",
            ["CodePathMac"] = $"osx/{package.Executable}",
            ["Software"] = new JsonObject { ["MinimumVersion"] = package.HostMinimumVersion },
            ["OS"] = new JsonArray(
                new JsonObject { ["Platform"] = "windows", ["MinimumVersion"] = "10" },
                new JsonObject { ["Platform"] = "mac", ["MinimumVersion"] = "11" }),
            ["Actions"] = entries,
        };
    }
}
