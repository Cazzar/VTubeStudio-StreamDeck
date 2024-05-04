using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Threading;
using Cazzar.StreamDeck.VTubeStudio.VTubeStudioApi;
using Cazzar.StreamDeck.VTubeStudio.VTubeStudioApi.Events;
using Cazzar.StreamDeck.VTubeStudio.VTubeStudioApi.Models;
using Cazzar.StreamDeck.VTubeStudio.VTubeStudioApi.Requests;
using Cazzar.StreamDeck.VTubeStudio.VTubeStudioApi.Responses;
using Microsoft.Extensions.Logging;

namespace Cazzar.StreamDeck.VTubeStudio
{
    public class HotkeyCache
    {
        private readonly VTubeStudioWebsocketClient _vts;
        private readonly ILogger<HotkeyCache> _logger;

        private readonly SemaphoreSlim _semaphore = new SemaphoreSlim(1, 1);

        private static readonly List<WeakReference<HotkeyCache>> instances = new();

        public IDictionary<string, List<Hotkey>> Hotkeys => _cache;

        private ConcurrentDictionary<string, List<Hotkey>> _cache = new();

        public HotkeyCache(ModelCache modelCache, VTubeStudioWebsocketClient vts, ILogger<HotkeyCache> logger)
        {
            instances.Add(new(this));
            _vts = vts;
            _logger = logger;
            modelCache.ModelCacheUpdated += Update;
            VTubeStudioWebsocketClient.OnModelHotkeys += OnModelHotkeys;
            VTubeStudioWebsocketClient.OnModelConfigChangedEvent += OnModelConfigChangedEvent;
        }
        private void OnModelConfigChangedEvent(object sender, ApiEventArgs<ModelConfigChangedEvent> e)
        {
            if (!e.Response.HotkeyConfigChanged)
                return;

            _logger.LogInformation("Requesting hotkeys for {ModelId}", e.Response.ModelId);
            _vts.Send(new ModelHotkeyRequest(e.Response.ModelId));
        }

        ~HotkeyCache()
        {
            instances.Remove(new(this));
        }

        private async void OnModelHotkeys(object sender, ApiEventArgs<ModelHotkeysResponse> e)
        {
            _logger.LogInformation("Hotkeys for {ModelName} ({ModelId}) updated", e.Response.ModelName, e.Response.ModelId);
            await _semaphore.WaitAsync();

            try {
                if (_cache.ContainsKey(e.Response.ModelId))
                    _cache.Remove(e.Response.ModelId, out var _);

                _cache.AddOrUpdate(e.Response.ModelId, e.Response.Hotkeys, (key, old) => e.Response.Hotkeys);

                Updated?.Invoke(this, new() {Hotkeys = _cache});
            } catch (Exception ex) {
                _logger.LogError(ex, "Error updating hotkeys");
            } finally {
                _semaphore.Release();
            }

        }

        private void Update(object sender, ModelCacheUpdatedEventArgs e)
        {
            if (!_vts.IsAuthed)
            {
                return;
            }

            foreach (var model in e.Models)
            {
                _logger.LogInformation("Requesting hotkeys for {Model} ({ModelId})", model.Name, model.Id);
                _vts.Send(new ModelHotkeyRequest(model.Id));
            }
        }

        public event EventHandler<HotkeyCacheUpdatedEventArgs> Updated;
    }

    public class HotkeyCacheUpdatedEventArgs : EventArgs
    {
        public IDictionary<string, List<Hotkey>> Hotkeys { get; init; }
    }
}
