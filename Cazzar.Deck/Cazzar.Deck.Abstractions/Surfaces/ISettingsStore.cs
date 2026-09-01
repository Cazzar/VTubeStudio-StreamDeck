using System.Text.Json.Nodes;

namespace Cazzar.Deck.Abstractions.Surfaces;

public interface ISettingsStore
{
    ValueTask SaveAsync(ActionRef @ref, JsonNode settings);
    ValueTask RequestAsync(ActionRef @ref);
    ValueTask SaveGlobalAsync(JsonNode settings);
    ValueTask RequestGlobalAsync();
}
