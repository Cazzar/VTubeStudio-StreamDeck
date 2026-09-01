using System.Text.Json.Serialization;

namespace VTubeStudio.Api.Models;

public sealed record Hotkey
{
    [JsonPropertyName("name")] public string Name { get; init; } = string.Empty;
    [JsonPropertyName("type")] public string Type { get; init; } = string.Empty;
    [JsonPropertyName("file")] public string? File { get; init; }
    [JsonPropertyName("hotkeyID")] public string Id { get; init; } = string.Empty;
    [JsonPropertyName("description")] public string? Description { get; init; }

    // The name is user-supplied and often blank; fall back to something recognisable on a key.
    public string ButtonTitle() =>
        !string.IsNullOrWhiteSpace(Name) ? Name :
        !string.IsNullOrWhiteSpace(File) ? Path.GetFileNameWithoutExtension(File) :
        Type;
}
