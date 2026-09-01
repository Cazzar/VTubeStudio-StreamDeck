using Cazzar.Deck.VTubeStudio.Actions.Encoders;
using Cazzar.Deck.VTubeStudio.Actions.Expressions;
using Cazzar.Deck.VTubeStudio.Actions.Hotkeys;
using Cazzar.Deck.VTubeStudio.Actions.Models;
using Cazzar.Deck.VTubeStudio.Actions.Movement;
using System.Text.Json.Serialization;

namespace Cazzar.Deck.VTubeStudio;

[JsonSerializable(typeof(VtsGlobalSettings))]
[JsonSerializable(typeof(VtsInfoPayload))]
[JsonSerializable(typeof(ChangeModelAction.Options), TypeInfoPropertyName = "ChangeModelOptions")]
[JsonSerializable(typeof(ExpressionSettings))]
[JsonSerializable(typeof(MoveAxisAction.Options), TypeInfoPropertyName = "MoveAxisOptions")]
[JsonSerializable(typeof(MoveSettings))]
[JsonSerializable(typeof(ReloadCurrentModelAction.Options), TypeInfoPropertyName = "ReloadCurrentModelOptions")]
[JsonSerializable(typeof(RotateModelAction.Options), TypeInfoPropertyName = "RotateModelOptions")]
[JsonSerializable(typeof(ScaleModelAction.Options), TypeInfoPropertyName = "ScaleModelOptions")]
[JsonSerializable(typeof(TriggerHotkeyAction.Options), TypeInfoPropertyName = "TriggerHotkeyOptions")]
[JsonSerializable(typeof(ZoomModelAction.Options), TypeInfoPropertyName = "ZoomModelOptions")]
sealed partial class VtsDeckJsonContext : JsonSerializerContext;
