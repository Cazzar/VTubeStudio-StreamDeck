namespace Cazzar.Deck.Build;

public sealed record PackageMetadata
{
    public string Uuid { get; init; } = string.Empty;
    public string Name { get; init; } = string.Empty;
    public string Category { get; init; } = string.Empty;
    public string Author { get; init; } = string.Empty;
    public string Description { get; init; } = string.Empty;
    public string Version { get; init; } = "1.0.0";
    public string Icon { get; init; } = string.Empty;
    public string? Url { get; init; }
    public string Executable { get; init; } = string.Empty;
    public string HostMinimumVersion { get; init; } = string.Empty;
    public string WindowsMinimumVersion { get; init; } = string.Empty;
    public string MacMinimumVersion { get; init; } = string.Empty;
}
