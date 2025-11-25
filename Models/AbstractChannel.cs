using Server.Models.Enums;

namespace Server.Models;
public abstract class AbstractChannel : Identifiable {
    public ChannelType Type { get; set; }
    public long? LastMessage { get; set; } = null;
}