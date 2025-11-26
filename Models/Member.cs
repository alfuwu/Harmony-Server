namespace Server.Models;
public class Member {
    public long ServerId { get; set; }
    public GuildServer Server { get; set; } = null!;

    public long UserId { get; set; }
    public User User { get; set; } = null!;

    public string? Nickname { get; set; } = null;

    public List<long> Roles { get; set; } = [];

    public DateTime JoinedAt { get; set; }
}
