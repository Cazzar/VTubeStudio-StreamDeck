namespace StreamDeckLib.Models
{
    public record SetSettings : EventMessage
    {
        public override string Event => "setSettings";
        public string Context { get; set; }
        public object Payload { get; set; }
    }
}
