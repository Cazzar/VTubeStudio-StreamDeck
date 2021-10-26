namespace StreamDeckLib.Models
{
    public record DeviceDidDisconnect(string Device = default) : EventMessage;
}