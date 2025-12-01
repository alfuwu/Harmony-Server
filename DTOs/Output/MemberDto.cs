using Server.Models;
using Server.Models.Enums.Settings;
using Server.Services;

namespace Server.DTOs.Output;
public class MemberDto(Member member) {
    public UserDto User { get; set; } = new(member.User);
    public long ServerId { get; set; } = member.ServerId;

    public string? Nickname { get; set; } = member.Nickname;
    public string? Bio { get; set; } = member.Bio;
    public string? Pronouns { get; set; } = member.Pronouns;
    public string? Avatar { get; set; } = member.Avatar;
    public string? Banner { get; set; } = member.Banner;
    public string? NameFont { get; set; } = member.NameFont;

    public DateTime JoinedAt { get; set; } = member.JoinedAt;

    public List<long> Roles { get; set; } = member.Roles/*.Select(r => r.Id).ToList()*/;

    public async Task Redact(IRelationshipService relationships, User self, long requestor) {
        var relationship = await User.Redact(relationships, self, requestor);

        // ignore no one context in this case since these have to be manually overridden per server
        if (self.WhoCanSeeBio != UserContext.NoOne && !relationships.CanSee(relationship, self.WhoCanSeeBio))
            Bio = null;
        if (self.WhoCanSeePronouns != UserContext.NoOne && !relationships.CanSee(relationship, self.WhoCanSeePronouns))
            Pronouns = null;
        if (self.WhoCanSeeAvatar != UserContext.NoOne && !relationships.CanSee(relationship, self.WhoCanSeeAvatar))
            Avatar = null;
        if (self.WhoCanSeeBanner != UserContext.NoOne && !relationships.CanSee(relationship, self.WhoCanSeeBanner))
            Banner = null;
    }
}
