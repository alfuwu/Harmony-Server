namespace Server.DTOs;
public class ServerCreateDto {
    public string Name { get; set; } = "";
    public string? Description { get; set; }
    public List<string>? Tags { get; set; }
    public List<string>? InviteUrls { get; set; }
}
