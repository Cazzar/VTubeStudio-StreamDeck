using Newtonsoft.Json.Linq;

namespace StreamDeckLib
{
    public interface IGlobalSettingsHandler
    {
        public void GotGlobalSettings(JToken token);
    }
}
