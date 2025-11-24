namespace Server.Models;
public class Message {
    public long Id { get; set; }
    public long ChannelId { get; set; }
    public long AuthorId { get; set; }
    public IEnumerable<long>? Mentions { get; set; }
    public string? Content { get; set; }
    public DateTime Timestamp { get; set; }
    public DateTime EditedTimestamp { get; set; }
    public bool IsDeleted { get; set; }
}