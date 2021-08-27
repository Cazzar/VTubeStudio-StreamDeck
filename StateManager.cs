using System;
using System.Timers;
using Cazzar.StreamDeck.VTubeStudio.VTubeStudioApi;
using Cazzar.StreamDeck.VTubeStudio.VTubeStudioApi.Requests;
using Cazzar.StreamDeck.VTubeStudio.VTubeStudioApi.Responses;

namespace Cazzar.StreamDeck.VTubeStudio
{
    public static class StateManager
    {
        public static string CurrentModelId { get; private set; }

        [EventRegistration]
        public static void RegisterEvents()
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

        private static void VTubeStudioWebsocketClientOnOnModelLoad(object sender, ApiEventArgs<ModelLoadResponse> e)
        {
            VTubeStudioWebsocketClient.Instance.Send(new CurrentModelRequest()); //request current model.
        }

        private static void VTubeStudioWebsocketClientOnOnCurrentModelInformation(object sender, ApiEventArgs<CurrentModelResponse> e)
        {
            CurrentModelId = e.Response.IsLoaded ? e.Response.Id : null;
        }

        private static void RequestStateTimer(object sender, ElapsedEventArgs e)
        {
            var vts = VTubeStudioWebsocketClient.Instance;

            if (!vts.IsAuthed)
                return;

            vts.Send(new CurrentModelRequest());
        }
    }
}
