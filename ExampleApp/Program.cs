using System.Diagnostics;
using System.IO;
using System.Threading;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json.Linq;
using StreamDeckLib;
using StreamDeckLib.Extensions;
using StreamDeckLib.Models;

namespace ExampleApp
{
    class Program
    {
        static void Main(string[] argv)
        {
            Process.GetProcessesByName("streamdeck-vtubestudio")[0].Kill();
            Thread.Sleep(1000);
            var args = System.IO.File.ReadAllLines(@"C:\Users\me\AppData\Roaming\Elgato\StreamDeck\Plugins\dev.cazzar.streamdeck.vtubestudio.sdPlugin\argv.txt");

            var config = new ConfigurationBuilder()
                .SetBasePath(System.IO.Directory.GetCurrentDirectory()) //From NuGet Package Microsoft.Extensions.Configuration.Json
                .AddJsonFile("appsettings.json", optional: true, reloadOnChange: true)
                .Build();

            
            Host.CreateDefaultBuilder(args)
                .ConfigureStreamDeckToolkit(args)
                .ConfigureServices(
                    (host, services) =>
                    {
                        services.AddStreamDeck(host.Configuration);
                        services.AddSingleton<IGlobalSettingsHandler, GlobalSettings>();
                        services.AddLogging();
                    })
                .Build()
                .Run();
        }
    }

    class GlobalSettings: IGlobalSettingsHandler
    {
        public void GotGlobalSettings(JToken token)
        {
            Debugger.Break();
        }
    }

    [StreamDeckAction("dev.cazzar.vtubestudio.scalemodel")]
    public class ScaleModel : BaseAction<ScaleModel.SettingsModel>, IDeviceAware, IPropertyInspector
    {
        private readonly ILogger<ScaleModel> _logger;

        public class SettingsModel
        {
            public double Size { get; set; }
            public double Scale { get; set; }
        }

        public ScaleModel(IStreamDeckConnection connection, ILogger<ScaleModel> logger) : base(connection)
        {
            _logger = logger;
        }

        public override void KeyDown(KeyActionPayload keyActionPayload)
        {
            this.Connection.OpenUrl("https://twitch.tv/projectbombshell");
        }

        public string DeviceId { get; set; }
        
        public void Appeared(PropertyInspectorDidAppear didAppear)
        {
            
        }

        public void Disappeared(PropertyInspectorDidDisappear didDisappear)
        {
            
        }

        public void OnSendToPlugin(SendToPlugin sendToPlugin)
        {
            
        }
    }
}
