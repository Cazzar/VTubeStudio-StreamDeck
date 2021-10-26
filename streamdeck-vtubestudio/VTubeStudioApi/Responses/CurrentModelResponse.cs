using Newtonsoft.Json;

namespace Cazzar.StreamDeck.VTubeStudio.VTubeStudioApi.Responses
{
    public class CurrentModelResponse
    {
        [JsonProperty("modelLoaded")] public bool IsLoaded { get; set; }
        [JsonProperty("modelName")] public string Name { get; set; }
        [JsonProperty("modelID")] public string Id { get; set; }
        [JsonProperty("vtsModelName")] public string vtsModelName { get; set; }
        [JsonProperty("vtsModelIconName")] public string vtsModelIconNAme { get; set; }
        [JsonProperty("live2DModelName")] public string Live2DModelName { get; set; }
        [JsonProperty("modelLoadTime")] public int ModelLoadTime { get; set; }
        [JsonProperty("timeSinceModelLoaded")] public int TimeSinceModelLoaded { get; set; }
        [JsonProperty("numberOfLive2DParameters")] public int Live2DParamCount { get; set; }
        [JsonProperty("numberOfLive2DArtmeshes")] public int Live2DArtMeshCount { get; set; }
        [JsonProperty("hasPhysicsFile")] public bool HasPhysicsFile { get; set; }
        [JsonProperty("numberOfTextures")] public int NumberOfTextures { get; set; }
        [JsonProperty("textureResolution")] public int TextureResolution { get; set; }
        [JsonProperty("modelPosition")] public ModelPosition ModelPosition { get; set; }
    }
    
    public class ModelPosition
    {
        [JsonProperty("positionX")] public double X { get; set; }
        [JsonProperty("positionY")] public double Y { get; set; }
        [JsonProperty("rotation")] public double Rotation { get; set; }
        [JsonProperty("size")] public double Size { get; set; }
    }
}










