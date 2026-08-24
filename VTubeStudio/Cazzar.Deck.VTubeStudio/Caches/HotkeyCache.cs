using System.Collections.Concurrent;
using VTubeStudio.Api;
using VTubeStudio.Api.Models;
using VTubeStudio.Api.Requests;

namespace Cazzar.Deck.VTubeStudio.Caches;

public sealed class HotkeyCache
{
    private readonly IVTubeStudio _vts;
    private readonly ModelCache _models;
    private readonly ConcurrentDictionary<string, IReadOnlyList<Hotkey>> _hotkeys = new();

    public HotkeyCache(IVTubeStudio vts, ModelCache models)
    {
        _vts = vts;
        _models = models;

        vts.ModelHotkeys += (_, e) =>
        {
            if (string.IsNullOrEmpty(e.Response.ModelId)) return;

            _hotkeys[e.Response.ModelId] = e.Response.Hotkeys;
            Updated?.Invoke(this, new(Hotkeys));
        };

        vts.AvailableModels += (_, e) =>
        {
            foreach (var model in e.Response.Models)
                vts.Send(new ModelHotkeysRequest(model.Id));
        };

        vts.ModelConfigChanged += (_, e) =>
        {
            if (e.Response.HotkeyConfigChanged) vts.Send(new ModelHotkeysRequest(e.Response.ModelId));
        };
    }

    public IReadOnlyDictionary<string, IReadOnlyList<Hotkey>> Hotkeys => _hotkeys;

    public event EventHandler<HotkeyCacheUpdatedEventArgs>? Updated;

    public IReadOnlyList<Hotkey> For(string? modelId) =>
        modelId is not null && _hotkeys.TryGetValue(modelId, out var hotkeys) ? hotkeys : [];

    public void Refresh()
    {
        foreach (var model in _models.Models)
            _vts.Send(new ModelHotkeysRequest(model.Id));
    }
}
