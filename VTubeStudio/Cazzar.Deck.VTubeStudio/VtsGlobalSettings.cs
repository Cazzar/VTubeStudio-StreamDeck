namespace Cazzar.Deck.VTubeStudio;

public sealed class VtsGlobalSettings
{
    public string? Token { get; set; }
    public string Host { get; set; } = "127.0.0.1";
    public ushort Port { get; set; } = 8001;
}
