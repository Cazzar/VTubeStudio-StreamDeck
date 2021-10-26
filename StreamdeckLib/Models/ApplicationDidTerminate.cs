namespace StreamDeckLib.Models
{
    public record ApplicationDidTerminate(ApplicationPayload Payload = default) : EventMessage;
}