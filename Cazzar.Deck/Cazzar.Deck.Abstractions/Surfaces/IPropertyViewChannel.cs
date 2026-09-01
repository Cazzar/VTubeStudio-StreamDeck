using System.Text.Json.Nodes;

namespace Cazzar.Deck.Abstractions.Surfaces;

public interface IPropertyViewChannel
{
    ValueTask SendAsync(ActionRef @ref, JsonNode payload);
}
