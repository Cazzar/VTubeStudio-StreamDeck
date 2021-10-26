using StreamDeckLib.Models;

namespace StreamDeckLib
{
    public interface IApplicationHandler
    {
        public void Launched(ApplicationDidLaunch launched);
        public void Terminated(ApplicationDidTerminate terminate);
    }
}