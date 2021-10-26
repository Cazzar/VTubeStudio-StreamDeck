namespace StreamDeckLib.Models
{
    public record DeviceInfo
    {
        public string Name { get; set; } = default;
        public StreamDeckDevice Type { get; set; } = default;
        public Size Size { get; set; } = default;
    }
}
