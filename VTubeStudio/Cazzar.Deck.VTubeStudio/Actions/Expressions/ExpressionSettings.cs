using System.Text.Json.Serialization;

namespace Cazzar.Deck.VTubeStudio.Actions.Expressions;

public sealed class ExpressionSettings
{
    [JsonPropertyName("modelId")] public string ModelId { get; set; } = string.Empty;
    [JsonPropertyName("expressionFile")] public string ExpressionFile { get; set; } = string.Empty;
    [JsonPropertyName("showName")] public bool ShowName { get; set; } = true;
}
