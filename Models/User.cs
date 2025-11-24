namespace Server.Models;
public class User {
    public long Id { get; set; }
    public string? Username { get; set; }
    public string? PasswordHash { get; set; }
    public DateTime LastSeen { get; set; }
}