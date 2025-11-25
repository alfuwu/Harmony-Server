namespace Server.Models;
public class Message : Identifiable {
    public long ChannelId { get; set; }
    public long AuthorId { get; set; }
    public List<long>? Mentions { get; set; }
    public List<Reaction>? Reactions { get; set; }
    public string? Content { get; set; }
    /// <summary>
    /// Previous versions of the message content, if edited.
    /// </summary>
    public string[]? PreviousContent { get; set; }
    public DateTime Timestamp { get; set; }
    public DateTime EditedTimestamp { get; set; }
    public bool IsDeleted { get; set; }
    public bool IsPinned { get; set; }
}