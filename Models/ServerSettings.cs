namespace Server.Models;
public class ServerSettings {
    public long ServerId { get; set; }
    public GuildServer Server { get; set; } = null!;
}
