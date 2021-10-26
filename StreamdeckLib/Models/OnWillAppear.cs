namespace StreamDeckLib.Models
{
    public record OnWillAppear : EventMessage
    {
        public override string Event { get; init; } = "willAppear";

        public string Action { get; set; }
        public string Context { get; set; }
        public string Device { get; set; }
        public AppearanceChangePayload Payload { get; set; }
    }

    // public record AppearPayload
    // {
    //     public JObject Settings { get; set; }
    //     public Coordinates Coordinates { get; set; }
    //     public int State { get; set; }
    //     public bool IsInMultiAction { get; set; }
    // }
}