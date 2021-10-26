namespace StreamDeckLib.Models
{
    public record GetGlobalSettings : EventMessage
    {
        public override string Event => "getGlobalSettings";
        public string Context { get; set; }
    }
}