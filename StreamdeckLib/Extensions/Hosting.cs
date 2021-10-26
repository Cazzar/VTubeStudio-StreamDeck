using System.Collections.Generic;
using System.Runtime.CompilerServices;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Hosting;

namespace StreamDeckLib.Extensions
{
    public static class Hosting
    {
        public static IHostBuilder ConfigureStreamDeckToolkit(this IHostBuilder hostBuilder, string[] args)
        {
            hostBuilder
                .ConfigureAppConfiguration((_, config) =>
                {
                    var switchMappings = new Dictionary<string, string>()
                    {
                        { "-port", "StreamDeck:Port" },
                        { "--port", "StreamDeck:Port" },
                        { "-pluginUUID", "StreamDeck:PluginUuid" },
                        { "-info", "StreamDeck:Info" },
                        { "-registerEvent", "StreamDeck:RegisterEvent" },
                    };
                    config.AddCommandLine(args, switchMappings);
                });
            return hostBuilder;
        }
    }
}
