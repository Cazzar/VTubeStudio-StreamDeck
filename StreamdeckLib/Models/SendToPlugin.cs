using Newtonsoft.Json.Linq;

namespace StreamDeckLib.Models
{
    public record SendToPlugin(string Action, string Context, JObject Payload) : EventMessage;
}