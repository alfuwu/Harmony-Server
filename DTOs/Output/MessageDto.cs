using Server.Models;
using Server.Models.Enums;

namespace Server.DTOs.Output;
public class MessageDto(Message message) {
    public long Id { get; set; } = message.Id;
    public long ChannelId { get; set; } = message.ChannelId;
    public long AuthorId { get; set; } = message.AuthorId;
    public bool? MentionsEveryone { get; set; } = message.MentionsEveryone;
    public List<long>? Mentions { get; set; } = message.Mentions;
    public List<long>? MentionRoles { get; set; } = message.MentionRoles;
    public List<Reaction>? Reactions { get; set; } = message.Reactions;
    public string Content { get; set; } = message.Content;
    public List<string>? PreviousContent { get; set; } = message.PreviousContent;
    public List<long>? References { get; set; } = message.References;
    public MessageType Type { get; set; } = message.Type;
    public DateTime Timestamp { get; set; } = message.Timestamp;
    public DateTime? EditedTimestamp { get; set; } = message.EditedTimestamp;
    public bool IsDeleted { get; set; } = message.IsDeleted;
    public bool IsPinned { get; set; } = message.IsPinned;
}