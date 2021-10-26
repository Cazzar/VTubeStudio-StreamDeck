using Newtonsoft.Json.Linq;

namespace StreamDeckLib.Models
{
    public record GlobalSettingsPayload
    {
        public JToken Settings { get; set; } = default;
    }
}