using Newtonsoft.Json.Linq;

namespace StreamDeckLib.Models
{
    public record SettingsPayload
    {
        public JObject Settings { get; set; }
        public Coordinates Coordinates { get; set; }
        public bool IsInMultiAction { get; set; }
    }
}