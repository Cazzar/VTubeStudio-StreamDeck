using System.Text.Json.Nodes;

namespace Cazzar.Deck.Abstractions.Protocol.Commands;

[RequiresFeature(DeckFeature.Encoder)]
public sealed record SetFeedback(ActionRef Ref, JsonObject Layout) : IDeckCommand;
