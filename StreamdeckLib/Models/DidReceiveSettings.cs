namespace StreamDeckLib.Models
{
    public record DidReceiveSettings : EventMessage
    {
        public override string Event { get; init; } = "didReceiveSettings";
        public string Context { get; set; } = default;
        public string Device { get; set; } = default;
        public SettingsPayload Payload { get; set; } = default;
    }
}
