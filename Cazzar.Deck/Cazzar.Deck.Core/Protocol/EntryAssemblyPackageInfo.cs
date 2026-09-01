using System.Reflection;
using Cazzar.Deck.Abstractions.Protocol;

namespace Cazzar.Deck.Core.Protocol;

public static class EntryAssemblyPackageInfo
{
    public const string MetadataKey = "DeckPackageUuid";

    public static IDeckPackageInfo Read() => new DeckPackageInfo(Uuid(Assembly.GetEntryAssembly()));

    public static string Uuid(Assembly? assembly) =>
        assembly?.GetCustomAttributes<AssemblyMetadataAttribute>()
            .FirstOrDefault(a => a.Key == MetadataKey)?.Value ?? string.Empty;
}
