using Server.Models.Enums;

namespace Server.DTOs.Input;
public class MessageCreateDto {
    public string Content { get; set; } = "";
    public long[]? Attachments { get; set; }
    public long[]? References { get; set; }
    public MessageType Type { get; set; } = MessageType.Normal;
    public long Nonce { get; set; }
}
