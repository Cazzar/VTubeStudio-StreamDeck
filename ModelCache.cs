using System;
using System.Collections.Generic;
using System.ComponentModel.Design.Serialization;
using System.Linq;
using System.Security.Cryptography.X509Certificates;
using System.Threading;
using System.Threading.Tasks;
using BarRaider.SdTools;
using Cazzar.StreamDeck.VTubeStudio.Models;
using Cazzar.StreamDeck.VTubeStudio.VTubeStudioApi;
using Cazzar.StreamDeck.VTubeStudio.VTubeStudioApi.Models;
using Cazzar.StreamDeck.VTubeStudio.VTubeStudioApi.Requests;
using Cazzar.StreamDeck.VTubeStudio.VTubeStudioApi.Responses;
using Microsoft.VisualBasic;
using Timer = System.Timers.Timer;

namespace Cazzar.StreamDeck.VTubeStudio
{
    public class ModelCache : IDisposable
    {
        public static ModelCache Instance { get; private set; } = new ModelCache();
        
        public IEnumerable<Model> Models => _models;
        
        private List<Model> _models = new();
        
        private readonly Timer _updateTimer = new Timer(TimeSpan.FromMinutes(10).TotalMilliseconds);

        private ModelCache()
        {
            VTubeStudioWebsocketClient.OnAuthenticationResponse += (sender, args) =>
            {
                if (args.Response.Authenticated)
                    Update();
            };
            
            VTubeStudioWebsocketClient.OnAvailableModels += OnAvailableModels;
            
            _updateTimer.Elapsed += (sender, args) => Update(); 
            _updateTimer.Start();
            
        }

        private void OnAvailableModels(object sender, ApiEventArgs<AvailableModelsResponse> e)
        {
            _models = e.Response.Models.Distinct(new Model.IdEqualityComparer()).ToList();
            ModelCacheUpdated?.Invoke(this, new () {Models = _models});
        }

        internal void Update()
        {
            if (!VTubeStudioWebsocketClient.Instance.IsAuthed)
            {
                Logger.Instance.LogMessage(TracingLevel.INFO, "Not logged in, not updating");
                return;
            }

            VTubeStudioWebsocketClient.Instance.Send(new AvailableModelsRequest());
        }

        public event EventHandler<ModelCacheUpdatedEventArgs> ModelCacheUpdated;

        public void Dispose()
        {
            _updateTimer.Dispose();
        }
    }

    public class ModelCacheUpdatedEventArgs : EventArgs
    {
        public List<Model> Models { get; set; }
    }
}
