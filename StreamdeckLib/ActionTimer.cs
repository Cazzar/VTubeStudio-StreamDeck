using System.Security.Cryptography.X509Certificates;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.Hosting;

namespace StreamDeckLib
{
    public class ActionTimer : IHostedService
    {
        private readonly ActionRepository _repository;
        private readonly IHostApplicationLifetime _lifetime;
        private readonly CancellationTokenSource _cancellationToken = new();

        public ActionTimer(ActionRepository repository, IHostApplicationLifetime lifetime)
        {
            _repository = repository;
            _lifetime = lifetime;
        }
        
        public Task StartAsync(CancellationToken cancellationToken)
        {
            _lifetime.ApplicationStarted.Register(() => Task.Run(OnStarted, cancellationToken));
            return Task.CompletedTask;
        }

        private void OnStarted()
        {
            while (!_cancellationToken.IsCancellationRequested)
            {
                _repository.Tick();
                
                Thread.Sleep(1000);
            }
        }

        public Task StopAsync(CancellationToken cancellationToken)
        {
            _cancellationToken.Cancel();
            return Task.CompletedTask;
        }
    }
}
