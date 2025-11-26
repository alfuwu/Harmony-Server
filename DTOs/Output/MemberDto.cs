using Server.Models;

namespace Server.DTOs.Output;
public class MemberDto(Member member) {
    public long Id { get; set; } = member.UserId;
    public User User { get; set; } = member.User;
    public long ServerId { get; set; } = member.ServerId;

    public string? Nickname { get; set; } = member.Nickname;

    public DateTime JoinedAt { get; set; } = member.JoinedAt;

    public List<long> Roles { get; set; } = member.Roles/*.Select(r => r.Id).ToList()*/;
}
