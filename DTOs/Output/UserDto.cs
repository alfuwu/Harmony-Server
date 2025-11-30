using Server.Models;

namespace Server.DTOs.Output;
public class UserDto(User user) {
    public long Id { get; set; } = user.Id;
    public string? DisplayName { get; set; } = user.DisplayName;
    public string Username { get; set; } = user.Username;
    public string? PasswordHash { get; set; } = user.PasswordHash;
    public string? Bio { get; set; } = user.Bio;
    public string? Pronouns { get; set; } = user.Pronouns;
    public string? Avatar { get; set; } = user.Avatar;
    public string? NameFont { get; set; } = user.NameFont;
    public DateTime JoinedAt { get; set; } = user.JoinedAt;
    public DateTime LastSeen { get; set; } = user.LastSeen;
    public bool IsDeleted { get; set; } = user.IsDeleted;
    public ulong Flags { get; set; } = user.Flags;
}