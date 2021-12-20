using System;
using System.Reflection.Metadata.Ecma335;
using Newtonsoft.Json;
using StreamDeckLib;

namespace Cazzar.StreamDeck.VTubeStudio.VTubeStudioApi.Models
{
    public class Hotkey
    {
        [JsonProperty("name")]
        public string Name { get; set; }
        [JsonProperty("type")]
        public string Type { get; set; }
        [JsonProperty("file")]
        public string File { get; set; }
        [JsonProperty("description")]
        public string Description { get; set; }
        [JsonProperty("hotkeyID")]
        public string Id { get; set; }

        public string ButtonTitle
        {
            get
            {
                if (!string.IsNullOrEmpty(Name))
                    return Name.Trim();
                
                if (!Enum.TryParse(Type, true, out HotkeyAction action))
                {
                    return Description;
                } 
                
                return action switch
                {
                    HotkeyAction.Unset => "Unset",
                    HotkeyAction.TriggerAnimation => File.TrimEnd(".motion3.json"),
                    HotkeyAction.ChangeIdleAnimation => File.TrimEnd(".motion3.json"),
                    HotkeyAction.ToggleExpression => File.TrimEnd(".exp3.json"),
                    HotkeyAction.RemoveAllExpressions => "Remove All Expressions",
                    HotkeyAction.MoveModel => "Move Model",
                    HotkeyAction.ChangeBackground => File,
                    HotkeyAction.ReloadMicrophone => "Reload Microphone",
                    HotkeyAction.ReloadTextures => "Reload Textures",
                    HotkeyAction.CalibrateCam => "Calibrate Camera",
                    HotkeyAction.ChangeVTSModel => File.TrimEnd(".vtube.json"),
                    HotkeyAction.TakeScreenshot => "Take Screenshot",
                    HotkeyAction.ScreenColorOverlay => "Screen Color Overlay",
                    HotkeyAction.RemoveAllItems => "Remove All Items",
                    HotkeyAction.ToggleItemScene => $"Toggle Item Scene {File}",
                    _ => "Unknown",
                };
            }
        } 
    }
}
