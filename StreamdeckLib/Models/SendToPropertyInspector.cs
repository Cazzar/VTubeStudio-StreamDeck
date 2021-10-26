namespace StreamDeckLib.Models
{
    public record SendToPropertyInspector : ContextMessage
    {
        public override string Event => "sendToPropertyInspector";
        public object Payload { get; set; }
    }
}