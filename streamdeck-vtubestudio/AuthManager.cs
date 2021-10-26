using System;
using Cazzar.StreamDeck.VTubeStudio.VTubeStudioApi;
using Cazzar.StreamDeck.VTubeStudio.VTubeStudioApi.Requests;
using Cazzar.StreamDeck.VTubeStudio.VTubeStudioApi.Responses;

namespace Cazzar.StreamDeck.VTubeStudio
{
    public class AuthManager : IAuthManger
    {
        // private VTubeStudioWebsocketClient vts;
        private readonly GlobalSettingsManager _gsm;

        public AuthManager(GlobalSettingsManager gsm)
        {
            _gsm = gsm;
        }

        public string Token
        {
            get => _gsm.Settings.Token;
            set
            {
                _gsm.Settings.Token = value;
                _gsm.SaveSettings();
            }
        }
    }

    public interface IAuthManger
    {
        public string Token { get; set; }
    }
}
