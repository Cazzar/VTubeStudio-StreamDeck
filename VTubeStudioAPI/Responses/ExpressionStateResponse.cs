using System.Collections.Generic;
using Newtonsoft.Json;

namespace Cazzar.StreamDeck.VTubeStudio.VTubeStudioApi.Responses;

public class ExpressionHotkeyReference
{
    [JsonProperty("name")]
    public required string Name { get; set; }

    [JsonProperty("id")]
    public required string Id { get; set; }
}

public class ExpressionParameter
{
    [JsonProperty("name")]
    public required string Name { get; set; }

    [JsonProperty("value")]
    public float Value { get; set; }
}

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
    public float SecondsRemaining { get; set; }

    [JsonProperty("usedInHotkeys")]
    public required List<ExpressionHotkeyReference> UsedInHotkeys { get; set; }

    [JsonProperty("parameters")]
    public required List<ExpressionParameter> Parameters { get; set; }
}

public class ExpressionStateResponse
{
    [JsonProperty("modelLoaded")]
    public bool ModelLoaded { get; set; }

    [JsonProperty("modelName")]
    public string ModelName { get; set; } = string.Empty;

    [JsonProperty("modelID")]
    public string ModelId { get; set; } = string.Empty;

    [JsonProperty("expressions")]
    public required List<Expression> Expressions { get; set; }
}
