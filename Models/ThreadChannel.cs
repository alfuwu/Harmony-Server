namespace Server.Models;
public class ThreadChannel : AbstractChannel { // see GroupDMChannel
    public long OwnerId { get; set; }
    public long ParentId { get; set; }
    public long ServerId { get; set; }
    public string Name { get; set; } = "";
    public string? Description { get; set; }
    public string? Icon { get; set; }
    public List<long> Members { get; set; } = [];
    public ulong Slowmode { get; set; }
    public DateTime CreatedAt { get; set; }
    public ulong ArchiveAfter { get; set; } = 4320; // default 3 days
    public bool Archived { get; set; }
    public bool Private { get; set; }
    public bool Locked { get; set; }
}
