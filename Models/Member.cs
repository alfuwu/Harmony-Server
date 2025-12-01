namespace Server.Models;
public class Member {
    public long ServerId { get; set; }
    public GuildServer Server { get; set; } = null!;

    public long UserId { get; set; }
    public User User { get; set; } = null!;

    // override display name
    public string? Nickname { get; set; } = null;
    // override bio
    public string? Bio { get; set; } = null;
    // override pronouns
    public string? Pronouns { get; set; } = null;
    // override avatar
    public string? Avatar { get; set; } = null;
    // override banner
    public string? Banner { get; set; } = null;
    // override font
    public string? NameFont { get; set; } = null;

    public List<long> Roles { get; set; } = [];

    public DateTime JoinedAt { get; set; }
}
