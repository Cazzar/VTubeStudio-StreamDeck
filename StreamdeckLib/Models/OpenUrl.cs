namespace StreamDeckLib.Models
{
    public record OpenUrl : EventMessage
    {
        public override string Event => "openUrl";
        public UrlPayload Payload { get; set; }
    }
}