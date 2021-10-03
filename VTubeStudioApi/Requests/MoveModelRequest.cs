using Cazzar.StreamDeck.VTubeStudio.VTubeStudioApi.Responses;
using Newtonsoft.Json;

namespace Cazzar.StreamDeck.VTubeStudio.VTubeStudioApi.Requests
{
    [JsonObject(ItemNullValueHandling = NullValueHandling.Ignore)]
    public class MoveModelRequest : ApiRequest
    {
        [JsonProperty("timeInSeconds", NullValueHandling = NullValueHandling.Include)] public double? TimeInSeconds { get; set; }
        [JsonProperty("valuesAreRelativeToModel")] public bool? RelativeMove { get; set; }
        [JsonProperty("positionX")] public double? PositionX { get; set; }
        [JsonProperty("positionY")] public double? PositionY { get; set; }
        [JsonProperty("rotation")] public double? Rotation { get; set; }
        [JsonProperty("size")] public int? Size { get; set; }
        
        public override RequestType MessageType { get; } = RequestType.MoveModelRequest;
    }
}







