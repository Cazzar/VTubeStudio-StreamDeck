namespace StreamDeckLib.Models
{
    public record RegisterEvent: EventMessage
    {
        public string Uuid { get; set; }
    }
}
