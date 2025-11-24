using Server.Models.Enums;

namespace Server.Models;
public class Channel {
    public long Id { get; set; }
    public string? Name { get; set; }
    public ChannelType Type { get; set; }
}