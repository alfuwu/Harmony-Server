namespace Server.DTOs;
public class ChannelCreateDto {
    public long Id { get; set; }
    public long ServerId { get; set; }
    public string? Name { get; set; }
    public string? Description { get; set; }
}
