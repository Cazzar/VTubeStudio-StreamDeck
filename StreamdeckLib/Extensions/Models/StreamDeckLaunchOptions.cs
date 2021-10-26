namespace StreamDeckLib.Extensions.Models
{
    public class StreamDeckLaunchOptions
    {
        public int Port { get; set; }
        public string PluginUuid { get; set; } = null!;
        public string RegisterEvent { get; set; } = null!;
        public string Info { get; set; } = null!;
    }
}
