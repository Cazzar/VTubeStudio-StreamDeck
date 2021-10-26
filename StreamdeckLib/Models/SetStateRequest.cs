namespace StreamDeckLib.Models
{
    record SetStateRequest : ContextMessage
    {
        public override string Event => "setState";
        public StatePayload Payload { get; set; }
    }
}