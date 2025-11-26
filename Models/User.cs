namespace Server.Models;
public class User : Identifiable {
    public string? DisplayName { get; set; }
    public string Username { get; set; } = "";
    public string PasswordHash { get; set; } = "";
    public string? Bio { get; set; }
    public string? Avatar { get; set; }
    public DateTime JoinedAt { get; set; }
    public DateTime LastSeen { get; set; }
    public bool IsDeleted { get; set; }
    public ulong Flags { get; set; }
}