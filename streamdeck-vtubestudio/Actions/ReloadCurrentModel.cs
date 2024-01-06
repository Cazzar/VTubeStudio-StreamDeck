using System;
using Cazzar.StreamDeck.VTubeStudio.VTubeStudioApi;
using Cazzar.StreamDeck.VTubeStudio.VTubeStudioApi.Requests;
using Cazzar.StreamDeck.VTubeStudio.VTubeStudioApi.Responses;
using Microsoft.Extensions.Logging;
using StreamDeckLib;

namespace Cazzar.StreamDeck.VTubeStudio.Actions
{
    [StreamDeckAction("dev.cazzar.vtubestudio.reloadmodel")]
    public class ReloadCurrentModel : BaseAction<ReloadCurrentModel.PluginSettings>, IDisposable
    {
        private string _requestId;

        public class PluginSettings { }

        public ReloadCurrentModel(GlobalSettingsManager gsm, VTubeStudioWebsocketClient vts, IStreamDeckConnection isd, ILogger<ReloadCurrentModel> logger) : base(gsm, vts, isd, logger)
        {
            VTubeStudioWebsocketClient.OnCurrentModelInformation += OnCurrentModelInfo;
        }

        private void OnCurrentModelInfo(object sender, ApiEventArgs<CurrentModelResponse> e)
        {
            if (e.RequestId != _requestId) return;
            
            Vts.Send(new ModelLoadRequest(e.Response.Id));
            ShowOk();
        }

        protected override void Pressed()
        {
            Vts.Send(new CurrentModelRequest(), _requestId = Guid.NewGuid().ToString());
        }

        protected override void Released() { }

        protected override void SettingsUpdated(PluginSettings oldSettings, PluginSettings newSettings) { }

        public void Dispose()
        {
            VTubeStudioWebsocketClient.OnCurrentModelInformation -= OnCurrentModelInfo;
        }
    }
}