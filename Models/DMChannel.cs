namespace Server.Models;
public class DMChannel : AbstractChannel {
    public List<long> Members { get; set; } = [];
}
