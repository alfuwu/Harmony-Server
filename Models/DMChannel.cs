namespace Server.Models;
public class DmChannel : AbstractChannel {
    public List<long> Members { get; set; } = [];
    public bool IsDeleted { get; set; }
}
