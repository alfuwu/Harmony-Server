namespace Server.Models;
public class GroupDMChannel : AbstractChannel { // can't inherit from DMChannel because microsoft's a BITCH
    // ugh
    public long OwnerId { get; set; }
    public string? Name { get; set; }
    public string? Description { get; set; }
    public string? Icon { get; set; }
    public List<long> Members { get; set; } = [];
    public DateTime CreatedAt { get; set; }
}
