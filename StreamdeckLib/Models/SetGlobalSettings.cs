namespace StreamDeckLib.Models
{
    public record SetGlobalSettings : EventMessage
    {
        public override string Event => "setGlobalSettings";
        public string Context { get; set; }
        public object Payload { get; set; }
    }
}