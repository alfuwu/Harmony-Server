namespace Server.Models;
public class ThreadChannel : Channel {
    public long OwnerId { get; set; }
    public List<long> Members { get; set; } = [];
    public ulong ArchiveAfter { get; set; } = 4320; // default 3 days
    public bool Archived { get; set; }
    public bool Private { get; set; }
    public bool Locked { get; set; }
}
