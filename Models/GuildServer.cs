namespace Server.Models;
public class GuildServer : Identifiable {
    public long OwnerId { get; set; }
    public string Name { get; set; } = "";
    public string? Description { get; set; }
    public string? Icon { get; set; }
    public string? Banner { get; set; } = null;
    public string? InviteBanner { get; set; } = null;
    public List<string>? Tags { get; set; }
    public List<string>? InviteUrls { get; set; }
    public List<Member> Members { get; set; } = [];
    public List<Role> Roles { get; set; } = [];
    public List<Emoji> Emojis { get; set; } = [];
    public DateTime CreatedAt { get; set; }
}
