using System.Diagnostics.CodeAnalysis;
using Cazzar.StreamDeck.VTubeStudio.VTubeStudioApi.Models;
using Cazzar.StreamDeck.VTubeStudio.VTubeStudioApi.Responses;
using Newtonsoft.Json;

namespace Cazzar.StreamDeck.VTubeStudio.VTubeStudioApi.Requests;

public class ModelHotkeyRequest(string modelId) : ApiRequest
{
    [JsonProperty("modelID")]
    public required string ModelId { get; set; } = modelId;

    public override RequestType MessageType => RequestType.HotkeysInCurrentModelRequest;
}