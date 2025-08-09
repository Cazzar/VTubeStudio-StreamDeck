namespace StreamDeckLib.Models
{
    public record StatePayload(uint State);

    record TitlePayload(string? Title, int Target, int State);
}