using Newtonsoft.Json.Linq;

namespace StreamDeckLib.Models
{
    public readonly record struct AppearanceChangePayload(JObject Settings = default, Coordinates Coordinates = default, int State = default, bool IsInMultiAction = default);
}