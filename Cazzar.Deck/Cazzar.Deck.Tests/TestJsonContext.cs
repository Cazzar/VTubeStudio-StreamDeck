using System.Text.Json.Serialization;

namespace Cazzar.Deck.Tests;

[JsonSerializable(typeof(PortableAction.Options), TypeInfoPropertyName = "PortableOptions")]
[JsonSerializable(typeof(CommandingAction.Options), TypeInfoPropertyName = "CommandingOptions")]
[JsonSerializable(typeof(DerivedIdAction.Options), TypeInfoPropertyName = "DerivedIdOptions")]
sealed partial class TestJsonContext : JsonSerializerContext;
