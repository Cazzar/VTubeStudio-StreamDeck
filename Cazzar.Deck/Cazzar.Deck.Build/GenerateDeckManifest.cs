using Cazzar.Deck.Shared;
using System.Text.Json.Nodes;
using Microsoft.Build.Framework;
using Task = Microsoft.Build.Utilities.Task;

namespace Cazzar.Deck.Build;

public sealed class GenerateDeckManifest : Task
{
    [Required] public string Assembly { get; set; } = string.Empty;
    [Required] public string Host { get; set; } = string.Empty;
    [Required] public string Output { get; set; } = string.Empty;

    public ITaskItem[] References { get; set; } = [];
    public ITaskItem[] Fragments { get; set; } = [];

    public string PackageUuid { get; set; } = string.Empty;
    public string PackageName { get; set; } = string.Empty;
    public string PackageAuthor { get; set; } = string.Empty;
    public string PackageDescription { get; set; } = string.Empty;
    public string PackageVersion { get; set; } = "1.0.0";
    public string PackageIcon { get; set; } = string.Empty;
    public string PackageCategory { get; set; } = string.Empty;
    public string PackageUrl { get; set; } = string.Empty;
    public string Executable { get; set; } = string.Empty;
    public string HostMinimumVersion { get; set; } = string.Empty;
    public string WindowsMinimumVersion { get; set; } = "10";
    public string MacMinimumVersion { get; set; } = "11.0";

    /// Directory the manifest paths are relative to.
    public string? PackageDirectory { get; set; }

    /// Escalate missing icons and property views from a warning to a build error. Turn on once the
    /// package directory is fully assembled, which for a Vite property view means after its build.
    public bool StrictAssets { get; set; }

    /// Single entry document every property view is served from.
    public string PropertyViewEntry { get; set; } = "PropertyInspector/index.html";

    /// Where to write the generated action-id to page-name route map.
    public string? RoutesOutput { get; set; }

    /// Compare against the file on disk instead of writing it. A mismatch fails the build.
    public bool Verify { get; set; }

    /// The property view is built later in this same build, so its absence now means nothing.
    public bool PropertyViewPending { get; set; }

    public override bool Execute()
    {
        try
        {
            var actions = ActionMetadataReader.Read(Assembly, References.Select(r => r.ItemSpec))
                .Where(a => a.RunsOn(Host))
                .Select(a => a with { ActionId = DeckActionNaming.Qualify(PackageUuid, a.ActionId) })
                .OrderBy(a => a.ActionId)
                .ToList();

            if (actions.Count == 0)
                Log.LogWarning($"No actions found in {Path.GetFileName(Assembly)} for host {Host}.");

            var fragments = LoadFragments();
            var paired = actions
                .Select(a => (
                    Action: a with { PropertyView = string.IsNullOrEmpty(a.PropertyView) ? null : PropertyViewEntry },
                    Fragment: fragments.TryGetValue(a.TypeName.Substring(a.TypeName.LastIndexOf('.') + 1), out var f) ? f : null))
                .ToList();

            var manifest = ManifestWriter.For(Host).Write(paired, new()
            {
                Uuid = PackageUuid,
                Name = PackageName,
                Category = PackageCategory,
                Author = PackageAuthor,
                Description = PackageDescription,
                Version = PackageVersion,
                Icon = PackageIcon,
                Url = string.IsNullOrEmpty(PackageUrl) ? null : PackageUrl,
                Executable = Executable,
                HostMinimumVersion = HostMinimumVersion,
                WindowsMinimumVersion = WindowsMinimumVersion,
                MacMinimumVersion = MacMinimumVersion,
            });

            ValidateStates(actions);
            ValidateAssets(actions);

            var json = ManifestWriter.Serialise(manifest);
            var current = File.Exists(Output) ? File.ReadAllText(Output) : null;

            if (Verify)
            {
                if (current == json) return true;

                Log.LogError($"{Output} is out of date. Rebuild without Verify to regenerate it.");
                return false;
            }

            WriteRoutes(actions);

            if (current == json) return true;

            Directory.CreateDirectory(Path.GetDirectoryName(Output)!);
            File.WriteAllText(Output, json);
            Log.LogMessage(MessageImportance.Normal, $"Wrote {actions.Count} action(s) to {Output}.");

            return true;
        }
        catch (Exception e)
        {
            Log.LogErrorFromException(e, showStackTrace: false);
            return false;
        }
    }

