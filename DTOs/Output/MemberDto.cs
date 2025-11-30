using Server.Models;

namespace Server.DTOs.Output;
public class MemberDto(Member member) {
    public UserDto User { get; set; } = new(member.User);
    public long ServerId { get; set; } = member.ServerId;

    public string? Nickname { get; set; } = member.Nickname;
    public string? Bio { get; set; } = member.Bio;
    public string? Pronouns { get; set; } = member.Pronouns;
    public string? Avatar { get; set; } = member.Avatar;
    public string? NameFont { get; set; } = member.NameFont;

    public DateTime JoinedAt { get; set; } = member.JoinedAt;

    public List<long> Roles { get; set; } = member.Roles/*.Select(r => r.Id).ToList()*/;
}
