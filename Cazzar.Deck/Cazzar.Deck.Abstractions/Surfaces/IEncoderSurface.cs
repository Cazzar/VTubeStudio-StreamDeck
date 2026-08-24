using System.Text.Json.Nodes;

namespace Cazzar.Deck.Abstractions.Surfaces;

[RequiresFeature(DeckFeature.Encoder)]
public interface IEncoderSurface
{
    ValueTask SetFeedbackAsync(ActionRef @ref, JsonObject layout);
    ValueTask SetFeedbackLayoutAsync(ActionRef @ref, string layoutId);
}
