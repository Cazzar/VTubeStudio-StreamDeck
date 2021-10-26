using Newtonsoft.Json.Linq;

namespace StreamDeckLib.Models
{
    public record KeyActionPayload(JObject Settings, Coordinates Coordinates, int State, int UserDesiredState, bool IsInMultiAction);
}