    // The front end routes on the action id the host hands it, so the map has to come from the
    // same attributes the manifest does or the two drift apart.
    private void WriteRoutes(IReadOnlyList<ActionMetadata> actions)
    {
        if (string.IsNullOrEmpty(RoutesOutput)) return;

        var lines = new List<string>
        {
            "// <auto-generated /> Written by GenerateDeckManifest.",
            "export default {",
        };

        foreach (var action in actions.Where(a => !string.IsNullOrEmpty(a.PropertyView)))
            lines.Add($"  '{action.ActionId}': () => import('@shared/pages/{action.PropertyView}/App.vue'),");

        lines.Add("}");
        lines.Add(string.Empty);

        var contents = string.Join(Environment.NewLine, lines);

        Directory.CreateDirectory(Path.GetDirectoryName(RoutesOutput)!);

        if (File.Exists(RoutesOutput) && File.ReadAllText(RoutesOutput) == contents) return;

        File.WriteAllText(RoutesOutput!, contents);
    }

    // Fragments are matched to their action by file name: ZoomModelAction.streamdeck.json.
    private Dictionary<string, JsonObject> LoadFragments()
    {
        var fragments = new Dictionary<string, JsonObject>(StringComparer.OrdinalIgnoreCase);

        foreach (var item in Fragments)
        {
            var name = Path.GetFileName(item.ItemSpec).Split('.')[0];

            if (JsonNode.Parse(File.ReadAllText(item.ItemSpec)) is not JsonObject fragment)
            {
                Log.LogError($"{item.ItemSpec} is not a JSON object.");
                continue;
            }

            fragment.Remove("$schema");
            fragments[name] = fragment;
        }

        return fragments;
    }

    private void ValidateStates(IReadOnlyList<ActionMetadata> actions)
    {
        foreach (var action in actions.Where(a => a.AutomaticStates && a.States.Count < 2))
            Log.LogWarning(
                $"{action.TypeName} sets AutomaticStates but declares {action.States.Count} state(s). " +
                "It has no effect below two.");

        foreach (var action in actions.Where(a => a.States.Count > 0))
            if (action.States.Select((s, i) => s.Value == (uint)i).Any(ordered => !ordered))
                Log.LogError(
                    $"{action.TypeName} declares states [{string.Join(", ", action.States.Select(s => s.Value))}]. " +
                    $"State values must run 0..{action.States.Count - 1} without gaps.");
    }

    private void ValidateAssets(IReadOnlyList<ActionMetadata> actions)
    {
        if (string.IsNullOrEmpty(PackageDirectory) || !Directory.Exists(PackageDirectory)) return;

        if (!PropertyViewPending &&
            actions.Any(a => !string.IsNullOrEmpty(a.PropertyView)) &&
            !File.Exists(Path.Combine(PackageDirectory!, PropertyViewEntry)))
            Report($"Property view entry '{PropertyViewEntry}' does not exist in {PackageDirectory}.");

        var icons = actions.Select(a => a.Icon)
            .Concat(actions.SelectMany(a => a.States.Select(s => s.Icon)))
            .Concat([PackageIcon])
            .Where(i => !string.IsNullOrEmpty(i))
            .Select(i => Path.HasExtension(i) ? i! : i + ".png")
            .Distinct(StringComparer.OrdinalIgnoreCase);

        foreach (var icon in icons.Where(i => !File.Exists(Path.Combine(PackageDirectory!, i))))
            Report($"Icon '{icon}' does not exist in {PackageDirectory}.");
    }

    private void Report(string message)
    {
        if (StrictAssets) Log.LogError(message);
        else Log.LogWarning(message);
    }
}
