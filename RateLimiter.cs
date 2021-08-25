using System;
using System.Threading;
using System.Threading.Tasks;

namespace Cazzar.StreamDeck.VTubeStudio
{
    public class RateLimiter
    {
        public static RateLimiter ChangeModel { get; } = new RateLimiter(TimeSpan.FromSeconds(2.5));

        public bool IsReady { get; private set; } = true;
        
        private readonly TimeSpan _timeout;

        private RateLimiter(TimeSpan timeout)
        {
            _timeout = timeout;
        }
        
        public async void Trigger()
        {
            IsReady = false;
            await Task.Delay(_timeout);
            IsReady = true;
        }
    }
}
