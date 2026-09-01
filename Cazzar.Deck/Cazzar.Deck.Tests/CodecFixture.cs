using Cazzar.Deck.Abstractions;
using System.Text.Json.Nodes;

namespace Cazzar.Deck.Tests;

static class CodecFixture
{
    public static readonly ActionRef Ref = new("ctx-1", "dev.cazzar.action");

    public static string? Event(string? frame) => (string?)JsonNode.Parse(frame!)!["event"];
}
