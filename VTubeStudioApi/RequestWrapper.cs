using Cazzar.StreamDeck.VTubeStudio.VTubeStudioApi.Requests;
using Newtonsoft.Json;

namespace Cazzar.StreamDeck.VTubeStudio.VTubeStudioApi
{
    public class RequestWrapper<T>
    {
        [JsonProperty("apiName")] public string ApiName { get; } = "VTubeStudioPublicAPI";
        [JsonProperty("apiVersion")] public string ApiVersion { get; } = "1.0";
        [JsonProperty("messageType")] public string MessageType { get; init; }
        [JsonProperty("data")] public ApiRequest<T> Data { get; init; }

        public RequestWrapper(ApiRequest<T> request)
        {
            MessageType = request.MessageType.ToString();
            Data = request;
        }
    }
}
