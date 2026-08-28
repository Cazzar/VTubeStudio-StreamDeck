namespace Cazzar.Deck.VTubeStudio.Caches;

public record ExpressionStatus
{
    public string File => FileName;
    
    public required string FileName { get; init; }
    public required bool IsActive { get; set; }
    public required string Name { get; init; }
}
