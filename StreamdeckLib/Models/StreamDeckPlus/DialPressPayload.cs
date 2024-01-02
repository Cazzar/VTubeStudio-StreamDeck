using Newtonsoft.Json.Linq;

namespace StreamDeckLib.Models.StreamDeckPlus;

public record DialPressPayload {
	public string Controller { get; set; }
	public JObject Settings { get; set; }
	public Coordinates Coordinates { get; set; }
}
