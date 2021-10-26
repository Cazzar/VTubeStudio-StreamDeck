namespace StreamDeckLib.Models
{
    public record GetSettings : EventMessage
    {
        public override string Event => "getSettings";
        public string Context { get; set; }
    }
}