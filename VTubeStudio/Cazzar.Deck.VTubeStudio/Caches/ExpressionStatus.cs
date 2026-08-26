using System.Runtime.CompilerServices;

namespace Cazzar.Deck.VTubeStudio.Caches;

public record ExpressionStatus
{
    public string File => FileName;
    
    public required string FileName { get; init; }
    public required bool IsActive { get; set; }
    public required string Name { get; init; }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    private static string TrimSuffix(string suffix, string subject)
    {
        return subject.EndsWith(suffix, StringComparison.OrdinalIgnoreCase)
            ? subject[..^suffix.Length]
            : subject;
    }
}
