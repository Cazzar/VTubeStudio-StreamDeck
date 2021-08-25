using System;
using System.Collections.Generic;
using BarRaider.SdTools;
using Cazzar.StreamDeck.VTubeStudio.VTubeStudioApi;
using Cazzar.StreamDeck.VTubeStudio.VTubeStudioApi.Models;
using Cazzar.StreamDeck.VTubeStudio.VTubeStudioApi.Requests;
using Cazzar.StreamDeck.VTubeStudio.VTubeStudioApi.Responses;

namespace Cazzar.StreamDeck.VTubeStudio
{
    public class HotkeyCache : IDisposable
    {
        public static HotkeyCache Instance { get; } = new();
        
        public IDictionary<string, List<Hotkey>> Hotkeys => _cache;
        
        private Dictionary<string, List<Hotkey>> _cache = new();
        
        private HotkeyCache()
        {
            ModelCache.Instance.ModelCacheUpdated += Update;
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
            if (!VTubeStudioWebsocketClient.Instance.IsAuthed)
            {
                Logger.Instance.LogMessage(TracingLevel.INFO, "Not logged in, not updating");
                return;
            }

            _cache = new();

            foreach (var model in e.Models)
                VTubeStudioWebsocketClient.Instance.Send(new ModelHotkeyRequest(model));
        }

        public event EventHandler<HotkeyCacheUpdatedEventArgs> Updated;

        public void Dispose()
        {
            GC.SuppressFinalize(this);
            ModelCache.Instance.ModelCacheUpdated -= Update;
        }
    }

    public class HotkeyCacheUpdatedEventArgs : EventArgs
    {
        public IDictionary<string, List<Hotkey>> Hotkeys { get; init; }
    }
}
