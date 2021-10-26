using System;
using System.Timers;
using Cazzar.StreamDeck.VTubeStudio.VTubeStudioApi;
using Cazzar.StreamDeck.VTubeStudio.VTubeStudioApi.Requests;
using Cazzar.StreamDeck.VTubeStudio.VTubeStudioApi.Responses;

namespace Cazzar.StreamDeck.VTubeStudio
{
    public  class StateManager
    {
        private readonly VTubeStudioWebsocketClient _ws;
        public static string CurrentModelId { get; private set; }

        public StateManager(VTubeStudioWebsocketClient ws)
        {
            _ws = ws;
            RegisterEvents();
        }
        
        public void RegisterEvents()
        {
            VTubeStudioWebsocketClient.OnCurrentModelInformation += VTubeStudioWebsocketClientOnOnCurrentModelInformation;
            VTubeStudioWebsocketClient.OnModelLoad += VTubeStudioWebsocketClientOnOnModelLoad;
            VTubeStudioWebsocketClient.SocketConnected += (sender, e) => CurrentModelId = null; //remove it
            VTubeStudioWebsocketClient.SocketClosed += (sender, e) => CurrentModelId = null; //remove it

            Timer t = new();
            t.Interval = TimeSpan.FromSeconds(30).TotalMilliseconds;
            t.Elapsed += RequestStateTimer;
            t.Start();
        }

        private void VTubeStudioWebsocketClientOnOnModelLoad(object sender, ApiEventArgs<ModelLoadResponse> e)
        {
            _ws.Send(new CurrentModelRequest()); //request current model.
        }

        private void VTubeStudioWebsocketClientOnOnCurrentModelInformation(object sender, ApiEventArgs<CurrentModelResponse> e)
        {
            CurrentModelId = e.Response.IsLoaded ? e.Response.Id : null;
        }

        private void RequestStateTimer(object sender, ElapsedEventArgs e)
        {
            if (!_ws.IsAuthed)
                return;

            _ws.Send(new CurrentModelRequest());
        }
    }
}
