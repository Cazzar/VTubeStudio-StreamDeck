using System;
using System.Collections.Generic;
using Cazzar.StreamDeck.VTubeStudio.VTubeStudioApi;
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

        private static readonly List<WeakReference<HotkeyCache>> instances = new();

        public IDictionary<string, List<Hotkey>> Hotkeys => _cache;
        
        private Dictionary<string, List<Hotkey>> _cache = new();
        
        public HotkeyCache(ModelCache modelCache, VTubeStudioWebsocketClient vts, ILogger<HotkeyCache> logger)
        {
            instances.Add(new(this));
            _vts = vts;
            _logger = logger;
            modelCache.ModelCacheUpdated += Update;
            VTubeStudioWebsocketClient.OnModelHotkeys += OnModelHotkeys;
        }

        private void OnModelHotkeys(object sender, ApiEventArgs<ModelHotkeysResponse> e)
        {
            if (_cache.ContainsKey(e.Response.ModelId))
                _cache.Remove(e.Response.ModelId);
            
            _cache.Add(e.Response.ModelId, e.Response.Hotkeys);

            Updated?.Invoke(this, new() {Hotkeys = _cache});
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
                _vts.Send(new ModelHotkeyRequest(model));
            }
        }

        public event EventHandler<HotkeyCacheUpdatedEventArgs> Updated;
    }

    public class HotkeyCacheUpdatedEventArgs : EventArgs
    {
        public IDictionary<string, List<Hotkey>> Hotkeys { get; init; }
    }
}
