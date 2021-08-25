using System.Collections.Generic;
using Newtonsoft.Json;

namespace Cazzar.StreamDeck.VTubeStudio.VTubeStudioApi.Models
{
    public class Model
    {
        internal sealed class IdEqualityComparer : IEqualityComparer<Model>
        {
            public bool Equals(Model x, Model y)
            {
                if (ReferenceEquals(x, y)) return true;
                if (ReferenceEquals(x, null)) return false;
                if (ReferenceEquals(y, null)) return false;
                if (x.GetType() != y.GetType()) return false;
                return x.Id == y.Id;
            }

            public int GetHashCode(Model obj)
            {
                return (obj.Id != null ? obj.Id.GetHashCode() : 0);
            }
        }

        public static IEqualityComparer<Model> IdComparer { get; } = new IdEqualityComparer();

        [JsonProperty("modelLoaded")]
        public bool IsLoaded { get; set; }

        [JsonProperty("modelName")]
        public string Name { get; set; }

        [JsonProperty("modelID")]
        public string Id { get; set; }

        [JsonProperty("vtsModelName")]
        public string VtsName { get; set; }

        [JsonProperty("vtsModelIconName")]
        public string VtsIconName { get; set; }
    }
}
