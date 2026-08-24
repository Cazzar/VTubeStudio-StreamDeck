using Microsoft.CodeAnalysis;

namespace Cazzar.Deck.Generators;

static class Diagnostics
{
    public static readonly DiagnosticDescriptor DuplicateActionId = new(
        "DECK001",
        "Duplicate deck action id",
        "Action id '{0}' is declared more than once; only the first registration wins at runtime",
        "Deck",
        DiagnosticSeverity.Error,
        isEnabledByDefault: true);

    public static readonly DiagnosticDescriptor ContradictoryHosts = new(
        "DECK002",
        "Action requires a capability its declared hosts cannot provide",
        "'{0}' requires an encoder but declares Hosts = DeckHost.CreatorCentral, which has none",
        "Deck",
        DiagnosticSeverity.Error,
        isEnabledByDefault: true);

    public static readonly DiagnosticDescriptor UncoveredSettings = new(
        "DECK003",
        "Action settings type is not covered by a JsonSerializerContext",
        "Settings type '{0}' is not declared with [JsonSerializable] on any JsonSerializerContext in this assembly, so a trimmed build cannot serialize it",
        "Deck",
        DiagnosticSeverity.Error,
        isEnabledByDefault: true);
}
