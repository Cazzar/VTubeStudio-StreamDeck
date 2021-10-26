namespace StreamDeckLib.Models
{
    public record GlobalSettings : EventMessage
    {
        public override string Event { get; init; } = "didReceiveGlobalSettings";

        public GlobalSettingsPayload Payload { get; set; } = default;
    }
}
