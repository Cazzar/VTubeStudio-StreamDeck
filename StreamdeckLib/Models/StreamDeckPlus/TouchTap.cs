namespace StreamDeckLib.Models.StreamDeckPlus;

public record TouchTap : ContextMessage {
	public override string Event { get; init; } = "touchTap";
	public string Action { get; set; }
	public string Device { get; set; }
	public TouchTapPayload Payload { get; set; }
}