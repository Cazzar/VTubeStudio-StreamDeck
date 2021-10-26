using System;
using System.Reflection;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using StreamDeckLib.Extensions.Models;

namespace StreamDeckLib.Extensions
{
    public static class DependencyInjection
    {
        public static IServiceCollection AddStreamDeck(this IServiceCollection services, IConfiguration configuration)
        {
            return AddStreamDeck(services, configuration, Assembly.GetEntryAssembly()!);
        }

        public static IServiceCollection AddStreamDeck(this IServiceCollection services, IConfiguration configuration, Assembly assembly)
        {
            services.Configure<StreamDeckLaunchOptions>(configuration.GetSection("StreamDeck"));
            services.Configure<StreamDeckRegistrationOptions>(config =>
            {
                config.LoadingAssembly = assembly;
            });

            services.AddHostedService<StreamDeckHostedService>();
            services.AddHostedService<ActionTimer>();
            services.AddSingleton<IStreamDeckConnection, StreamDeckConnection>();
            services.AddSingleton<ActionRepository>();
            

            return services;
        }
    }
}
