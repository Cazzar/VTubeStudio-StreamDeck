using Newtonsoft.Json.Linq;

namespace StreamDeckLib.Models
{
    public record TitleParametersPayload(Coordinates Coordinates = default, JObject Settings = default, int State = default, string Title = default, TitleParameters TitleParameters = default);
}