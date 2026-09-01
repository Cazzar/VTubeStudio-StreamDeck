using Cazzar.Deck.Abstractions.Actions.Handlers;
using Cazzar.Deck.Abstractions.Surfaces;
using Cazzar.Deck.Abstractions;
using Cazzar.Deck.Core;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using VTubeStudio.Api;

namespace Cazzar.Deck.VTubeStudio;

// Settings flow one way — store to client — so neither side needs to resolve the other.
public sealed class VtsSettingsStore(
    ISettingsStore settings,
    IOptionsMonitor<VtsConnectionOptions> connection,
    ILogger<VtsSettingsStore> logger) : IGlobalSettingsHandler, IVtsSettingsStore
{
    private VtsGlobalSettings _current = new();

    public bool IsLoaded { get; private set; }

    public string? Token
    {
        get => _current.Token;
        set
        {
            if (_current.Token == value) return;

            _current.Token = value;
            Save();
        }
    }

    public void GotGlobalSettings(IPayload payload)
    {
        _current = payload.As<VtsGlobalSettings>() ?? new VtsGlobalSettings();
        IsLoaded = true;

        logger.LogInformation("Global settings loaded; VTube Studio at {Host}:{Port}", _current.Host, _current.Port);

        connection.CurrentValue.Host = _current.Host;
        connection.CurrentValue.Port = _current.Port;
    }

    public void SetEndpoint(string host, ushort port)
    {
        _current.Host = host;
        _current.Port = port;
        Save();
    }

    private void Save() => _ = settings.SaveGlobalAsync(DeckJson.ToNode(_current)!);
}
