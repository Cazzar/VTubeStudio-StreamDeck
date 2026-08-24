using System.Reflection;

namespace Cazzar.Deck.VTubeStudio;

public static class EmbeddedIcon
{
    public static string? Read(Assembly assembly, string fileName)
    {
        if (assembly.GetManifestResourceNames().FirstOrDefault(n => n.EndsWith(fileName, StringComparison.Ordinal)) is not { } resource)
            return null;

        using var stream = assembly.GetManifestResourceStream(resource)!;
        using var bytes = new MemoryStream();
        stream.CopyTo(bytes);

        return Convert.ToBase64String(bytes.ToArray());
    }
}
