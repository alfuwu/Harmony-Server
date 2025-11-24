namespace Server.DTOs;
public class MessageDto {
    public long Id { get; set; }
    public long ChannelId { get; set; }
    public long AuthorId { get; set; }
    public string? Content { get; set; }
    public DateTime Timestamp { get; set; }
}
