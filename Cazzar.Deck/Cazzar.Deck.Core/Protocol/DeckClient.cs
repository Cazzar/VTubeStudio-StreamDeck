using Cazzar.Deck.Abstractions.Protocol.Commands;
using Cazzar.Deck.Abstractions.Protocol;
using Cazzar.Deck.Abstractions.Surfaces;
using Cazzar.Deck.Abstractions;
using Microsoft.Extensions.Logging;
using System.Text.Json.Nodes;

namespace Cazzar.Deck.Core.Protocol;

public sealed class DeckClient(
    IDeckTransport transport,
    IDeckCodec codec,
    IDeckHostInfo host,
    ILogger<DeckClient> logger)
    : IWidgetSurface, ISettingsStore, IPropertyViewChannel, IEncoderSurface
{
    public ValueTask SetTitleAsync(ActionRef @ref, string? title, uint state = 0) => Send(new SetTitle(@ref, title, state));
    public ValueTask SetImageAsync(ActionRef @ref, string image, uint state = 0) => Send(new SetImage(@ref, image, state));
    public ValueTask SetStateAsync(ActionRef @ref, uint state) => Send(new SetState(@ref, state));
    public ValueTask ShowAlertAsync(ActionRef @ref) => Send(new ShowAlert(@ref));
    public ValueTask ShowOkAsync(ActionRef @ref) => Send(new ShowOk(@ref));

    public ValueTask SaveAsync(ActionRef @ref, JsonNode settings) => Send(new SaveSettings(@ref, settings));
    public ValueTask RequestAsync(ActionRef @ref) => Send(new RequestSettings(@ref));
    public ValueTask SaveGlobalAsync(JsonNode settings) => Send(new SaveGlobalSettings(settings));
    public ValueTask RequestGlobalAsync() => Send(new RequestGlobalSettings());

    public ValueTask SendAsync(ActionRef @ref, JsonNode payload) => Send(new SendToPropertyView(@ref, payload));

    public ValueTask SetFeedbackAsync(ActionRef @ref, JsonObject layout) => Send(new SetFeedback(@ref, layout));
    public ValueTask SetFeedbackLayoutAsync(ActionRef @ref, string layoutId) => Send(new SetFeedbackLayout(@ref, layoutId));

    public ValueTask Send(IDeckCommand command)
    {
        if (codec.Encode(command) is not { } frame)
        {
            throw new NotSupportedException(
                $"Host '{host.Name}' has no encoding for {command.GetType().Name}. " +
                "An action requiring it should not have been registered on this host.");
        }

        logger.LogTrace("-> {Frame}", frame);
        return new(transport.SendAsync(frame, CancellationToken.None));
    }
}
