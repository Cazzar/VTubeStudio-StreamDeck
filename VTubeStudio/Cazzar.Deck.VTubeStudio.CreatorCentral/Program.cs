using Cazzar.Deck.Core;
using Cazzar.Deck.Hosts.CreatorCentral;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using NLog.Extensions.Logging;
using Cazzar.Deck.VTubeStudio;
using Microsoft.Extensions.DependencyInjection;
using VTubeStudio.Api;
using VTubeStudio.CreatorCentral;

var builder = Host.CreateApplicationBuilder(args);

builder.Configuration.AddCreatorCentralCommandLine(args);

builder.Services.AddCreatorCentralHost(builder.Configuration)
    .AddDeckCore()
    .AddSingleton<IVtsPluginInfo, CreatorCentralPluginInfo>()
    .AddVTubeStudio();

builder.Logging.ClearProviders()
    .SetMinimumLevel(LogLevel.Trace)
    .AddNLog();

await builder.Build().RunAsync();
