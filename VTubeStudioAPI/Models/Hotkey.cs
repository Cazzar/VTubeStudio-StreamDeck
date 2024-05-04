using System;
using System.Reflection.Metadata.Ecma335;
using Newtonsoft.Json;

namespace Cazzar.StreamDeck.VTubeStudio.VTubeStudioApi.Models;

public class Hotkey
{
    [JsonProperty("name")]
    public required string Name { get; set; }
    
    [JsonProperty("type")]
    public required string Type { get; set; }
    
    [JsonProperty("file")]
    public required string File { get; set; }
    
    [JsonProperty("description")]
    public required string Description { get; set; }
    
    [JsonProperty("hotkeyID")]
    public required string Id { get; set; }
}