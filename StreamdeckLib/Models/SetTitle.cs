namespace StreamDeckLib.Models
{
    record SetTitle : ContextMessage
    {
        public override string Event => "setTitle";
        public TitlePayload Payload { get; set; }
    }
}
