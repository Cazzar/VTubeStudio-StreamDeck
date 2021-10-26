using System.Threading.Tasks;
using StreamDeckLib.Models;

namespace StreamDeckLib
{
    public interface IPropertyInspector : IContextAware
    {
        public IStreamDeckConnection Connection { get; set; }
        
        public void Appeared(PropertyInspectorDidAppear didAppear); 
        public void Disappeared(PropertyInspectorDidDisappear didDisappear);
        public void OnSendToPlugin(SendToPlugin sendToPlugin);

        public async Task SendToPropertyInspector(object data)
        {
            await this.Connection.SendMessage(new SendToPropertyInspector { Context = ContextId, Payload = data });
        }
    }
}