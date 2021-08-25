using System;
using BarRaider.SdTools;
using Cazzar.StreamDeck.VTubeStudio.VTubeStudioApi;
using Cazzar.StreamDeck.VTubeStudio.VTubeStudioApi.Requests;
using Cazzar.StreamDeck.VTubeStudio.VTubeStudioApi.Responses;

namespace Cazzar.StreamDeck.VTubeStudio
{
    public class AuthManager
    {
        [EventRegistration]
        public static void RegisterEvents()
        {
            Logger.Instance.LogMessage(TracingLevel.INFO, "Registering events for GSM");
            //register events
            VTubeStudioWebsocketClient.SocketConnected += ConnectedToVts;
            VTubeStudioWebsocketClient.OnTokenResponse += GotVtsToken;
        }
        
        private static void GotVtsToken(object sender, ApiEventArgs<AuthenticateResponse> e)
        {
            GlobalSettingsManager.Instance.Settings.Token = e.Response.AuthToken;
            GlobalSettingsManager.Instance.SaveSettings();
            VTubeStudioWebsocketClient.Instance.Send(new AuthWithTokenRequest(e.Response.AuthToken));
        }

        private static void ConnectedToVts(object sender, EventArgs e)
        {
            var vts = VTubeStudioWebsocketClient.Instance;
            if (vts.IsAuthed) return;

            string token = GlobalSettingsManager.Instance.Settings.Token;

            if (string.IsNullOrEmpty(token))
            {
                vts.Send(new AuthenticateRequest());
                return;
            }
            
            vts.Send(new AuthWithTokenRequest(token));
        }
    }
}
