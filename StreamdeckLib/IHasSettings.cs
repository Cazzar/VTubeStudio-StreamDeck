using Newtonsoft.Json.Linq;

namespace StreamDeckLib
{
    public interface IHasSettings
    {
        public void GotSettings(JObject settings);
    }
}
