
using System.IO;
using System.Threading.Tasks;
using Cazzar.StreamDeck.VTubeStudio.VTubeStudioApi;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using NLog.Extensions.Logging;
using StreamDeckLib;
using StreamDeckLib.Extensions;

namespace Cazzar.StreamDeck.VTubeStudio
{
    class Program
    {
        public static async Task Main(string[] args)
        {
            // while (!System.Diagnostics.Debugger.IsAttached) { System.Threading.Thread.Sleep(100); }

            #if DEBUG
            args = await File.ReadAllLinesAsync(@"C:\Users\me\AppData\Roaming\Elgato\StreamDeck\Plugins\dev.cazzar.streamdeck.vtubestudio.sdPlugin\argv.txt");
            #endif

            var hostBuilder = CreateHostBuilder(args);
            var build = hostBuilder.Build();
            var sp = build.Services;

            await build.RunAsync();
        }

        private static IHostBuilder CreateHostBuilder(string[] args)
        {
            var config = new ConfigurationBuilder()
                .SetBasePath(System.IO.Directory.GetCurrentDirectory()) //From NuGet Package Microsoft.Extensions.Configuration.Json
                .AddJsonFile("appsettings.json", optional: true, reloadOnChange: true)
                .Build();


            return Host.CreateDefaultBuilder(args)
                .ConfigureStreamDeckToolkit(args)
                .ConfigureServices(
                    (hostContext, services) =>
                    {
                        services.AddStreamDeck(hostContext.Configuration);
                        services.AddSingleton<VTubeStudioWebsocketClient>();
                        services.AddSingleton<IAuthManger, AuthManager>();
                        services.AddSingleton<GlobalSettingsManager>();
                        services.AddSingleton<IGlobalSettingsHandler>((sp) => sp.GetService<GlobalSettingsManager>()!);
                        services.AddSingleton<HotkeyCache>();
                        services.AddSingleton<ModelCache>();
                        services.AddLogging(
                            logging =>
                            {
                                logging.ClearProviders();
                                logging.SetMinimumLevel(LogLevel.Trace);
                                logging.AddNLog(config);
                            }
                        );
                    }
                );
        }
    }
}
