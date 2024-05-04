using System;

namespace Cazzar.StreamDeck.VTubeStudio.VTubeStudioApi;

public static class Extensions
{
    public static string TrimEnd(this string source, string value) => 
        source.EndsWith(value) ? 
            source.Remove(source.LastIndexOf(value, StringComparison.OrdinalIgnoreCase)) : 
            source;
}