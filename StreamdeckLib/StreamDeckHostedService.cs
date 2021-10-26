using System;
using System.Net.WebSockets;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using StreamDeckLib.Extensions;
using StreamDeckLib.Extensions.Models;

namespace StreamDeckLib
{
    public class StreamDeckHostedService : IHostedService
    {
        private readonly ILogger<StreamDeckHostedService> _logger;
        private readonly StreamDeckRegistrationOptions _options;
        private readonly IStreamDeckConnection _connection;
        private readonly ActionRepository _actionRepository;
        private readonly IHostApplicationLifetime _lifetime;

        public StreamDeckHostedService(
            ILogger<StreamDeckHostedService> logger,
            IOptions<StreamDeckRegistrationOptions> options,
            IStreamDeckConnection streamDeckConnection,
            ActionRepository actionRepository,
            IHostApplicationLifetime lifetime
        )
        {
            _logger = logger;
            _options = options.Value;
            _connection = streamDeckConnection;
            _actionRepository = actionRepository;
            _lifetime = lifetime;
        }

        public Task StartAsync(CancellationToken cancellationToken)
        {
            _lifetime.ApplicationStarted.Register(() => Task.Run(OnStarted, cancellationToken));
            _lifetime.ApplicationStopping.Register(OnStopping);
            
            return Task.CompletedTask;
        }

        private async void OnStopping()
        {
            await _connection.Cancel();
        }

        private async void OnStarted()
        {
            _actionRepository.FindActions();
            await _connection.Run();
        }

        public Task StopAsync(CancellationToken cancellationToken) => Task.CompletedTask;
    }
}
