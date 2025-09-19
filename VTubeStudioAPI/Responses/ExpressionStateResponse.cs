using Newtonsoft.Json;
using VTubeStudioAPI.Models;

namespace VTubeStudioAPI.Responses;

public class ExpressionStateResponse
{
    [JsonProperty("modelLoaded")]
    public bool ModelLoaded { get; set; }
    
    [JsonProperty("modelName")]
    public required string ModelName { get; set; }

    [JsonProperty("modelID")]
    public required string ModelId { get; set; }

    [JsonProperty("expressions")]
    public required List<Expression> Expressions { get; set; }
}