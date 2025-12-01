using Server.Models.Enums;

namespace Server.Models;
public class Message : Identifiable {
    public long ChannelId { get; set; }
    public AbstractChannel Channel { get; set; } = null!;
    public long AuthorId { get; set; }
    public List<long>? Mentions { get; set; }
    public List<Reaction>? Reactions { get; set; }
    public string Content { get; set; } = "";
    /// <summary>
    /// Previous versions of the message content, if edited.
    /// </summary>
    public List<string>? PreviousContent { get; set; }
    /// <summary>
    /// Messages this one is replying to.
    /// </summary>
    public List<long>? References { get; set; }
    public MessageType Type { get; set; }
    public List<Attachment>? Attachments { get; set; }
    public DateTime Timestamp { get; set; }
    public DateTime? EditedTimestamp { get; set; } = null;
    public bool IsDeleted { get; set; }
    public bool IsPinned { get; set; }
}