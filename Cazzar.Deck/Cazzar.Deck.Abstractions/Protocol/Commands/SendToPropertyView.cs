using System.Text.Json.Nodes;

namespace Cazzar.Deck.Abstractions.Protocol.Commands;

public sealed record SendToPropertyView(ActionRef Ref, JsonNode Payload) : IDeckCommand;
