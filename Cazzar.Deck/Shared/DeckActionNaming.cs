namespace Cazzar.Deck.Shared;

// Shared by the generator, the build task and ReflectionActionProvider so an omitted
// [DeckAction] id derives the same way whether it is read from source, metadata or reflection.
static class DeckActionNaming
{
    private const string Suffix = "Action";

    public static string Derive(string typeName) =>
        (typeName.Length > Suffix.Length && typeName.EndsWith(Suffix, StringComparison.Ordinal)
            ? typeName.Substring(0, typeName.Length - Suffix.Length)
            : typeName).ToLowerInvariant();

    public static string Qualify(string? packageUuid, string actionId) =>
        string.IsNullOrEmpty(packageUuid) ? actionId : packageUuid + "." + actionId;
}
