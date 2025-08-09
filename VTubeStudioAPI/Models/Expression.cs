using Newtonsoft.Json;

namespace VTubeStudioAPI.Models;

public class Expression
{
    [JsonProperty("name")]
    public required string Name { get; set; }

    [JsonProperty("file")]
    public required string File { get; set; }

    [JsonProperty("active")]
    public bool Active { get; set; }

    [JsonProperty("deactivateWhenKeyIsLetGo")]
    public bool DeactivateWhenKeyIsLetGo { get; set; }

    [JsonProperty("autoDeactivateAfterSeconds")]
    public bool AutoDeactivateAfterSeconds { get; set; }

    [JsonProperty("secondsRemaining")]
    public int SecondsRemaining { get; set; }
}
