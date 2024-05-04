using System;
using Cazzar.StreamDeck.VTubeStudio.VTubeStudioApi;
using Cazzar.StreamDeck.VTubeStudio.VTubeStudioApi.Models;

namespace Cazzar.StreamDeck.VTubeStudio;

public static class HotKeyNameExtensions
{
    public static string GetHotkeyButtonTitle(this Hotkey key)
    {
        if (!string.IsNullOrEmpty(key.Name))
            return key.Name.Trim();
                
        if (!Enum.TryParse(key.Type, true, out HotkeyAction action))
        {
            return key.Description;
        } 
                
        return action switch
        {
            HotkeyAction.Unset => "Unset",
            HotkeyAction.TriggerAnimation => key.File.TrimEnd(".motion3.json"),
            HotkeyAction.ChangeIdleAnimation => key.File.TrimEnd(".motion3.json"),
            HotkeyAction.ToggleExpression => key.File.TrimEnd(".exp3.json"),
            HotkeyAction.RemoveAllExpressions => "Remove All Expressions",
            HotkeyAction.MoveModel => "Move Model",
            HotkeyAction.ChangeBackground => key.File,
            HotkeyAction.ReloadMicrophone => "Reload Microphone",
            HotkeyAction.ReloadTextures => "Reload Textures",
            HotkeyAction.CalibrateCam => "Calibrate Camera",
            HotkeyAction.ChangeVTSModel => key.File.TrimEnd(".vtube.json"),
            HotkeyAction.TakeScreenshot => "Take Screenshot",
            HotkeyAction.ScreenColorOverlay => "Screen Color Overlay",
            HotkeyAction.RemoveAllItems => "Remove All Items",
            HotkeyAction.ToggleItemScene => $"Toggle Item Scene {key.File}",
            _ => "Unknown",
        };
    }
}
