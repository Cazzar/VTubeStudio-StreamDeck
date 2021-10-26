using System;
using System.Diagnostics;
using System.Drawing;
using System.Reflection.Metadata.Ecma335;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices.ComTypes;
using System.Threading.Tasks.Dataflow;
using Cazzar.StreamDeck.VTubeStudio.VTubeStudioApi;
using Cazzar.StreamDeck.VTubeStudio.VTubeStudioApi.Requests;
using Cazzar.StreamDeck.VTubeStudio.VTubeStudioApi.Responses;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Newtonsoft.Json.Linq;
using StreamDeckLib;
using StreamDeckLib.Extensions.Models;
using StreamDeckLib.Models;

namespace Cazzar.StreamDeck.VTubeStudio
{
    public class GlobalSettings
    {
        public string Token { get; set; } = string.Empty;
        public string Host { get; set; } = "127.0.0.1";
        public ushort? Port { get; set; } = 8001;
    }

    public class GlobalSettingsManager : IGlobalSettingsHandler
    {
        public GlobalSettings Settings { get; private set; } = new();
        private readonly IServiceProvider _serviceProvider;
        private readonly ILogger _logger;
        private readonly StreamDeckLaunchOptions _options;

        public GlobalSettingsManager(IServiceProvider serviceProvider, IOptions<StreamDeckLaunchOptions> options, ILogger<GlobalSettingsManager> logger)
        {
            _options = options.Value;
            _serviceProvider = serviceProvider;
            _logger = logger;
            // BarRaider.SdTools.GlobalSettingsManager.Instance.OnReceivedGlobalSettings += SdGlobalSettingsReceived;
            // VTubeStudioWebsocketClient.Instance.ConnectIfNeeded();
        }

        public void GotGlobalSettings(dynamic obj)
        {
            Debugger.Break();
        }

        public void RequestGlobalSettings()
        {
            _serviceProvider.GetService<IStreamDeckConnection>()!.SendMessage(new GetGlobalSettings() { Context = _options.PluginUuid });
        }

        // private void SdGlobalSettingsReceived(object sender, ReceivedGlobalSettingsPayload e)
        // {
        //     var current = Settings;
        //     
        //     Settings = e.Settings.ToObject<GlobalSettings>();
        //     if (Settings == null)
        //     {
        //         BarRaider.SdTools.GlobalSettingsManager.Instance.SetGlobalSettings(JObject.FromObject(new GlobalSettings()));
        //         return;
        //     }
        //
        //     if (current.Token == Settings.Token && !string.IsNullOrEmpty(Settings.Token)) return;
        //     
        //     if (current.Host != Settings.Host || current.Port != Settings.Port)
        //         VTubeStudioWebsocketClient.Instance.SetConnection(Settings.Host ?? "127.0.0.1", Settings.Port ?? 8001);
        //
        //     
        //     Logger.Instance.LogMessage(TracingLevel.INFO, "Token changed!");
        //     TokenChanged?.Invoke(this, Settings);
        // }
        //
        // public void SaveSettings()
        // {
        //     var settings = JObject.FromObject(Settings);
        //     
        //     BarRaider.SdTools.GlobalSettingsManager.Instance.SetGlobalSettings(settings);
        // }
        //
        public event EventHandler<GlobalSettings> TokenChanged;
        //
        // public void Load()
        // {
        //     if (_loaded) return;
        //     RequestGlobalSettings();
        //     _loaded = true;
        // }
        //
        // public void RequestGlobalSettings()
        // {
        //     BarRaider.SdTools.GlobalSettingsManager.Instance.RequestGlobalSettings();
        // }
        //
        public void SetVts(string host, ushort port)
        {
            Settings.Host = host;
            Settings.Port = port;
            
            _serviceProvider.GetRequiredService<VTubeStudioWebsocketClient>().SetConnection(host, port);
                 
            SaveSettings();
        }

        public void SaveSettings()
        {
            _serviceProvider.GetRequiredService<IStreamDeckConnection>().SendMessage(new SetGlobalSettings()
            {
                Context = _options.PluginUuid,
                Payload = Settings,
            });
        }

        public void GotGlobalSettings(JToken token)
        {
            var current = Settings;
            
            Settings = token.ToObject<GlobalSettings>();
            if (Settings == null)
            {
                Settings = new();
                SaveSettings();
                return;
            }
            
            if (current.Token == Settings.Token && !string.IsNullOrEmpty(Settings.Token)) return;
            
            //this is a bad hack to work around a circular dependency.
            _serviceProvider.GetRequiredService<VTubeStudioWebsocketClient>().SetConnection(Settings.Host ?? "127.0.0.1", Settings.Port ?? 8001);
            _logger.LogInformation("Token changed!");
            TokenChanged?.Invoke(this, Settings);
        }
    }
}
