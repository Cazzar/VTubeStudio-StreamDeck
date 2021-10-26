namespace StreamDeckLib.Models
{
    public record ContextMessage : EventMessage
    {
        public override string Event { get; init; }
        public string Context { get; set; }
    }
}