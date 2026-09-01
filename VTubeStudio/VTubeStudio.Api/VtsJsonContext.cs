using System.Text.Json.Serialization;
using VTubeStudio.Api.Events;
using VTubeStudio.Api.Models;
using VTubeStudio.Api.Requests;
using VTubeStudio.Api.Responses;

namespace VTubeStudio.Api;

[JsonSerializable(typeof(ApiStateRequest))]
[JsonSerializable(typeof(AuthenticationRequest))]
[JsonSerializable(typeof(AuthenticationTokenRequest))]
[JsonSerializable(typeof(AvailableModelsRequest))]
[JsonSerializable(typeof(CurrentModelRequest))]
[JsonSerializable(typeof(EventSubscriptionRequest))]
[JsonSerializable(typeof(ExpressionActivationRequest))]
[JsonSerializable(typeof(ExpressionStateRequest))]
[JsonSerializable(typeof(HotkeyTriggerRequest))]
[JsonSerializable(typeof(ModelHotkeysRequest))]
[JsonSerializable(typeof(ModelLoadRequest))]
[JsonSerializable(typeof(MoveModelRequest))]
[JsonSerializable(typeof(ApiErrorResponse))]
[JsonSerializable(typeof(ApiStateResponse))]
[JsonSerializable(typeof(AuthenticationResponse))]
[JsonSerializable(typeof(AuthenticationTokenResponse))]
[JsonSerializable(typeof(AvailableModelsResponse))]
[JsonSerializable(typeof(CurrentModelResponse))]
[JsonSerializable(typeof(ExpressionStateResponse))]
[JsonSerializable(typeof(HotkeyTriggerResponse))]
[JsonSerializable(typeof(ModelHotkeysResponse))]
[JsonSerializable(typeof(ModelLoadResponse))]
[JsonSerializable(typeof(HotkeyTriggeredEvent))]
[JsonSerializable(typeof(ModelConfigChangedEvent))]
[JsonSerializable(typeof(ModelLoadedEvent))]
[JsonSerializable(typeof(ModelMovedEvent))]
[JsonSerializable(typeof(Expression))]
[JsonSerializable(typeof(ExpressionHotkeyReference))]
[JsonSerializable(typeof(Hotkey))]
[JsonSerializable(typeof(Model))]
[JsonSerializable(typeof(ModelPosition))]
[JsonSerializable(typeof(ExpressionToggledEvent))]
sealed partial class VtsJsonContext : JsonSerializerContext;
