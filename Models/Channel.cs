namespace Server.Models;
public class Channel : AbstractChannel {
    public long ServerId { get; set; }
    public GuildServer Server { get; set; } = null!;
    public long? ParentId { get; set; } = null;
    public Channel? Parent { get; set; } = null;
    public List<Channel>? Children { get; set; } = null;
    public string Name { get; set; } = "";
    public string? Description { get; set; }
    public string? Icon { get; set; }
    public ushort Position { get; set; }
    public ulong Slowmode { get; set; }
    public DateTime CreatedAt { get; set; }
}