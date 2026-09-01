using System.Text.Json.Serialization;

namespace VTubeStudio.Api.Events;

public sealed record ExpressionToggledEvent
{
    [JsonPropertyName("modelID")] public string ModelId { get; init; }  = string.Empty;
    [JsonPropertyName("modelName")] public string ModelName { get; init; }  = string.Empty;
    [JsonPropertyName("isLive2DItem")] public bool IsLive2DItem { get; init; } = false;
    [JsonPropertyName("itemInstanceID")] public string ItemInstanceId { get; init; }  = string.Empty;
    [JsonPropertyName("justLoaded")] public bool JustLoaded { get; init; }  = false;
    [JsonPropertyName("expressionFile")] public string ExpressionFile { get; init; }  = string.Empty;
    [JsonPropertyName("expressionName")] public string ExpressionName { get; init; }  = string.Empty;
    [JsonPropertyName("active")] public bool Active { get; init; } = false;
}
