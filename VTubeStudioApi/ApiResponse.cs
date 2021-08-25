using Cazzar.StreamDeck.VTubeStudio.VTubeStudioApi.Responses;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace Cazzar.StreamDeck.VTubeStudio.VTubeStudioApi
{
    class ApiResponse<T>
    {
        [JsonProperty("apiName")]
        public string ApiName { get; set; }

        [JsonProperty("apiVersion")]
        public string ApiVersion { get; set; }
        
        [JsonProperty("messageType")] 
        public string MessageType { get; init; }
        
        [JsonProperty("data")] 
        public T Data { get; init; }
    }
    
    class ApiResponse
    {
        [JsonProperty("apiName")]
        public string ApiName { get; set; }

        [JsonProperty("apiVersion")]
        public string ApiVersion { get; set; }
        
        [JsonProperty("messageType")] 
        public ResponseType MessageType { get; init; }
        
        [JsonProperty("data")] 
        public JObject Data { get; init; }
    }
}