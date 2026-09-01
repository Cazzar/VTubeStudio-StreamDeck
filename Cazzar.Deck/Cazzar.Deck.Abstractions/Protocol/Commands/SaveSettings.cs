using System.Text.Json.Nodes;

namespace Cazzar.Deck.Abstractions.Protocol.Commands;

public sealed record SaveSettings(ActionRef Ref, JsonNode Settings) : IDeckCommand;
