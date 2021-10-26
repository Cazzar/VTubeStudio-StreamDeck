namespace StreamDeckLib.Models
{
    public record ApplicationDidLaunch(ApplicationPayload Payload = default) : EventMessage;
}