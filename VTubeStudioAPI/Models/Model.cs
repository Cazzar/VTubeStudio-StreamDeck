using System.Collections.Generic;
using Newtonsoft.Json;

namespace Cazzar.StreamDeck.VTubeStudio.VTubeStudioApi.Models;

public class Model
{
    public sealed class IdEqualityComparer : IEqualityComparer<Model>
    {
        public bool Equals(Model? x, Model? y)
        {
                if (ReferenceEquals(x, y)) return true;
                if (ReferenceEquals(x, null)) return false;
                if (ReferenceEquals(y, null)) return false;
                if (x.GetType() != y.GetType()) return false;
                return x.Id == y.Id;
            }

        public int GetHashCode(Model obj)
        {
            return obj.Id.GetHashCode();
        }
    }

    public static IEqualityComparer<Model> IdComparer { get; } = new IdEqualityComparer();

    [JsonProperty("modelLoaded")]
    public bool IsLoaded { get; set; }

    [JsonProperty("modelName")]
    public required string Name { get; set; }

    [JsonProperty("modelID")]
    public required string Id { get; set; }

    [JsonProperty("vtsModelName")]
    public required string VtsName { get; set; }

    [JsonProperty("vtsModelIconName")]
    public required string VtsIconName { get; set; }
}