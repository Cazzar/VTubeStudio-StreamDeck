using System.Text.Json.Nodes;

namespace Cazzar.Deck.Abstractions.Protocol.Commands;

public sealed record SaveGlobalSettings(JsonNode Settings) : IDeckCommand;
