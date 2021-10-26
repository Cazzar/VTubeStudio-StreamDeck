
namespace StreamDeckLib.Models
{
    public record DeviceDidConnect : EventMessage
    {
        public override string Event { get; init; } = "deviceDidConnect";

        public string Device { get; set; } = default;

        public DeviceInfo DeviceInfo { get; set; } = default;
    }
}
