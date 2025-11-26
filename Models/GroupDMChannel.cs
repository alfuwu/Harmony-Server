namespace Server.Models;
public class GroupDmChannel : AbstractChannel {
    public long OwnerId { get; set; }
    public string? Name { get; set; }
    public string? Description { get; set; }
    public string? Icon { get; set; }
    public List<string>? InviteUrls { get; set; }
    public List<long> Members { get; set; } = [];
    public DateTime CreatedAt { get; set; }
}
