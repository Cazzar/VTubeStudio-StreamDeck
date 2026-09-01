using System.Text.Json.Nodes;

namespace Cazzar.Deck.Build;

public sealed class CreatorCentralManifestWriter : IManifestWriter
{
    private static string Icon(string? icon, string fallback) =>
        (icon ?? fallback) is var name && Path.HasExtension(name) ? name : name + ".png";

    private static JsonArray States(ActionMetadata action, PackageMetadata package)
    {
        var title = action.Name ?? action.ActionId;

        return action.States.Count == 0
            ? new JsonArray(new JsonObject
            {
                ["Image"] = Icon(action.Icon, package.Icon),
                ["Title"] = title,
            })
            : new JsonArray(action.States
                .Select(s => (JsonNode)new JsonObject
                {
                    ["Image"] = Icon(s.Icon ?? action.Icon, package.Icon),
                    ["Title"] = s.Name ?? title,
                })
                .ToArray());
    }

    public JsonObject Write(IReadOnlyList<(ActionMetadata Action, JsonObject? Fragment)> actions, PackageMetadata package)
    {
        var widgets = new JsonArray();

        foreach (var (action, fragment) in actions)
        {
            var widget = new JsonObject
            {
                ["UUID"] = action.ActionId,
                ["Name"] = action.Name ?? action.ActionId,
                ["Icon"] = Icon(action.Icon, package.Icon),
                ["Tooltip"] = action.Tooltip,
                ["States"] = States(action, package),
                ["Layouts"] = new JsonArray(new JsonObject
                {
                    ["Title"] = action.Name ?? action.ActionId,
                    ["Icon"] = Icon(action.Icon, package.Icon),
                    ["Width"] = 1,
                    ["Height"] = 1,
                }),
            };

            if (action.PropertyView is { Length: > 0 } view) widget["PropertyViewPath"] = view;

            widgets.Add(ManifestWriter.Merge(widget, fragment, "UUID"));
        }

        return new()
        {
            ["Name"] = package.Name,
            ["UUID"] = package.Uuid,
            ["Author"] = package.Author,
            ["Description"] = package.Description,
            ["Version"] = package.Version,
            ["Icon"] = Icon(null, package.Icon),
            ["URL"] = package.Url,
            ["Runtime"] = new JsonObject
            {
                ["mac"] = new JsonObject
                {
                    ["type"] = "bin",
                    ["target"] = $"osx/{package.Executable}",
                    ["MinimumVersion"] = package.MacMinimumVersion,
                },
                ["win"] = new JsonObject
                {
                    ["type"] = "bin",
                    ["target"] = $"win/{package.Executable}.exe",
                    ["MinimumVersion"] = package.WindowsMinimumVersion,
                },
            },
            ["CreatorCentral"] = new JsonObject
            {
                ["MinimumVersion"] = package.HostMinimumVersion,
                ["SDKVersion"] = 2,
            },
            ["Widgets"] = widgets,
        };
    }
}
