using Server.Models;
using Server.Models.Enums;
using Server.Services;

namespace Server.DTOs.Output;
public class UserDto(User user) {
    public long Id { get; set; } = user.Id;
    public string? Email { get; set; } = user.Email;
    public string? PhoneNumber { get; set; } = user.PhoneNumber;
    public string? DisplayName { get; set; } = user.DisplayName;
    public string Username { get; set; } = user.Username;
    public string? PasswordHash { get; set; } = user.PasswordHash;
    public string? Status { get; set; } = user.Status;
    public OnlineStatus OnlineStatus { get; set; } = user.OnlineStatus;
    public string? Bio { get; set; } = user.Bio;
    public string? Pronouns { get; set; } = user.Pronouns;
    public string? Avatar { get; set; } = user.Avatar;
    public string? Banner { get; set; } = user.Banner;
    public int BannerColor { get; set; } = user.BannerColor;
    public string? NameFont { get; set; } = user.NameFont;
    public DateTime JoinedAt { get; set; } = user.JoinedAt;
    public DateTime LastSeen { get; set; } = user.LastSeen;
    public bool IsDeleted { get; set; } = user.IsDeleted;
    public ulong Flags { get; set; } = user.Flags;

    public async Task<Relationship> Redact(IRelationshipService relationships, User self, long? requestor) {
        var relationship = requestor.HasValue ? await relationships.GetUserContext(Id, requestor.Value) : Relationship.Everyone;
        if (!relationships.CanSee(relationship, self.WhoCanSeeEmail))
            Email = null;
        if (!relationships.CanSee(relationship, self.WhoCanSeePhoneNumber))
            PhoneNumber = null;
        if (!relationships.CanSee(relationship, self.WhoCanSeeBio))
            Bio = null;
        if (!relationships.CanSee(relationship, self.WhoCanSeePronouns))
            Pronouns = null;
        if (!relationships.CanSee(relationship, self.WhoCanSeeAvatar))
            Avatar = null;
        if (!relationships.CanSee(relationship, self.WhoCanSeeBanner)) {
            Banner = null;
            BannerColor = 0;
        }
        if (!relationships.CanSee(relationship, self.WhoCanSeeStatus))
            Status = null;
        if (!relationships.CanSee(relationship, self.WhoCanSeePasswordHash))
            PasswordHash = null;
        return relationship;
    }
}