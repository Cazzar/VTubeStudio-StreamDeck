using Cazzar.StreamDeck.VTubeStudio.VTubeStudioApi.Requests;
using Newtonsoft.Json;

namespace Cazzar.StreamDeck.VTubeStudio.VTubeStudioApi
{
    public class RequestWrapper
    {
        [JsonProperty("apiName")] public string ApiName { get; } = "VTubeStudioPublicAPI";
        [JsonProperty("apiVersion")] public string ApiVersion { get; } = "1.0";
        [JsonProperty("messageType")] public string MessageType { get; init; }
        [JsonProperty("data")] public ApiRequest Data { get; init; }
        [JsonProperty("requestID")] public string RequestId { get; init; }

        public RequestWrapper(ApiRequest request)
        {
            MessageType = request.MessageType.ToString();
            Data = request;
        }
    }
}
