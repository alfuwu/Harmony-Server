using Server.Models;

namespace Server.DTOs.Output;
public class ServerDto(GuildServer server) {
    public long Id { get; set; } = server.Id;
    public long OwnerId { get; set; } = server.OwnerId;
    public string Name { get; set; } = server.Name;
    public string? Description { get; set; } = server.Description;
    public string? Icon { get; set; } = server.Icon;
    public string? Banner { get; set; } = server.Banner;
    public string? InviteBanner { get; set; } = server.InviteBanner;
    public List<string>? Tags { get; set; } = server.Tags;
    public List<string>? InviteUrls { get; set; } = server.InviteUrls;
    public List<RoleDto> Roles { get; set; } = [.. server.Roles.Select(r => new RoleDto(r))];
    public List<Emoji> Emojis { get; set; } = server.Emojis;
    public DateTime CreatedAt { get; set; } = server.CreatedAt;
}
