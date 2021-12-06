using System;
using System.Reflection.Metadata.Ecma335;
using Newtonsoft.Json;

namespace Cazzar.StreamDeck.VTubeStudio.VTubeStudioApi.Models
{
    public class Hotkey
    {
        [JsonProperty("name")]
        public string Name { get; set; }
        [JsonProperty("type")]
        public HotkeyAction Type { get; set; }
        [JsonProperty("file")]
        public string File { get; set; }
        [JsonProperty("hotkeyID")]
        public string Id { get; set; }

        public string ButtonTitle => Type switch
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
                _ => "Unknown",
            };
    }
}
