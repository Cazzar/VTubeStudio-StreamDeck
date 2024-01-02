namespace StreamDeckLib.Models.StreamDeckPlus;

public record DialRotate : ContextMessage {
	public override string Event { get; init; } = "dialRotate";
	public string Action { get; set; }
	public string Device { get; set; }
	public DialRotatePayload Payload { get; set; }
}