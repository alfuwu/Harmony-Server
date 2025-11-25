namespace Server.Models;
public class GuildServer {
    public long Id { get; set; }
    public long OwnerId { get; set; }
    public string Name { get; set; } = "";
    public string? Description { get; set; }
    public List<string>? Tags { get; set; }
    public List<string>? InviteUrls { get; set; }
    public List<long> Members { get; set; } = [];
}
