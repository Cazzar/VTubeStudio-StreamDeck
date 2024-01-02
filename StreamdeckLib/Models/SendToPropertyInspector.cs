using System.Collections.Generic;
using Newtonsoft.Json.Linq;

namespace StreamDeckLib.Models
{
    public record SendToPropertyInspector : ContextMessage
    {
        public override string Event => "sendToPropertyInspector";
        public object Payload { get; set; }
    }
    
    public record SetFeedback : ContextMessage
    {
        public override string Event => "setFeedback";
        public Dictionary<string, object> Payload { get; set; }
    }
}