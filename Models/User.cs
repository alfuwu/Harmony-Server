using Server.Models.Enums;
using Server.Models.Enums.Settings;

namespace Server.Models;
public class User : Identifiable {
    public string Email { get; set; } = "";
    public bool EmailVerified { get; set; }
    public string? PhoneNumber { get; set; }
    public bool PhoneNumberVerified { get; set; }
    public string? DisplayName { get; set; }
    public string Username { get; set; } = "";
    public string PasswordHash { get; set; } = "";
    public string? Status { get; set; }
    public OnlineStatus OnlineStatus { get; set; }
    public string? Bio { get; set; }
    public string? Pronouns { get; set; }
    public string? Avatar { get; set; }
    public string? Banner { get; set; }
    public int BannerColor { get; set; }
    public string? NameFont { get; set; }
    public DateTime JoinedAt { get; set; }
    public DateTime LastSeen { get; set; }
    public bool IsDeleted { get; set; }
    public ulong Flags { get; set; }
    public int? DmColor { get; set; }
    public List<int>? DmColors { get; set; }
    public RoleDisplayType DmNameDisplayType { get; set; }

    // settings stuff
    public UserSettings Settings { get; set; } = null!;
    public FriendRequestContext WhoCanSendFriendRequests = FriendRequestContext.Everyone;
    public UserContext WhoCanSendDms = UserContext.MutualsAndFriends;
    public UserContext WhoCanAddToGcs = UserContext.MutualsAndFriends;
    public UserContext WhoCanSeeEmail = UserContext.NoOne;
    public UserContext WhoCanSeePhoneNumber = UserContext.NoOne;
    public UserContext WhoCanSeeBio = UserContext.Everyone;
    public UserContext WhoCanSeePronouns = UserContext.Everyone;
    public UserContext WhoCanSeeAvatar = UserContext.Everyone;
    public UserContext WhoCanSeeBanner = UserContext.Everyone;
    public UserContext WhoCanSeeStatus = UserContext.Everyone;
    // silly
    public UserContext WhoCanSeePasswordHash = UserContext.NoOne;
}