using Cazzar.Deck.Core;
using Cazzar.Deck.Hosts.StreamDeck;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using NLog.Extensions.Logging;
using Cazzar.Deck.VTubeStudio;
using Microsoft.Extensions.DependencyInjection;
using VTubeStudio.Api;
using VTubeStudio.StreamDeck;

var builder = Host.CreateApplicationBuilder(args);

builder.Configuration.AddStreamDeckCommandLine(args);

builder.Services.AddStreamDeckHost(builder.Configuration)
    .AddDeckCore()
    .AddSingleton<IVtsPluginInfo, StreamDeckPluginInfo>()
    .AddVTubeStudio();

builder.Logging.ClearProviders()
    .SetMinimumLevel(LogLevel.Trace)
    .AddNLog();

await builder.Build().RunAsync();
