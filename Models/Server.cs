namespace Server.Models;
public class Server {
    public long Id { get; set; }
    public string? Name { get; set; }
    public string? Description { get; set; }
    public string[]? Tags { get; set; }
    public string[]? InviteUrls { get; set; }
}
