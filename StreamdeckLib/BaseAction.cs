using System;
using System.Security.Cryptography.X509Certificates;
using Newtonsoft.Json.Linq;
using StreamDeckLib.Models;

namespace StreamDeckLib
{
    public class BaseAction<T> : IHasSettings, IContextAware, IButtonReactions where T : new()
    {
        public BaseAction(IStreamDeckConnection connection)
        {
            Connection = connection;
        }

        public T Settings { get; private set; } = new T();
        
        public IStreamDeckConnection Connection { get; set; }

        public virtual void GotSettings(JObject settings)
        {
            Settings = settings.ToObject<T>() ?? new T();
        }
        
        public string ContextId { get; set; }
        public virtual void KeyUp(KeyActionPayload keyActionPayload)
        {
        }

        public virtual void KeyDown(KeyActionPayload keyActionPayload)
        {
        }

        public async void SaveSettings(T? settings = default)
        {
            await Connection.SendMessage(new SetSettings() { Context = ContextId, Payload = (settings ?? Settings)!, });
        }

        public async void ShowAlert()
        {
            await Connection.SendMessage(new ContextMessage() { Context = ContextId, Event = "showAlert" });
        }
        
        public async void ShowOk()
        {
            await Connection.SendMessage(new ContextMessage() { Context = ContextId, Event = "showOk" });
        }
        
        public async void RequestSettings()
        {
            await Connection.SendMessage(new ContextMessage() { Context = ContextId, Event = "getSettings" });
        }

        public async void SetTitle(string? title, int state = 0)
        {
            await Connection.SendMessage(new SetTitle() { Context = ContextId, Payload = new(title, 0, 0)});
        }
    }
}
